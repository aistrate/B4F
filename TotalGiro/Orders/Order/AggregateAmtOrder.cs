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
    /// This class holds aggregated amount-based orders
    /// </summary>
	public class AggregateAmtOrder : OrderAmountBased, IAggregateAmtOrder
	{
		protected AggregateAmtOrder() { }

		/// <summary>
		/// Constructor, initializes amount-based order class
		/// </summary>
		/// <param name="account">User account</param>
		/// <param name="value">Value in money</param>
		/// <param name="tradedInstrument">Traded instrument</param>
        public AggregateAmtOrder(IAccountTypeInternal account, Money value, IInstrument tradedInstrument)
            : base(account, value, tradedInstrument, true, null, true)
		{
		}

        /// <summary>
        /// Constructor, initializes amount-based order class
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="childOrder">Amount-based child order</param>
        public AggregateAmtOrder(IAccountTypeInternal account, IOrderAmountBased childOrder)
			: base(account, new Money(0M, (ICurrency)childOrder.Value.Underlying), childOrder.TradedInstrument, childOrder.IsValueInclComm, null, true)
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
        /// Determines if this order is fillable depending on the state it is in.
        /// </summary>
		public override OrderFillability IsFillable
		{
			get 
			{
				OrderFillability value = OrderFillability.True;
				
				if (TradedInstrument == null)
					throw new ApplicationException("Traded instrument on the Order can not be null");

				//if (TradedInstrument is ITradeableInstrument)
				//{
				//    if ((!((ITradeableInstrument)TradedInstrument).CurrencyNominal.IsBase))
				//    {
				//        // instrument in different currency
				//        // order is not fillable
				//        value = OrderFillability.NoDifferentCurrencies;
				//    }
				//}

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
        //    get 
        //    {
        //        return (Value.Underlying.Equals(((ITradeableInstrument)RequestedInstrument).CurrencyNominal));
        //    }
        //}

		#region IAggregatedOrder Members

        /// <summary>
        /// Calculates the value of the order that has not been filled yet.
        /// </summary>
        /// <returns>The total open value represented by a InstrumentSize</returns>
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


        
        //public bool Send()
        //{
        //    bool retVal = false;
        //    retVal = OrderStateMachine.SetNewStatus(this, OrderStateEvents.Send);
        //    return retVal;
        //}

		#endregion

	}
}
