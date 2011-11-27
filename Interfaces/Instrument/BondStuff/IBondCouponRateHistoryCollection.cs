using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments
{
    public interface IBondCouponRateHistoryCollection : IList<IBondCouponRateHistory>
    {
        IBond Parent { get; set; }
        IBondCouponRateHistory LastCouponRate { get; }
        IBondCouponRateHistory GetCouponRateByDate(DateTime date);
        void AddCouponRate(IBondCouponRateHistory item);
        bool RemoveCouponRate(IBondCouponRateHistory item);
        void CheckPeriodOverlap();
        decimal GetWeightedCouponRateForPeriod(DateTime startDate, DateTime endDate);
    }
}
