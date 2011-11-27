using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IBondCouponPaymentDailyCalculation
    {
        int Key { get; }
        IBondCouponPayment Parent { get; set; }
        IFundPosition Position { get; }
        DateTime CalcDate { get; }
        DateTime SettlementDate { get; }
        InstrumentSize PositionSize { get; set;  }
        Money CalculatedAccruedInterestUpToDate { get; set;  }
        Money DailyInterest { get; }
        IList<IBondCouponPaymentDailyCalculation> OldCalculations { get; }

        void CalculateDailyInterest();
        void Edit(InstrumentSize positionSize, Money calculatedAccruedInterestUpToDate, IList<IBondCouponPaymentDailyCalculation> oldCalculations);
    }
}
