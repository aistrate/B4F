using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.History;

namespace B4F.TotalGiro.Notas
{
    public interface INotaInstrumentConversion : INotaTransactionBase
    {
        string Description { get; }
        InstrumentSize ConvertedInstrumentSize { get; }
        IInstrumentHistory InstrumentTransformation { get; }
    }
}
