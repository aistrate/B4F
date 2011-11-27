using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public interface IAverageHoldingFee : IMgtFeeBreakupLineFee
    {
        int Key { get; set; }
        IAverageHolding Parent { get; }
        IManagementPeriodUnit Unit { get; }
        Money CalculatedAmount { get; }
        Money Amount { get; }
        Money PreviousCalculatedFeeAmount { get; }
        FeeType FeeType { get; }
        DateTime CreationDate { get; }
        //IObsoleteStornoTransaction StornoTransaction { get; }
        bool IsIgnored { get; set; }
        bool IsEditted { get; }
        int CalcSourceKey { get; }
        IFeeCalcVersion FeeCalcSource { get; }
        decimal FeePercentageUsed { get; }
        string DisplayMessage { get; }

        //bool Deactivate();
        void Edit(Money calculatedAmount, IFeeCalcVersion calcSource);
    }
}
