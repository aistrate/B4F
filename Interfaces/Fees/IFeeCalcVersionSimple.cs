using System;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public interface IFeeCalcVersionSimple : IFeeCalcVersion
    {
        bool NoFees { get; set; }
    }
}
