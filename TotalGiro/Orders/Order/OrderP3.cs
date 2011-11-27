using System;

namespace B4F.TotalGiro.Orders
{
	abstract public partial class Order : IOrder
	{
        /// <summary>
        /// This method de-aggregates the aggregated order.
        /// </summary>
        /// <returns>True when successfull</returns>
        public bool DeAggregate()
        {
            return DeAggregate(false);
        }

        /// <summary>
        /// This method de-aggregates the aggregated order.
        /// </summary>
        /// <param name="unApproveChildren">Should the child orders be unapproved</param>
        /// <returns>True when successfull</returns>
        public bool DeAggregate(bool unApproveChildren)
        {
            bool retVal = false;

            if (ChildOrders != null && ChildOrders.Count > 0)
            {
                for (int i = ChildOrders.Count; i > 0; i--)
                {
                    IOrder child = ChildOrders[i - 1];
                    ChildOrders.Remove(child, unApproveChildren);
                }
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// This method resets a StgOrder
        /// </summary>
        /// <returns>True when successfull</returns>
        protected bool reset()
        {
            bool retVal = false;

            if (IsStgOrder && ParentOrder == null && Approved && (Status == OrderStati.Placed || Status == OrderStati.Routed) && FilledValue == null)
            {
                ExportFile = null;
                Status = OrderStati.New;
                retVal = true;
            }
            return retVal;
        }
    }
}
