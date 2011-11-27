using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.History
{
    public class InstrumentConversionCollection : TransientDomainCollection<IInstrumentConversion>, IInstrumentConversionCollection
    {
        public InstrumentConversionCollection()
            : base() { }

        public InstrumentConversionCollection(IInstrumentsHistoryConversion parent)
            : base()
        {
            Parent = parent;
        }

        public IInstrumentsHistoryConversion Parent
        {
            get { return (IInstrumentsHistoryConversion)parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        public InstrumentSize TotalOriginalSize()
        {
            return this.Select(x => x.ValueSize).Sum();
        }

        public InstrumentSize TotalConvertedSize()
        {
            return this.Select(x => x.ConvertedInstrumentSize).Sum();
        }

        public void AddConversion(IInstrumentConversion entry)
        {
            base.Add(entry);
        }

        private IInstrumentsHistoryConversion parent;

    }
}
