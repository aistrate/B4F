using System;
using System.Collections.Generic;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.History
{
    public interface IInstrumentConversionCollection : IList<IInstrumentConversion>
    {
        void AddConversion(IInstrumentConversion entry);
        IInstrumentsHistoryConversion Parent { get; set; }
        InstrumentSize TotalOriginalSize();
        InstrumentSize TotalConvertedSize();
    }
}
