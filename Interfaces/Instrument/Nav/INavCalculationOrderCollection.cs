using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments.Nav
{
    public interface INavCalculationOrderCollection : IList<INavCalculationOrder>
    {
        void AddOrder(INavCalculationOrder entry);
    }
}
