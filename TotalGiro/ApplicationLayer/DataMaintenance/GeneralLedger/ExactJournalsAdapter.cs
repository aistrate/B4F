using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Communicator.Exact;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.GeneralLedger
{
    public static class ExactJournalsAdapter
    {
        public static DataSet GetJournals()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return ExactJournalMapper.GetExactJournals(session).Select(a => new
                {
                    a.Key,
                    a.JournalNumber,
                    a.LedgerType.TypeOfLedger
                }).ToDataSet();
            }
        }

    }
}
