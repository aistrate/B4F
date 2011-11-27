using System;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.FeeCalculations;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public interface IManagementPeriodUnitFee : IMgtFeeBreakupLineFee
    {
        int Key { get; set; }
        IManagementPeriodUnit Parent { get; }
        FeeType FeeType { get; }
        Money Amount { get; set; }
        DateTime CreationDate { get; }

        void Edit(Money amount, IFeeCalcVersion calcSource);
    }
}
