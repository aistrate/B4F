using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public class AverageHoldingCollection : TransientDomainCollection<IAverageHolding>, IAverageHoldingCollection
    {
        public AverageHoldingCollection()
            : base() { }

        public AverageHoldingCollection(IManagementPeriodUnitParent parent)
            : base()
        {
            Parent = parent;
        }

        public IManagementPeriodUnitParent Parent { get; internal set; }
    }
}
