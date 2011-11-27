using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Notas
{
    public interface INotaTransfer : INotaTransactionBase
    {
        ITradeableInstrument Instrument { get; }
    }
}
