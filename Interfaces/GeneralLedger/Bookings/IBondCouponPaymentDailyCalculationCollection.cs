using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IBondCouponPaymentDailyCalculationCollection : IList<IBondCouponPaymentDailyCalculation>
    {
        IBondCouponPayment Parent { get; set; }
        Money TotalAccruedInterest { get; }
        IBondCouponPaymentDailyCalculation LastCalculation { get; }

        void AddCalculation(DateTime calcDate, DateTime settlementDate, InstrumentSize positionSize, Money calculatedAccruedInterestUpToDate, IList<IBondCouponPaymentDailyCalculation> oldCalculations);
        IBondCouponPaymentDailyCalculation GetCalculationByDate(DateTime calcDate);
    }
}
