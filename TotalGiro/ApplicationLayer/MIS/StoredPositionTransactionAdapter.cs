using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.MIS.StoredPositions;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.MIS.Positions;

namespace B4F.TotalGiro.ApplicationLayer.MIS
{
    public static class StoredPositionTransactionAdapter
    {
        public static IList<IHistoricalPosition> test(DateTime reportDate, int instrumentKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return StoredPositionTransactionMapper.GetAccountsWithPositionByDate(session, reportDate, instrumentKey);
            }
        }

    }
}
