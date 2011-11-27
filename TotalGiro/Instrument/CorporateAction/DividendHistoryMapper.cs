using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Linq;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using System.Collections;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public static class DividendHistoryMapper
    {
        public static IList<IDividendHistory> GetDividendHistoryList(IDalSession session, int instrumentKey, DateTime startdate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable();
            if (instrumentKey != 0 && instrumentKey != int.MinValue)
                parameters.Add("instrumentKey", instrumentKey);
            if (Util.IsNotNullDate(startdate))
                parameters.Add("startdate", startdate);
            if (Util.IsNotNullDate(endDate))
                parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<IDividendHistory>(
                "B4F.TotalGiro.Instruments.CorporateAction.GetDividendHistories",
                parameters);
        }

        public static IDividendHistory GetDividendHistory(IDalSession session, int detailsKey)
        {
            var step1 =  session.Session.Linq<DividendHistory>()
                                .Where(dh => dh.Key == detailsKey)
                                .AsQueryable();

            if (step1.Count() == 1)
                return step1.Cast<IDividendHistory>()
                            .First();
            else
                return null;
        }

        public static IList<ICorporateActionStockDividend> GetStockDividendDetails(IDalSession session, int detailsKey)
        {
            IList<ICorporateActionStockDividend> list = null;
            if (detailsKey != 0)
            {
                IDividendHistory history = GetDividendHistory(session, detailsKey);
                list = history.Get(e => e.StockDividends);
            }
            return list;
        }

        public static IList<ICashDividend> GetCashDividendDetails(IDalSession session, int detailsKey)
        {
            IList<ICashDividend> list = null;
            if (detailsKey != 0)
            {
                IDividendHistory history = GetDividendHistory(session, detailsKey);
                list = history.Get(e => e.CashDividends);
            }
            return list;
        }
    }
}
