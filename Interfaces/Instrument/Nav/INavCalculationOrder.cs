using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments.Nav
{
    public interface INavCalculationOrder
    {
        int Key { get; set; }
        INavCalculation Parent { get; set; }
        B4F.TotalGiro.Orders.IOrder Order { get; set; }
    }
}
