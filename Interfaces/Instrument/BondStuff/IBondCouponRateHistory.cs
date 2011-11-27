using System;

namespace B4F.TotalGiro.Instruments
{
    public interface IBondCouponRateHistory
    {
        int Key { get; }
        IBond Parent { get; set; }
        DateTime StartDate { get; set; }
        decimal CouponRate { get; set; }
        DateTime EndDate { get; set; }
        DateTime CreationDate { get; }
        string CreatedBy { get;}
        DateTime LastUpdated { get; }
        string LastUpdatedBy { get; set; }

        int GetRelevantDaysInPeriod(DateTime startDate, DateTime endDate);
    }
}
