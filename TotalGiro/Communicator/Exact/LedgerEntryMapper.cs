using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Dal;
using System.Data;
using NHibernate.Criterion;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public static class LedgerEntryMapper
    {
        public static IList<ILedgerType> GetLedgerEntryGroupings(IDalSession session, DateTime dateUntil)
        {
            string query = "B4F.TotalGiro.Communicator.Exact.LedgerEntry.GetLedgerEntryGroupings";

            Hashtable parameters = new Hashtable();
            parameters.Add("dateUntil", dateUntil);

            return session.GetTypedListByNamedQuery<ILedgerType>(query, parameters);
        }

        public static IList GetLedgerEntries(IDalSession session, ILedgerType ledgerType, DateTime dateUntil)
        {
            string query = "B4F.TotalGiro.Communicator.Exact.LedgerEntry.GetLedgerEntries";
            
            Hashtable parameters = new Hashtable();
            parameters.Add("LedgerType", ledgerType);
            parameters.Add("DateUntil", dateUntil);

            return session.GetListByNamedQuery(query, parameters);
        }

        public static ILedgerType GetLedgerType(IDalSession session, string name)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Type", name));
            IList result = session.GetList(typeof(LedgerType), expressions);
            if ((result != null) && (result.Count > 0))
                return (ILedgerType)result[0];
            else
                return null;
        }

        public static ILedgerType GetLedgerType(IDalSession session, JournalTypes journalType)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("JournalType", journalType));
            IList result = session.GetList(typeof(LedgerType), expressions);
            if ((result != null) && (result.Count > 0))
                return (ILedgerType)result[0];
            else
                return null;
        }

        public static string GetNextLedgerEntryNumber(IDalSession session, string exactLedger)
        {
            string sqlQuery = "SELECT TOP 1 {E.*} FROM LEDGER E WHERE E.JOURNAL = '" + exactLedger + "' order by E.BookingNumber DESC";
            IList ledgerEntries = session.GetListbySQLQuery(sqlQuery, "E", typeof(LedgerEntry));

            if (ledgerEntries.Count > 0)
                return (int.Parse(((ILedgerEntry)ledgerEntries[0]).BookingNumber) + 1).ToString().PadLeft(8, '0');
            else
                return (("8" + exactLedger.PadLeft(3, '0')).PadRight(7, '0')) + "1";
        }

        public static IList<ILedgerEntry> GetLedgerEntriesinFile(IDalSession session, int fileID)
        {
            string query = @"FROM LedgerEntry L
                             WHERE L.ExportedLedgerFile.Key = :fileID
                             ORDER BY L.BookingNumber";

            Hashtable parameters = new Hashtable();
            parameters.Add("fileID", fileID);

            return session.GetTypedListByNamedQuery<ILedgerEntry>(query, parameters);
        }


        public static bool Update(IDalSession session, ILedgerEntry obj)
        {
            return session.InsertOrUpdate(obj);
        }
    }
}
