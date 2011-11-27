using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Valuations.ReportedData
{
    public class EndTermValueCollection : TransientDomainCollection<IEndTermValue>, IEndTermValueCollection
    {
        public EndTermValueCollection()
            : base() { }

        public EndTermValueCollection(IPeriodicReporting parent)
            : base()
        {
            Parent = parent;
        }

        public void AddEndTermValue(IEndTermValue entry)
        {
            if (IsInitialized)
                entry.ReportingPeriod = Parent;
            base.Add(entry);
        }

        public IPeriodicReporting Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        private IPeriodicReporting parent;
    }
}
