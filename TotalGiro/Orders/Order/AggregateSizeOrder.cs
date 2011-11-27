using System;
using System.Collections;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Routes;
//using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Orders
{
	public class AggregateSizeOrder : OrderSizeBased, IAggregateSizeOrder
	{
		internal AggregateSizeOrder() {	}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Initial value for the order</param>
        /// <param name="isClosure">Closure</param>
        public AggregateSizeOrder(IAccountTypeInternal account, InstrumentSize value, bool isClosure)
            : base(account, value, isClosure, null, true)
        {
        }

        /// <summary>
        /// Constructor, creates an aggregated order from a child order
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="childOrder">Child order</param>
        public AggregateSizeOrder(IAccountTypeInternal account, IOrderSizeBased childOrder)
			: base(account, childOrder.Value.Clone(0M), childOrder.IsClosure, null, true) 
		{
            base.Side = childOrder.Side;
        }

        /// <summary>
        /// Returns the total value of the order that has not been filled.
        /// </summary>
        /// <returns>The total value of the order that has not been filled.</returns>
		public InstrumentSize TotalOpenValue()
		{
			InstrumentSize total = null;

			if (ChildOrders != null && ChildOrders.Count > 0)
			{
				foreach (OrderSizeBased order in ChildOrders)
				{
                    if (order.OpenValue != null && order.OpenValue.IsNotZero)
                        total += order.OpenValue;
				}
			}
			return total;
		}

        /// <summary>
        /// Is this order size based
        /// </summary>
		public override bool IsSizeBased
		{
			get { return true; }
		}

        /// <summary>
        /// Is this an aggregated order
        /// </summary>
		public override bool IsAggregateOrder
		{
			get { return true; }
		}

        /// <summary>
        /// Gets the order type. This is one of the enumeration <seealso cref="OrderType">OrderType</seealso>
        /// </summary>
		public override OrderTypes OrderType
		{
			get { return OrderTypes.SizeBased; }
		}

        /// <summary>
        /// Determines if the order is fillable according to the state it is in.
        /// </summary>
		public override OrderFillability IsFillable
		{
			get
			{
				OrderFillability value = OrderFillability.True;

				OrderStatus status = OrderStateMachine.GetOrderStatus(this.Status);
				if (!(status.IsFillable))
				{
					value = OrderFillability.NoStatusNotFillable;
				}
				return value;
			}
		}

        public override OrderValidationResult Validate()
        {
            return new OrderValidationResult(OrderValidationSubType.Success, "");
        }

        //public virtual bool IsSendable
        //{
        //    get { return base.isSendable; }
        //}

        //public bool Send()
        //{
        //    bool retVal = false;
        //    retVal = OrderStateMachine.SetNewStatus(this, OrderStateEvents.Send);
        //    return retVal;
        //}
	}
}
