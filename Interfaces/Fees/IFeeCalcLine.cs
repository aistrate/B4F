using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public interface IFeeCalcLine
    {
        int Key { get; set; }
        IFeeCalcVersion Parent { get; set; }
        short SerialNo { get; set; }
        decimal FeePercentage { get; set; }
        Money LowerRange { get; set; }
        Money StaticCharge { get; set; }
        Money UpperRange { get; set; }
        string DisplayRange { get; }

        Money Calculate(Money val, decimal days);
        bool Envelops(B4F.TotalGiro.Instruments.Money val);
        bool IsUnder(Money val);
    }
}
