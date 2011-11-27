using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class CashTransactionPageAdapter
    {
        public static DataSet GetCurrencies()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = InstrumentMapper.GetCurrencies(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Name
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }

        public static bool GetBankAccountName(string bankAccountId, out string bankAccountName, out string cashBalance)
        {
            bankAccountName = "";
            cashBalance = "";

            IDalSession session = NHSessionFactory.CreateSession();
            IAccountTypeInternal bankAccount = (IAccountTypeInternal)AccountMapper.GetAccountByNumber(session, bankAccountId);

            if (bankAccount != null)
            {
                bankAccountName = bankAccount.ShortName;
                cashBalance = bankAccount.TotalCashAmount.DisplayString;
            }
            
            session.Close();

            return true;
        }


        private static IList<int> getTRansactions(IDalSession session)
        {
            string hqlstring = @"Select T.Key from Transaction T where T.Description = ''";

            return session.GetTypedListByHQL<int>(hqlstring);
        }

        public static bool SetDescriptionsOnTransactions()
        {
            IDalSession session = NHSessionFactory.CreateSession();

                IList<int> theList = getTRansactions(session);
            
            session.Close();

            foreach (int i in theList)
                setDescriptionsOnTransactions(i);

            return true;
        }

        private static bool setDescriptionsOnTransactions(int tradeid)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                ITransaction trade = TransactionMapper.GetTransaction(session, tradeid);
                string test = trade.Description;
                session.Update(trade);
                return true;
            }

            catch (Exception e)
            {
                return false;
            }
            finally
            {
                session.Close();    

            }

        }




        public static void CreateCashTransaction(string bankAccountId, decimal amount, int currencyId, DateTime txDate, string description, bool doNotDisplay, bool isInternalBooking)
        {                                                                                                               
            IDalSession dataSession = NHSessionFactory.CreateSession();

            try
            {
                decimal exRate = 0M;
                IAccountTypeInternal acctA = (IAccountTypeInternal)AccountMapper.GetAccountByNumber(dataSession, bankAccountId);

                ICurrency cur = InstrumentMapper.GetCurrency(dataSession, currencyId);
                Money cashAmount = new Money(amount, cur);

                if (cur.IsBase)
                    exRate = 1M;
                else
                {
                    IHistoricalExRate rate = HistoricalExRateMapper.GetHistoricalExRate(dataSession, (ICurrency)cashAmount.Underlying, txDate);
                    if (rate == null)
                        throw new ApplicationException(string.Format("The exchange rate for {0} on {1} is not available.", cashAmount.Underlying.Name, txDate.ToShortDateString()));
                    exRate = rate.Rate;
                }

                //IObsoleteCashTransfer theCash = new ObsoleteCashTransfer(acctA, cashAmount, txDate, exRate, description, true, string.Empty);
                //theCash.DoNotDisplay = doNotDisplay;
                //theCash.IsNoDepositWithDrawal = isInternalBooking;
                //((IObsoleteTransaction)theCash).Approve();

                //ObsoleteTransactionMapper.Update(dataSession, theCash);
            }
            finally
            {
                dataSession.Close();
            }
        }
    }
}
