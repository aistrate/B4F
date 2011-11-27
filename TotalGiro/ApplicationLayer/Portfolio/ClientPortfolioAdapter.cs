using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class ClientPortfolioAdapter
    {
        public static AccountDetailsView GetAccountDetails(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
                ICurrency baseCurrency = LoginMapper.GetCurrentManagmentCompany(session).BaseCurrency;

                return GetAccountDetails(account, baseCurrency);
            }
        }

        public static AccountDetailsView GetAccountDetails(IAccountTypeCustomer account, ICurrency baseCurrency)
        {
            return account != null ? new AccountDetailsView(account) : new AccountDetailsView(baseCurrency);
        }


        public static bool HasCashPosition(int accountId)
        {
            bool returnValue = false;
            IDalSession session = NHSessionFactory.CreateSession();
            IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
            if (account.Portfolio.PortfolioCashGL.SettledCashTotalInBaseValue.IsNotZero)
                returnValue = true;

            session.Close();

            return returnValue;
        }

        public static DataSet GetPositions(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);

                return GetPositions(session, account);
            }
        }

        public static DataSet GetPositions(IDalSession session, IAccountTypeCustomer account)
        {
            if (account != null)
            {
                List<PositionRowView> positionRowViews = FundPositionMapper.GetPositions(session, account, PositionsView.NotZero)
                                                                           .Select(p => new PositionRowView(p))
                                                                           .ToList();

                decimal totalValue = account.TotalPositionAmount(PositionAmountReturnValue.All).Quantity;
                if (totalValue != 0m)
                    foreach (PositionRowView rowView in positionRowViews.Where(pv => pv.Value != null))
                        rowView.Percentage = Math.Round(100m * rowView.Value.Quantity / totalValue, 2);

                return positionRowViews.ToDataSet();
            }
            else
                return (new List<PositionRowView>()).ToDataSet();
        }

        public static string[] ClosePositions(int[] positionIds, bool ignoreWarnings, bool noCharges)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ArrayList warningMessages = new ArrayList();
            string instrumentName = "";
            string accountName = "";

            try
            {
                IList<IFundPosition> positions = AccountMapper.GetFundPositions(session, positionIds);
                FeeFactory feeFactory = FeeFactory.GetInstance(session);

                ArrayList orders = new ArrayList();
                foreach (IFundPosition position in positions)
                {
                    instrumentName = string.Format("{0} ({1})", position.Instrument.Name, ((ITradeableInstrument)position.Instrument).Isin);
                    accountName = string.Format("{0} ({1})", position.Account.ShortName, position.Account.Number);

                    if (!AccountMapper.AccountStatusIsOpen(session, position.Account.Status))
                        throw new ApplicationException("Position cannot be closed for this account because the account's status is closed.");

                    Order order = new OrderSizeBased(position.Account, position.Size * (-1), true, feeFactory, noCharges);
                    OrderValidationResult validationResult = order.Validate();

                    if (validationResult.MainType == OrderValidationType.Success ||
                       (validationResult.MainType == OrderValidationType.Warning && ignoreWarnings))
                        orders.Add(order);
                    else if (validationResult.MainType == OrderValidationType.Warning)
                        warningMessages.Add(validationResult.Message);
                    else if (validationResult.MainType == OrderValidationType.Invalid)
                        throw new ApplicationException(validationResult.Message);
                }

                if (orders.Count > 0 && warningMessages.Count == 0)
                    OrderMapper.Insert(session, orders);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("{0} [Instrument: {1}, Account: {2}]", ex.Message, instrumentName, accountName));
            }
            finally
            {
                session.Close();
            }

            string[] messagesArray = new string[warningMessages.Count];
            warningMessages.CopyTo(messagesArray);
            return messagesArray;
        }

        public static bool RebalanceAccount(int accountId, bool noCharges, out string message)
        {
            bool retVal = false;
            message = "";

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);

                if (!AccountMapper.AccountStatusIsOpen(session, account.Status))
                    throw new ApplicationException("Account cannot be rebalanced because its status is closed.");

                if (account.CreateInstruction(InstructionTypes.Rebalance, OrderActionTypes.Rebalance, DateTime.Now.Date, noCharges) != null)
                {
                    AccountMapper.Update(session, account);
                    message = string.Format("A Rebalance instruction for account {0} was created successfully.", account.DisplayNumberWithName);
                    retVal = true;
                }
                return retVal;
            }
        }

        public static bool LiquidateAccount(int accountId, bool noCharges, out string message)
        {
            bool retVal = false;
            message = "";

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);

                if (!AccountMapper.AccountStatusIsOpen(session, account.Status))
                    throw new ApplicationException("Account cannot be liquidated because its status is closed.");

                if (account.CounterAccount == null)
                    throw new ApplicationException(string.Format("The account {0} does not have a default counter account.", account.DisplayNumberWithName));

                if (account.CreateDepartureInstruction(DateTime.Now.Date, null, "", noCharges) != null)
                {
                    AccountMapper.Update(session, account);
                    message = string.Format("A client departure instruction for account {0} was created successfully.", account.DisplayNumberWithName);
                    retVal = true;
                }
                return retVal;
            }
        }

        public static bool AggregatePOSOrders(out string errorMessage)
        {
            string message;
            errorMessage = "";

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IList<IOrder> unPOSAggregatedUnApprovedOrders = OrderMapper.GetPOSUnaggregatedOrders(session);

                if (unPOSAggregatedUnApprovedOrders != null && unPOSAggregatedUnApprovedOrders.Count > 0)
                {
                    foreach (IOrder order in unPOSAggregatedUnApprovedOrders) order.Approve();

                    IList<IOrder> aggregateOrders = null;

                    IList<IOrder> orders = Order.AggregateOrders(aggregateOrders, unPOSAggregatedUnApprovedOrders, out message);

                    foreach (IOrder order in orders) order.Approve();

                    if (orders != null)
                        OrderMapper.SaveAggregatedOrders(session, orders);
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            finally
            {
                session.Close();
            }
        }

        public static DataSet GetBuyingPowerDisplayForClient(int accountId, bool showForeignCurrency, bool showUnSettledCash)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeInternal acc = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);

                BuyingPowerDisplayCollection collection = BuyingPowerDisplayMapper.GetBuyingPowerDisplay(session, acc, showForeignCurrency, showUnSettledCash);
                //BuyingPowerDisplay[] list = new BuyingPowerDisplay[collection.Count];
                //collection.ToList().CopyTo(list,0);

                //"Key, PositionID, LineDescription, Status, Value, Value.Quantity, ExRate, BaseValueSettled.Quantity, BaseValueAll.Quantity, IsSubTotalLine, ValueDisplay, BaseValueSettledDisplay, BaseValueAllDisplay, IsCashFundLine, IsSummaryLine"
                return collection
                    .Select(c => new
                    {
                        c.Key,
                        c.SubPositionID,
                        c.PositionID,
                        c.LineDescription,
                        c.Status,
                        c.Value,
                        Value_Quantity = c.Value.Quantity,
                        c.ExRate,
                        BaseValue_Quantity = c.BaseValue.Quantity,
                        c.IsSubTotalLine,
                        c.ValueDisplay,
                        c.BaseValueDisplay,
                        c.IsCashFundLine,
                        c.IsSummaryLine,
                        c.IsCashLine
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber,
                                string accountName, bool showActive, bool showInactive)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = AccountMapper.GetCustomerAccounts(session, assetManagerId, 0, 0, 0, modelPortfolioId, accountNumber, accountName, false,
                        true, showActive, showInactive, 0, true, true)
                        .Select(c => new
                        {
                            c.Key,
                            c.Number,
                            c.DisplayNumberWithName
                        })
                        .ToDataSet();

                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }
    }
}
