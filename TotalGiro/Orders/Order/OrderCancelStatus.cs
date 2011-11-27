using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Class to hold all stati when the order has been cancelled
    /// </summary>
	internal class OrderCancelStatus
	{
		private int key;
		public string Name;
		public bool TerminateOrder;

        /// <summary>
        /// Unique key, readonly
        /// </summary>
		public OrderCancelStati Key
		{
			get { return (OrderCancelStati)key; }
		}
	}
}
