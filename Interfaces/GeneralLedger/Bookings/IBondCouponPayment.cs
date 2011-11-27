using System;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments.CorporateAction;
using System.Collections.Generic;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public enum BondCouponPaymentStati
    {
        Active,
        ToBeSettled,
        Settled,
        Cancelled
    }
    
    public interface IBondCouponPayment: IGeneralOperationsBooking
    {
        IFundPosition Position { get; set; } 
        IBond Bond { get; } 
        ICouponHistory CouponHistory { get; set; }
        BondCouponPaymentStati Status { get; }
        IBondCouponPaymentDailyCalculationCollection DailyCalculations { get; }
        Money TotalAmountUnSettled { get; }

        void CalculateDailyInterest(InstrumentSize size, DateTime calcDate, DateTime settlementDate, IList<IBondCouponPaymentDailyCalculation> oldCalculations, IGLLookupRecords lookups);
        void SetToBeSettled(DateTime calcDate, DateTime settlementDate);
        void SettleInterest(DateTime calcDate);
        bool Cancel();
        bool Cancel(bool ignoreSizeNotZero);
    }
}
