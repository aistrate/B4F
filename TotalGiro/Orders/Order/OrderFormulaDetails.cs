using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Orders
{
    public class OrderFormulaDetails : IOrderFormulaDetails
    {
        public virtual int Key { get; set; }
        
        /// <summary>
        /// Returns the TopMost order (being actually sent to the exchange) of the current order
        /// </summary>
        public virtual IOrder TopParentOrder
        {
            get { return this.topParentOrder; }
        }

        /// <summary>
        /// Returns the status of the top parent order as a string for display purposes.
        /// When the top parent is null -> return empty string
        /// </summary>
        public virtual string TopParentDisplayStatus
        {
            get
            {
                string status = string.Empty;
                if (this.topParentOrder != null)
                {
                    status += TopParentOrder.DisplayStatus;
                }
                return status;
            }
        }

        private IOrder topParentOrder;
    }
}
