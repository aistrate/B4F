using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.History
{
    public class CorporateActionCollection : TransientDomainCollection<ICorporateAction>, ICorporateActionCollection
    {
        public CorporateActionCollection()
            : base() { }

        public CorporateActionCollection(IInstrumentHistory parent)
            : base()
        {
            Parent = parent;
        }

        public IInstrumentHistory Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        public void AddInstrumentHistory(ICorporateAction entry)
        {
            if (IsInitialized)
                entry.InstrumentTransformation = Parent;
            base.Add(entry);
        }

        protected IInstrumentHistory parent;
    }
}
