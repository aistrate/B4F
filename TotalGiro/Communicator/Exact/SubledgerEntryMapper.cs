using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class SubledgerEntryMapper
    {
        public static IList<ExactEntryGrouping> GetJournalEntryGroupsToExport(IDalSession session, DateTime dateUntil)
        {
            string query = "B4F.TotalGiro.Communicator.Exact.SubledgerEntry.GetJournalEntryGroupsToExport";

            Hashtable parameters = new Hashtable();
            parameters.Add("transactionDate", dateUntil);

            IList<ExactEntryGrouping> groups = new List<ExactEntryGrouping>();
            foreach (object[] entry in session.GetListByNamedQuery(query, parameters))
            {
                groups.Add(new ExactEntryGrouping(entry));
            }

            return groups;
        }


        public static IList<IJournalEntryLine> GetJournalEntriesToExport(IDalSession session, ExactEntryGrouping grouping)
        {
            string query = "B4F.TotalGiro.Communicator.Exact.SubledgerEntry.GetJournalEntriesToExport";

            Hashtable parameters = new Hashtable();
            parameters.Add("exactJournalID", grouping.Key);
            parameters.Add("transactionDate", grouping.TransactionDate);

            IList<IJournalEntryLine> lines = session.GetTypedListByNamedQuery<IJournalEntryLine>(query, parameters);
            return lines;
        }



    }
}
