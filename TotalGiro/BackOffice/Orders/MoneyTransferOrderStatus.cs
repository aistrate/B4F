using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.BackOffice.Orders
{

	public class MoneyTransferOrderStatus : IMoneyTransferOrderStatus
	{

        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public bool IsEditable { get; set; }

        public MoneyTransferOrderStati Key
		{
            get { return (MoneyTransferOrderStati)key; }
		}

        private int key;
	}
}
