using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Routes;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// This class holds aggregated monetary orders
    /// </summary>
    public class AggregateMonetaryOrder : MonetaryOrder, IAggregateMonetaryOrder
	{
		protected AggregateMonetaryOrder() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Initial value of the order</param>
		public AggregateMonetaryOrder(IAccountTypeInternal account, Money value)
			: base(account, value, null)
		{
		}

        protected AggregateMonetaryOrder(IAccountTypeInternal account, Money value, ICurrency requestedCurrency)
            : base(account, value, requestedCurrency, null)
        {
        }

        /// <summary>
        /// Constructor, creates an aggregated order from a child order
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="childOrder">Child order</param>
        public AggregateMonetaryOrder(IAccountTypeInternal account, IMonetaryOrder childOrder)
            : base(account, new Money(0M, (ICurrency)childOrder.Value.Underlying), childOrder.RequestedCurrency, null)
		{
			base.Side = childOrder.Side;
		}

        /// <summary>
        /// Is this an aggregated order
        /// </summary>
		public override bool IsAggregateOrder
		{
			get { return true; }
		}

        /// <summary>
        /// Checks the status of the order to see if it is fillable.
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

        #region IAggregateOrder

        /// <summary>
        /// Total open values for this order. That is the value that has not been filled.
        /// </summary>
        /// <returns></returns>
        public InstrumentSize TotalOpenValue()
		{
			InstrumentSize total = null;

			if (ChildOrders != null && ChildOrders.Count > 0)
			{
				foreach (OrderAmountBased order in ChildOrders)
				{
                    if (order.OpenValue != null && order.OpenValue.IsNotZero)
                        total += order.OpenValue;
				}
			}
			return total;
		}

		#endregion

        //public bool Send()
        //{
        //    bool retVal = false;
        //    retVal = OrderStateMachine.SetNewStatus(this, OrderStateEvents.Send);
        //    return retVal;
        //}

	}
}
