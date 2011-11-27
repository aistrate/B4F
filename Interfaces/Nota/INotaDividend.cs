using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Notas
{
    public interface INotaDividend : INotaGeneralOperationsBookingTaxeable
    {
        Money DividendAmount { get; }
        ITradeableInstrument Instrument { get; }
        DateTime ExDividendDate { get; }
        DateTime SettlementDate { get; }
        InstrumentSize Units { get; }
        Price UnitPrice { get; }
        Money ValueIncludingTax { get; }
    }
}
