using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using System.Collections;
using B4F.TotalGiro.PortfolioComparer;

namespace B4F.TotalGiro.ApplicationLayer.Test
{
    public static class OrderCreationTestAdapter
    {
        public static DataSet GetOrders()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                OrderMapper.GetOrders<ISecurityOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.None, ApprovalState.All, 
                    SecurityInfoOptions.ManagedsAcctsOnly, ParentalState.All).ToArray(),
                    "Account.Number, Account.ShortName, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, Value, Value.DisplayString, " +
                    "CommissionInfo, Status, OrderID");

            session.Close();

            return ds;
        }

        public static DataSet GetAccounts()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetAccounts<ICustomerAccount>(session, AccountTypes.Customer,
                    LoginMapper.GetCurrentManagmentCompany(session))
                    .Select(c => new
                    {
                        c.Key,
                        c.ShortName,
                        c.Number
                    })
                    .ToDataSet();
            }
        }

        public static void GetTotalCashAmount(int accountId, out string cashPositionString, out string openOrderCashString)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAccountTypeInternal bankAccount = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);
            Money totalCashAmount = bankAccount.TotalPositionAmount(PositionAmountReturnValue.BothCash);
            
            cashPositionString = totalCashAmount.ToString();
            openOrderCashString = bankAccount.OpenOrderAmount().ToString();
            
            session.Close();
        }

        public static void GetCurrencies(int accountId, int instrumentId, out string baseCurrency, out string instrumentCurrency)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IAccountTypeCustomer bankAccount = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
            baseCurrency = bankAccount.AccountOwner.StichtingDetails.BaseCurrency.ToString();

            IInstrument theInstrument = InstrumentMapper.GetInstrument(session, instrumentId);
            instrumentCurrency = ((ITradeableInstrument)theInstrument).CurrencyNominal.ToString();

            session.Close();
        }

        //public static void BuyModelPortfolio(int accountId)
        //{
        //    IDalSession session = NHSessionFactory.CreateSession();

        //    IFeeFactory fee = FeeFactory.GetInstance(session);
        //    IAccountTypeCustomer bankAccount = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
        //    PortfolioComparer.PortfolioComparer pc = new PortfolioComparer.PortfolioComparer(bankAccount, fee);

        //    RebalanceResults result;
        //    IList orders = pc.CompareToModel(new PortfolioComparer.PortfolioCompareSetting(false, PortfolioCompareAction.Rebalance, null), out result);
        //    if (orders != null)
        //        session.Insert(orders);

        //    session.Close();
        //}

        public static string GetModelPortfolioDescription(int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAccountTypeCustomer bankAccount = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);

            string description = bankAccount.ModelPortfolio.ToString();
            
            session.Close();

            return description;
        }

        public static void PlaceOrder(int accountId, int instrumentId, string currencyName, 
                                      bool isAmountBased, bool isSell, bool isValueInclComm, decimal amount, decimal size)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IAccountTypeInternal bankAccount = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);
                IInstrument theInstrument = InstrumentMapper.GetInstrument(session, instrumentId);
                FeeFactory fee = FeeFactory.GetInstance(session);
                Order order;

                if (isAmountBased)
                {
                    Decimal qty = (isSell ? (amount * -1) : amount);
                    ICurrency currency = InstrumentMapper.GetCurrencyByName(session, currencyName);
                    if (currency == null)
                        currency = ((ITradeableInstrument)theInstrument).CurrencyNominal;
                    Money theMoney = new Money(qty, currency);
                    order = new OrderAmountBased(bankAccount, theMoney, theInstrument, isValueInclComm, fee, false);
                }
                else
                {
                    Decimal qty = (isSell ? (size * -1) : size);
                    InstrumentSize theSize = new InstrumentSize(qty, theInstrument);
                    order = new OrderSizeBased(bankAccount, theSize, true, fee, false);
                }

                //if (order.Validate())
                    OrderMapper.Insert(session, order);
            }
            finally
            {
                session.Close();
            }
        }

        public static int GetSelectedAccountID(string strSearchBankAcctNumber)
        {
            int accountID = 0;
            if (strSearchBankAcctNumber != string.Empty)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IAccount account = AccountMapper.GetAccountByNumber(session, strSearchBankAcctNumber);
                if (account != null)
                    accountID = account.Key;
            }
            return accountID;
        }
    }
}
