using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class JournalEntryFinderAdapter
    {
        public static DataSet GetJournals(JournalTypes journalType)
        {
            DataSet ds = null;
            // "Key, FullDescription"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ds = JournalMapper.GetJournals(session, journalType)
                    .Select(c => new
                    {
                        c.Key,
                        c.FullDescription
                    })
                    .ToDataSet();
            }
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }
    }
}
