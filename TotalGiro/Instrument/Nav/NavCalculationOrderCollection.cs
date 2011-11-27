using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Instruments.Nav
{
    public class NavCalculationOrderCollection : TransientDomainCollection<INavCalculationOrder>, INavCalculationOrderCollection
    {
        public NavCalculationOrderCollection()
            : base() { }

        public NavCalculationOrderCollection(INavCalculation parent)
            : base()
        {
            Parent = parent;
        }

        public void AddOrder(INavCalculationOrder entry)
        {
            entry.Parent = Parent;
            base.Add(entry);
        }


        public INavCalculation Parent { get; set; }
    }
}
