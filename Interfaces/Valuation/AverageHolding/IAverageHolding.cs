using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public interface IAverageHolding
    {
        int Key { get; set; }
        IManagementPeriodUnitParent UnitParent { get; set; }
        IAccountTypeInternal Account { get; }
        IInstrument Instrument { get; }
        int Period { get; }
        int Month { get; }
        DateTime BeginDate { get; }
        DateTime EndDate { get; }
        short Days { get; }
        Money AverageValue { get; }
        //IManagementFee Transaction { get; set; }
        DateTime CreationDate { get; }
        //bool DeactivateFeeItems();
        IAverageHolding PreviousHolding { get; }
        IAverageHoldingFeeCollection FeeItems { get; }
        Money GetPreviousCalculatedFee(FeeType feeType);
        bool ContainsNewFeeItems { get; }
        bool IsDirty { get; }
        bool IsInValid { get; }
        bool SkipFees { get; }
        string DisplayMessage { get; }

        bool IgnoreFeeItems();

    }
}
