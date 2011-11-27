using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public interface IFeeCalcVersionFlat : IFeeCalcVersion
    {
        Money MinValue { get; set; }
        Money MinValueMonthly { get; }
        Money MaxValue { get; set; }
        Money MaxValueMonthly { get; }
        IFeeCalcLineCollection FeeLines { get; }
    }
}
