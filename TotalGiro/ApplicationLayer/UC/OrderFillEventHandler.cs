using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public delegate void OrderFillEventHandler(object sender, OrderFillEventArgs e);

    public class OrderFillEventArgs : EventArgs
    {
        public OrderFillEventArgs(OrderFillView orderFillView)
        {
            this.orderFillView = orderFillView;
        }

        public OrderFillView OrderFillView { get { return orderFillView; } }

        private OrderFillView orderFillView;
    }
}
