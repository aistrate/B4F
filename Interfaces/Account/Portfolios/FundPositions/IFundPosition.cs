using System;
using System.Collections.Generic;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Valuations;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public interface IFundPosition : ITotalGiroBase<IFundPosition>, IAuditable, ICommonPosition
    {
        int Key { get; }
        B4F.TotalGiro.Accounts.IAccountTypeInternal Account { get; set; }
        DateTime CreationDate { get; set; }
        DateTime LastUpdated { get; }
        DateTime OpenDate { get; set; }
        InstrumentSize Size { get; set; }
        InstrumentSize TotalOpenSize { get; set; }
        IInstrumentsWithPrices InstrumentOfPosition { get; }
        string InstrumentDescription { get; }
        IFundPositionTxCollection PositionTransactions { get; }
        Money CurrentValue { get; }
        Money CurrentBaseValue { get; }
        decimal ModelAllocation { get; }

        Money MaxWithdrawalAmount { get; }
        IAssetClass AssetClass { get; }
        IRegionClass RegionClass { get; }
        IInstrumentsCategories InstrumentsCategories { get; }
        ISectorClass SectorClass { get; }
        IValuationMutation LastMutation { get; set; }
        IValuation LastValuation { get; }

        DateTime LastBondCouponCalcDate { get; set; }
        IBondCouponPaymentCollection BondCouponPayments { get; }
        IList<IBondCouponPaymentDailyCalculation> BondCouponCalculations { get; }
    }
}
