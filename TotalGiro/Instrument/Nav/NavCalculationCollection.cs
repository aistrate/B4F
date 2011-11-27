using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments.Nav
{
    public class NavCalculationCollection : TransientDomainCollection<INavCalculation>, INavCalculationCollection
    {
        public NavCalculationCollection()
            : base() { }

        public NavCalculationCollection(IVirtualFund parent)
            : base()
        {
            Parent = parent;
        }

        public IVirtualFund Parent { get; set; }




    }
}
