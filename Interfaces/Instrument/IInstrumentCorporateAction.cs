using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public interface IInstrumentCorporateAction : IInstrumentsWithPrices
    {
        ISecurityInstrument Underlying { get; set; }
        bool IsStockDividend { get; }
    }
}
