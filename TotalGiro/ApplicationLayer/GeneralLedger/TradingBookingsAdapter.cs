using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal;
using System.Linq;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class TradingBookingsAdapter
    {
        public static DataSet GetTradingBookings(    int journalId, DateTime transactionDateFrom, DateTime transactionDateTo, string journalEntryNumber, JournalEntryStati statuses)
        {
            //@"Key, JournalEntryNumber, Status, DisplayStatus, Journal.JournalNumber, TransactionDate"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IJournal journal = JournalMapper.GetJournal(session, journalId);
                return JournalEntryMapper.GetJournalEntries<ITradingJournalEntry>(session, JournalTypes.ClientTransaction, journal, 
                    transactionDateFrom, transactionDateTo, journalEntryNumber, statuses, true)
                    .Select(c => new
                    {
                        c.Key,
                        c.JournalEntryNumber, 
                        c.Status, 
                        c.DisplayStatus,
                        Journal_JournalNumber =
                            c.Journal.JournalNumber,
                        c.TransactionDate
                    })
                    .ToDataSet();
            }
        }
    }
}
