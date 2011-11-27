using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Linq;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public static class CorporateActionHistoryMapper
    {
        public static IList<ICorporateActionBonusDistribution> GetBonusDistributionList(IDalSession session, int instrumentKey, DateTime startdate, DateTime endDate)
        {
            var stage1 = session.Session.Linq<CorporateActionBonusDistribution>().AsQueryable();

            if (instrumentKey != 0)
                stage1 = stage1.Where(dh => dh.Instrument.Key == instrumentKey);

            if (startdate != null)
                stage1 = stage1.Where(dh => dh.DistributionDate >= startdate);

            if (endDate != null)
                stage1 = stage1.Where(dh => dh.DistributionDate <= endDate);

            return stage1.Cast<ICorporateActionBonusDistribution>()
                        .Select(dh => dh)
                        .ToList();
        }
    }
}
