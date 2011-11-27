using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class BankStatementsAdapter
    {
        public static DateTime GetLastTransactionDate(int journalId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IJournal journal = JournalMapper.GetJournal(session, journalId);
                return JournalEntryMapper.GetLastJournalEntryDate(session, journal);
            }
            finally
            {
                session.Close();
            }
        }

        public static DataSet GetBankStatements(
            int journalId, DateTime transactionDateFrom, DateTime transactionDateTo, string journalEntryNumber, JournalEntryStati statuses)
        {
            //@"Key, JournalEntryNumber, Status, DisplayStatus, Journal.JournalNumber, TransactionDate, Debit, Credit, OpenAmount, 
            //  DisplayOpenAmount, ClosingBalance, HasClosingBalance"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IJournal journal = JournalMapper.GetJournal(session, journalId);
                return JournalEntryMapper.GetJournalEntries<IBankStatement>(session, JournalTypes.BankStatement, journal, 
                    transactionDateFrom, transactionDateTo, journalEntryNumber, statuses, true)
                    .Select(c => new
                    {
                        c.Key,
                        c.JournalEntryNumber,
                        c.Status,
                        c.DisplayStatus,
                        Journal_JournalNumber =
                            c.Journal.JournalNumber,
                        c.TransactionDate,
                        c.Debit,
                        c.Credit,
                        c.OpenAmount,
                        c.DisplayOpenAmount,
                        c.ClosingBalance,
                        c.HasClosingBalance
                    })
                    .ToDataSet();
            }
        }

        public static int CreateBankStatement(int journalId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IJournal journal = JournalMapper.GetJournal(session, journalId);
                IBankStatement lastBankStatement = (IBankStatement)JournalEntryMapper.GetLastJournalEntry(session, journal);

                if (lastBankStatement != null && lastBankStatement.Status == JournalEntryStati.New)
                    throw new ApplicationException(
                        string.Format("Please book the last Bank Statement of Journal '{0}' before creating a new one.", journal.JournalNumber));

                string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);
                decimal exRate = 1M;
                if (!journal.Currency.IsBase)
                    exRate = HistoricalExRateMapper.GetHistoricalExRate(session, journal.Currency, Util.DateAdd(DateInterval.Day, 1, lastBankStatement.TransactionDate, DateIntervalOptions.ExcludeWeekends, null)).Rate;

                IBankStatement newBankStatement = new BankStatement(lastBankStatement, journal, nextJournalEntryNumber, exRate);

                JournalEntryMapper.Insert(session, newBankStatement);

                return newBankStatement.Key;
            }
            finally
            {
                session.Close();
            }
        }
    }
}