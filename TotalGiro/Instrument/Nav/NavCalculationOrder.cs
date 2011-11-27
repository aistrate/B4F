using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Instruments.Nav
{
    public class NavCalculationOrder :INavCalculationOrder
    {
        public NavCalculationOrder() { }
        public NavCalculationOrder(IOrder newOrder, INavCalculation parent)
        {
            this.Order = newOrder;
            this.Parent = parent;
        }
        public int Key { get; set; }
        public INavCalculation Parent { get; set; }
        public B4F.TotalGiro.Orders.IOrder Order { get; set; }

    }
}
