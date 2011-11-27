using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using System.Web;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.ApplicationLayer.Orders
{
    public static class SingleOrderAdapter
    {
        public static DataSet GetFilteredAccounts()
        {
            string accountFilter = (string)HttpContext.Current.Session["dsFilteredAccounts"];
            
            return GetFilteredAccounts_Inner(accountFilter);
        }

        public static DataSet GetFilteredAccounts_Inner(string accountFilter)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                AccountMapper.GetFilteredAccounts(session, AccountTypes.Customer, accountFilter), "Key, Number");

            session.Close();

            Utility.AddEmptyFirstRow(ds.Tables[0], "Key", "Number");
            return ds;
        }

        public static DataSet GetFilteredTradeableInstruments()
        {
            string instrumentFilter = (string)HttpContext.Current.Session["dsFilteredInstruments"];

            return GetFilteredTradeableInstruments_Inner(instrumentFilter);
        }

        public static DataSet GetFilteredTradeableInstruments_Inner(string instrumentFilter)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                InstrumentMapper.GetFilteredTradeableInstruments(session, instrumentFilter), "Key, Name, Isin");

            session.Close();

            Utility.AddEmptyFirstRow(ds.Tables[0], "Key", "Name");
            return ds;
        }

        public static void PlaceOrder(int accountId, int instrumentId, bool isAmountBased, bool isSell, decimal amount, decimal size)
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
                    Money theMoney = new Money(qty, ((ITradeableInstrument)theInstrument).CurrencyNominal);
                    order = new OrderAmountBased(bankAccount, theMoney, theInstrument, true, fee);
                }
                else
                {
                    Decimal qty = (isSell ? (size * -1) : size);
                    InstrumentSize theSize = new InstrumentSize(qty, theInstrument);
                    order = new OrderSizeBased(bankAccount, theSize, true, fee);
                }

                OrderMapper.Insert(session, order);
            }
            finally
            {
                session.Close();
            }
        }

        public static string GetCurrencyFromInstrument(int instrumentId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            
            IInstrument theInstrument = InstrumentMapper.GetInstrument(session, instrumentId);
            string currency = ((ITradeableInstrument)theInstrument).CurrencyNominal.ToString();
            
            session.Close();

            return currency;
        }
    }
}
