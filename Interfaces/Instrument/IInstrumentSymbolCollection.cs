using System;
using System.Collections.Generic;
using B4F.TotalGiro.Communicator;
namespace B4F.TotalGiro.Instruments
{
    public interface IInstrumentSymbolCollection : IList<IInstrumentSymbol>
    {
        void Add(IInstrumentSymbol entry);
        IInstrument Parent { get; set; }
    }
}
