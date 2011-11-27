using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.MIS.Positions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.MIS.StoredPositions
{
    public static class StoredPositionTransactionMapper
    {
        public static IList<IHistoricalPosition> GetAccountsWithPositionByDate(IDalSession session, DateTime reportDate, int instrumentKey)
        
        {
            Hashtable parameters = new Hashtable(2);

            parameters.Add("reportDate", reportDate);
            parameters.Add("instrumentKey", instrumentKey);

            IInstrumentsWithPrices instrument = InstrumentMapper.GetInstrumentWithPrice(session, instrumentKey);

            IList<IAccountTypeInternal> listOfAccounts = session.GetTypedListByNamedQuery<IAccountTypeInternal>(@"B4F.TotalGiro.MIS.StoredPositions.StoredPositionTransaction.GetAccountsWithPosition"
                                    , parameters);
            IList<IHistoricalPosition> stuff = new List<IHistoricalPosition>();
            if ((listOfAccounts != null) && (listOfAccounts.Count > 0))
            {
                foreach (IAccountTypeInternal acc in listOfAccounts)
                {
                    parameters = new Hashtable(3);
                    parameters.Add("reportDate", reportDate);
                    parameters.Add("instrumentKey", instrumentKey);
                    parameters.Add("accountid", acc.Key);
                    decimal positionTotal = (decimal)(session.GetListByNamedQuery("B4F.TotalGiro.MIS.StoredPositions.StoredPositionTransaction.GetPositionValueForAccount", parameters)[0]);
                    stuff.Add(new HistoricalPosition(acc.Key, instrument, positionTotal, reportDate, acc));
                }

            }

            return stuff;

        }

    }
}
