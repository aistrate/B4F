using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using System.Collections;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.TGTransactions
{
    public static class TransactionAdapter
    {
        public static ITransaction GetTransaction(int tradeid)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ITransaction returnValue = TransactionMapper.GetTransaction(session, tradeid);
            return returnValue;

        }

        public static void SetTransactionDescriptions()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList<int> txs = TransactionMapper.GetUnmigratedTransactions(session);
            int counter = 0;

            foreach (int tx in txs)
            {
                ITransaction tran = TransactionMapper.GetTransaction(session, tx);
                tran.Migrate();
                TransactionMapper.Update(session, tran);
                if (counter++ == 1000)
                {
                    session.Close();
                    session = NHSessionFactory.CreateSession();
                }
            }

            session.Close();
        }


        //public static bool ApproveExecution(int tradeID)
        //{
        //    IDalSession session = NHSessionFactory.CreateSession();
        //    IInternalEmployeeLogin employee = LoginMapper.GetCurrentEmployee(session);
        //    IOrderExecution exec = (IOrderExecution)TransactionMapper.GetTransaction(session, tradeID);
        //    IList childrenOfExecution = B4F.TotalGiro.Orders.OldTransactions.ObsoleteTransactionMapper.GetNewExecutionChildren(session, tradeID);
        //    exec.Approve(employee);
        //    if ((exec.AccountA.AccountType == AccountTypes.Customer) || (exec.AccountB.AccountType == AccountTypes.Customer))
        //    {
        //        //ITradingJournalEntry execSetlementJournal = TransactionAdapter.GetNewTradingJournalEntry(session, exec.TradingJournalEntry, exec.TransactionDate);
        //        exec.ClientSettle(exec.TradingJournalEntry);
        //    }
        //    TransactionMapper.Update(session, exec);
        //    session.Close();

        //    foreach (int childID in childrenOfExecution)
        //    {
        //        session = NHSessionFactory.CreateSession();
        //        IOrderExecutionChild child = (IOrderExecutionChild)TransactionMapper.GetTransaction(session, childID);
        //        child.Approve(employee);

        //        //ITradingJournalEntry childSetlementJournal = TransactionAdapter.GetNewTradingJournalEntry(session, child.TradingJournalEntry, child.TransactionDate);
        //        child.ClientSettle(child.TradingJournalEntry);
        //        TransactionMapper.Update(session, child);
        //        session.Close();
        //    }

        //    //Check the flag Isallocated
        //    session = NHSessionFactory.CreateSession();
        //    exec = (IOrderExecution)TransactionMapper.GetTransaction(session, tradeID);
        //    exec.SetIsAllocated();
        //    TransactionMapper.Update(session, exec);
        //    session.Close();

        //    return true;
        //}

        //public static bool ClientSettleExecution(int executionTradeID)
        //{
        //    IDalSession session = NHSessionFactory.CreateSession();
        //    IOrderExecution exec = (IOrderExecution)TransactionMapper.GetTransaction(session, executionTradeID);
        //    ITradingJournalEntry newEntry = GetNewTradingJournalEntry(session, exec.TradingJournalEntry, exec.TransactionDate);
        //    exec.ClientSettle(newEntry);
        //    TransactionMapper.Update(session, exec);
        //    session.Close();
        //    return true;
        //}

        public static ITradingJournalEntry GetNewTradingJournalEntry(IDalSession session, ITradingJournalEntry unsettledJournal, DateTime transactionDate)
        {
            return GetNewTradingJournalEntry(session, unsettledJournal.Journal.Currency.Symbol.ToUpper(), transactionDate);
        }

        public static ITradingJournalEntry GetNewTradingJournalEntry(IDalSession session, string currency, DateTime transactionDate)
        {
            int journalID;
            string appSettingKey = string.Format("DefaultTransacties{0}", (string.IsNullOrEmpty(currency) ? "EUR" : currency.ToUpper()));
            journalID = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get(appSettingKey)));

            IJournal journal = JournalMapper.GetJournal(session, journalID);
            string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);
            //decimal exRate = 1M;
            //if (!journal.Currency.IsBase)
            //{
            //    IHistoricalExRate historicalExRate = HistoricalExRateMapper.GetHistoricalExRate(session, journal.Currency, transactionDate);
            //    if (historicalExRate == null)
            //        throw new ApplicationException(string.Format("No exchange rate can be found");

            //    exRate = historicalExRate.Rate;
            //}
            ITradingJournalEntry entry = new TradingJournalEntry(journal, nextJournalEntryNumber, transactionDate);
            return entry;
        }

    }
}
