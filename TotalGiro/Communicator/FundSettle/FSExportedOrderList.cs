using System;
using System.Collections.Generic;
using System.Collections;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Communicator.FSInterface
{
    public class FSExportedOrderList : GenericCollection<Order>
	{
        /// <summary>
        /// Constructor, holds a list of orders and binds them to an export file.
        /// based on an IList.
        /// </summary>
        /// <param name="parent">Export file.</param>
        /// <param name="orders">List of orders that have been written to the export file.</param>
		public FSExportedOrderList(FSExportFile parent, IList orders)
            :base(orders)
		{
			this.parent = parent;
		}

		#region Overrides

		public override void Add(Order item)
		{
			int index = Count;
			base.Add(item);
			item.ExportFile = (IFSExportFile)(this.parent);
		}

		#endregion

		#region Private Variables
        
		private FSExportFile parent;

		#endregion
	}
}
