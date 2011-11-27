using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees
{
    public interface IMgtFeeBreakupLineFee
    {
        FeeType FeeType { get; }
        Money Amount { get; }
    }
}
