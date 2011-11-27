using System;

namespace B4F.TotalGiro.Orders
{
	internal class OrderStatus
	{
		private int key;
		public string Name;
		public bool IsOpen;
		public bool IsEditable;
		public bool IsFillable;

		public OrderStati Key
		{
			get { return (OrderStati)key; }
		}
	}
}
