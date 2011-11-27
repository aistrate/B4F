using System;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public interface ICorporateActionStockDividend : ICorporateActionHistory
    {
        short BasisNominator { get; }
        short BasisDenominator { get; }
    }
}
