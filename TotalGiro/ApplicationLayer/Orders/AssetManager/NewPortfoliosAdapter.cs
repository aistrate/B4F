using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.Orders.AssetManager
{
    public static class NewPortfoliosAdapter
    {
        public static bool BuyModel(int accountId, OrderActionTypes actionType, bool noCharges, decimal depositCashPositionDiff, bool includePrevCash)
        {
            return NewPortfoliosAdapter.rebalanceAccount(accountId, InstructionTypes.BuyModel, actionType, noCharges, depositCashPositionDiff, includePrevCash);
        }

        public static bool Rebalance(int accountId, OrderActionTypes actionType, bool noCharges)
        {
            return NewPortfoliosAdapter.rebalanceAccount(accountId, InstructionTypes.Rebalance, actionType, noCharges, 0M, false);
        }

        internal static bool rebalanceAccount(int accountId, InstructionTypes instructionType, OrderActionTypes actionType, bool noCharges, decimal depositCashPositionDiff, bool includePrevCash)
        {
            bool success = false;
            Money diff = null;
            IDalSession session = NHSessionFactory.CreateSession();

            IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
            if (account.ActiveRebalanceInstructions != null && account.ActiveRebalanceInstructions.Count > 0)
                throw new ApplicationException(string.Format("The account {0} already has an active rebalance instruction.", account.Number));

            IList<IJournalEntryLine> cashTransfers = JournalEntryMapper.GetUnProcessedCashTransfers(session, account);
            if (cashTransfers == null || cashTransfers.Count == 0)
                throw new ApplicationException("It is not possible to do this rebalance without cash transfers");
            else
                diff = new Money(depositCashPositionDiff, cashTransfers[0].Currency);

            if (account.ModelPortfolio == null)
                throw new ApplicationException(string.Format("The account {0} does not have a model attached.", account.Number));


            IInstruction instruction = account.CreateInstruction(instructionType, actionType, DateTime.Now.Date, noCharges, cashTransfers);
            if (instruction != null)
            {
                // check total value of the transfers
                if (instructionType == InstructionTypes.BuyModel)
                {
                    IBuyModelInstruction bmi = (IBuyModelInstruction)instruction;
                    if (!(bmi.CashTransfers.TotalTransferAmount.IsGreaterThanZero && account.TotalCash.IsGreaterThanZero))
                        throw new ApplicationException("It is not possible to do this rebalance with a zero/negative cash transfer amount");

                    if (!(account.ActiveWithdrawalInstructions.Count == 0 && account.ActiveMoneyTransferOrders.Count == 0))
                        throw new ApplicationException(string.Format("It is not possible to do a buy model for account {0} since there are active money transfers/withdrawals.", account.Number));

                    if (account.OpenOrdersForAccount.NewCollection(x => x.Side == Side.Sell).Count > 0)
                        throw new ApplicationException(string.Format("It is not possible to do a buy model for account {0} since ther are sell orders.", account.Number));

                    if (account.OpenOrdersForAccount.Count > 0)
                    {
                        if (diff.IsLessThanZero)
                            throw new ApplicationException(string.Format("It is not possible to do a buy model for account {0} since the cash difference is negative.", account.Number));
                        if (account.TotalCash - account.OpenOrderAmount() < bmi.CashTransfers.TotalTransferAmount)
                            throw new ApplicationException(string.Format("It is not possible to do a buy model for account {0} since the cash is already spent.", account.Number));
                    }
                    else
                    {
                        if (diff != null && diff.IsNotZero)
                        {
                            // When there is a cash difference (max 15% of the deposit) -> clear it away
                            if (includePrevCash)
                            {
                                if ((bmi.CashTransfers.TotalTransferAmount + diff).IsLessThanZero)
                                    throw new ApplicationException(string.Format("It is not possible to do a buy model for account {0} since there is not enough cash.", account.Number));
                                bmi.DepositCashPositionDifference = diff;
                            }
                        }
                    }
                }

                ICurrency underlying = account.AccountOwner.StichtingDetails.BaseCurrency;
                InstructionEngineParameters engineParams = InstructionEngineParametersMapper.GetParameters(session);
                InstructionEngine engine = new InstructionEngine(engineParams);

                if (engine.ProcessInstruction(instruction))
                {
                    session.BeginTransaction();
                    AccountMapper.Update(session, account);
                    if (instruction.UpdateableOrders != null && instruction.UpdateableOrders.Count > 0)
                        OrderMapper.Insert(session, instruction.UpdateableOrders);
                    success = session.CommitTransaction();
                }
            }
            session.Close();
            return success;
        }

        public static bool SkipTransferForRebalance(int lineID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IJournalEntryLine transfer = JournalEntryMapper.GetJournalEntryLine(session, lineID);
                transfer.SkipOrders = true;
                session.Update(transfer);
                return true;
            }
        }

        public static bool GetAccountDetails(int id, out string number, out string shortName)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAccount account = AccountMapper.GetAccount(session, id);
            number = account.Number;
            shortName = account.ShortName;
            session.Close();
            return true;
        }

        public static DataSet GetNewAccounts()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                Hashtable parameters = new Hashtable(1);
                parameters.Add("company", company);
                IList<IAccountTypeCustomer> list = session.GetTypedListByNamedQuery<IAccountTypeCustomer>(
                    "B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.AccountsFirstDeposit", parameters);

                // Key, Number, ShortName, ModelPortfolioName, TotalCashAmount, TradeableStatus, Status
                return list
                .Select(c => new
                {
                    c.Key,
                    c.Number,
                    c.ShortName,
                    c.ModelPortfolioName,
                    c.TotalCashAmount,
                    c.TradeableStatus,
                    c.Status
                })
                .ToDataSet();
            }
        }

        public static DataSet GetAccountsNewCashTransfers()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            Hashtable parameters = new Hashtable(4);
            parameters.Add("company", company);
            parameters.Add("baseCurrency", company.BaseCurrency.Key);
            parameters.Add("statusBooked", JournalEntryStati.Booked);
            parameters.Add("accountType", AccountTypes.Customer);

            IList list = session.GetListByNamedQuery(
                "B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.AccountsNewCashTransfers", 
                parameters);
            if (list != null && list.Count > 0)
            {
                int[] accountIds = (from a in list.Cast<object[]>()
                            select (int)a[0]).ToArray();
                Hashtable parameterLists = new Hashtable(1);
                parameterLists.Add("accountIds", accountIds);
                IList<ICustomerAccount> accounts = session.GetTypedListByNamedQuery<ICustomerAccount>("B4F.TotalGiro.ApplicationLayer.Orders.AssetManager.AccountsNewCashTransfersRebalanceInfo", new Hashtable(), parameterLists);

                //"Key, Number, ShortName, IsExecOnlyCustomer, TradeableStatus, Status, ModelPortfolioName, CashPosition, TotalCashTransfers, Transactions"
                ds = list.Cast<object[]>()
                      .Select(a => new
                      {
                          Key = (int)a[0],
                          Number = (string)a[1],
                          ShortName = (string)a[2],
                          IsExecOnlyCustomer = (bool)a[3],
                          TradeableStatus = (Tradeability)a[4],
                          Status = (AccountStati)a[5],
                          ModelPortfolioName = (string)a[6],
                          CashPosition = (decimal)a[7],
                          TotalCashTransfers = (decimal)a[8],
                          Transactions = (long)a[9],
                          // CashPosition - TotalCashTransfers
                          DepositCashPositionDiff = (decimal)a[7] - (decimal)a[8],
                          IsDiffWithinTolerance = (bool)(Math.Abs((decimal)a[7] -(decimal)a[8]) < 5M)
                      })
                      .ToDataSet();



                ds.Tables[0].Columns.Add("BuyModelMessage", typeof(string));
                ds.Tables[0].Columns.Add("IsBuyModelAllowed", typeof(bool));
                ds.Tables[0].Columns.Add("ShowBuyModelDiffWarning", typeof(bool));
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int accountId = (int)row[0];
                    ICustomerAccount account = accounts.Where(u => u.Key == accountId).FirstOrDefault();
                    if (account != null)
                    {
                        int instructionCount = account.ActiveRebalanceInstructions.Count;
                        int moneyTransferCount = account.ActiveMoneyTransferOrders.Count;
                        int orderCount = account.OpenOrdersForAccount.Count;
                        decimal depositCashPositionDiff = (decimal)row["DepositCashPositionDiff"];
                        row["BuyModelMessage"] = string.Format("Rebalance Instructions:{0}<br/>Money Transfers: {1}<br/>Orders: {2}<br/>Deposit Cash Difference: {3}",
                            instructionCount, moneyTransferCount, orderCount, depositCashPositionDiff.ToString("0.00"));

                        bool isBuyModelAllowed = false;
                        bool showBuyModelDiffWarning = false;
                        decimal cashPosition = (decimal)row["CashPosition"];
                        decimal totalCashTransfers = (decimal)row["TotalCashTransfers"];
                        if (cashPosition > 0 && instructionCount == 0 && moneyTransferCount == 0 && account.OpenOrdersForAccount.NewCollection(x => x.Side == Side.Sell).Count == 0)
                        {
                            if (orderCount > 0)
                            {
                                Money orderAmount = account.OpenOrdersForAccount.TotalGrossAmount();
                                if (depositCashPositionDiff >= 0M && ((cashPosition - orderAmount.Quantity) >= totalCashTransfers))
                                {
                                    isBuyModelAllowed = true;
                                    row["DepositCashPositionDiff"] = 0M;
                                }
                            }
                            else
                            {
                                isBuyModelAllowed = true;
                                if (depositCashPositionDiff > 0M)
                                    showBuyModelDiffWarning = true;
                            }
                        }
                        row["IsBuyModelAllowed"] = isBuyModelAllowed;
                        row["ShowBuyModelDiffWarning"] = showBuyModelDiffWarning;
                    }
                }

                //ds.Tables[0].Columns.Add("IsBuyModelAllowed", typeof(bool), "IsDiffWithinTolerance and InstructionCount = 0 and MoneyTransferCount = 0 and OrderCount = 0");
                //ds.Tables[0].Columns.Add("IsBuyModelAllowed", typeof(bool), "CashPosition > 0 and DepositCashPositionDiff >= 0 and (CashPosition - OrderAmount) >= TotalCashTransfers and InstructionCount = 0 and MoneyTransferCount = 0 and SellOrderCount = 0");
            
            }
            session.Close();
            return ds;
        }

        public static DataSet GetNewCashTransfers(int accountID)
        {
            //@"Key, AccountA.Number, CashTransferType, ValueSize.DisplayString, TransactionDate, Description, CreationDate, CreatedBy"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountID);
                return JournalEntryMapper.GetUnProcessedCashTransfers(session, account)
                .Select(c => new
                {
                    c.Key,
                    c.GiroAccount.Number,
                    CashTransferType =
                        c.GLAccount.Description,
                    CreditDisplayString =
                        c.Balance.Negate().DisplayString,
                    c.Parent.TransactionDate,
                    c.Description,
                    c.CreationDate,
                    c.CreatedBy
                })
                .ToDataSet();
            }
        }
    }
}
