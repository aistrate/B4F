using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Communicator;

namespace B4F.TotalGiro.Instruments
{
    public class InstrumentSymbolCollection : TransientDomainCollection<IInstrumentSymbol>, IInstrumentSymbolCollection
    {
        public InstrumentSymbolCollection()
            : base() { }

        public InstrumentSymbolCollection(IInstrument parent)
            : base()
        {
            Parent = parent;
        }

        public IInstrument Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        public void Add(IInstrumentSymbol entry)
        {
            if (IsInitialized)
                entry.Instrument = Parent;
            base.Add(entry);
        }

        private IInstrument parent;
    }
}
