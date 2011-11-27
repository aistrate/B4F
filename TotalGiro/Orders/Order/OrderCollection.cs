using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Holds an collection of <see cref="T:B4F.TotalGiro.Orders.Order">Order</see> objects.
    /// Implements basic Collection class behaviour.
    /// </summary>
    public class OrderCollection : TransientDomainCollection<IOrder>, IOrderCollection
	{
        public OrderCollection() : base() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bagOfOrders">List of orders</param>
        /// <param name="parent">Parent order</param>
		public OrderCollection(IOrder parent)
            : base()
		{
			this.ParentOrder = parent;
		}

        /// <summary>
        /// Gets/sets the parent order for this collection or orders.
        /// </summary>
		public IOrder ParentOrder
		{
			get { return parentOrder; }
			set 
            { 
                parentOrder = value;
                IsInitialized = true;
            }
		}

        public IOrderCollection NewCollection(Func<IOrder, bool> criteria)
        {
            OrderCollection returnValue = new OrderCollection(this.ParentOrder);
            returnValue.AddRange(this.Where(criteria));
            return returnValue;
        }

		#region IOrderCollection



		public Money TotalAmount()
		{
			Money amount = null;

            if (this.Count > 0)
                amount = this.Select(x => x.Amount).Sum();
			return amount;
		}

		public Money TotalAmount(IInstrument instrument)
		{
            return TotalAmount(instrument, true);
		}

        public Money TotalAmount(IInstrument instrument, bool useRequestedInstrument)
        {
            Money amount = null;

            foreach (IOrder order in this)
            {
                if (useRequestedInstrument)
                {
                    if (order.RequestedInstrument.Equals(instrument))
                        amount += order.Amount;
                }
                else
                {
                    // Monetary only -> Value needs to be in foreign currency but req inst is system currency
                    if (order.Value.Underlying.Equals(instrument) && order.RequestedInstrument.Equals(order.Account.AccountOwner.StichtingDetails.BaseCurrency))
                        amount += order.Amount;
                }
            }
            return amount;
        }

        public Money TotalAmountInSpecifiedNominalCurrency(ICurrency currencyNominal)
        {
            // This method returns the total amount of orders that 
            // are in a instrument in the specified currency
            // It only returns the orders in tradeable instruments
            Money amount = null;

            foreach (IOrder order in this)
            {
                ITradeableInstrument instrument = null;
                if (order.RequestedInstrument.IsTradeable)
                    instrument = (ITradeableInstrument)order.RequestedInstrument;
                if (instrument != null && instrument.CurrencyNominal.Equals(currencyNominal))
                {
                    if (order.Amount.Underlying.Equals(currencyNominal))
                        amount += order.GrossAmount;
                    else
                        amount += order.GrossAmount.Convert(order.ExRate, currencyNominal);
                }
            }
            return amount;
        }

        public InstrumentSize TotalSize(IInstrument instrument)
        {
            // Returns the total ordered size of the requested instrument
            // If Amount Based Order -> we use predicted size
            InstrumentSize size = null;

            foreach (IOrder order in this)
            {
                if (order.RequestedInstrument.Equals(instrument))
                {
                    if (order.IsSizeBased)
                        size += order.Value;
                    else
                    {
                        // Amount based order -> predict the size with latest rate/price
                        PredictedSize predSize = order.RequestedInstrument.PredictSize(order.Amount);
                        if (predSize.Status != PredictedSizeReturnValue.NoRate)
                            size += predSize.Size;
                    }
                }
            }
            return size;
        }

        /// <summary>
        /// The total Commission of the orders
        /// </summary>
        /// <returns>Total Commission</returns>
        public Money TotalCommission()
        {
            Money commission = null;
            foreach (IOrder order in this)
            {
                if (order.Commission != null && order.Commission.IsNotZero)
                {
                    if (order.Commission.Underlying.ToCurrency.IsBase)
                        commission += order.Commission;
                    else
                        commission += order.Commission.CurrentBaseAmount;
                }
            }
            return commission;
        }

        /// <summary>
        /// This method returns a filtered order collection in a specific instrument.
        /// It is also possible to return these orders depending on the instrument and the <see cref="T:B4F.TotalGiro.Orders.Side">side</see> of the order. 
        /// </summary>
        /// <param name="tradedInstrument">The instrument of interest</param>
        /// <param name="sideFilter">Value determines which orders are included depending on the <see cref="T:B4F.TotalGiro.Orders.Side">side</see> of the order</param>
        /// <returns>A filtered collection of orders</returns>
        public IOrderCollection Filter(IInstrument tradedInstrument, OrderSideFilter sideFilter)
        {
            OrderCollection col = new OrderCollection(this.ParentOrder);
            foreach (IOrder order in this)
            {
                if (order.RequestedInstrument.Equals(tradedInstrument))
                {
                    if (sideFilter == OrderSideFilter.All ||
                       (sideFilter == OrderSideFilter.Buy && order.Side == Side.Buy) ||
                       (sideFilter == OrderSideFilter.Sell && order.Side == Side.Sell))
                    {
                        col.Add(order);
                    }
                }
            }
            return col;
        }

        /// <summary>
        /// This method returns a filtered order collection in a specific orderType.
        /// </summary>
        /// <param name="orderType">The order types</param>
        /// <param name="sideFilter">Value determines which orders are included depending on the <see cref="T:B4F.TotalGiro.Orders.Side">side</see> of the order</param>
        /// <returns>A filtered collection of orders</returns>
        public IOrderCollection Filter(OrderTypes orderType, OrderSideFilter sideFilter)
        {
            OrderCollection col = new OrderCollection(this.ParentOrder);
            foreach (IOrder order in this)
            {
                if (order.OrderType.Equals(orderType))
                {
                    if (sideFilter == OrderSideFilter.All ||
                       (sideFilter == OrderSideFilter.Buy && order.Side == Side.Buy) ||
                       (sideFilter == OrderSideFilter.Sell && order.Side == Side.Sell))
                    {
                        col.Add(order);
                    }
                }
            }
            return col;
        }

        /// <summary>
        /// This method returns a order collection without the instruments to exclude
        /// </summary>
        /// <param name="excludedInstruments">The instruments to exclude from the result</param>
        /// <returns>A filtered collection of orders</returns>
        public IOrderCollection Exclude(IList<IInstrument> excludedInstruments)
        {
            OrderCollection col = new OrderCollection(this.ParentOrder);

            if (excludedInstruments == null || excludedInstruments.Count == 0)
                return this;

            foreach (IOrder order in this)
            {
                if (!excludedInstruments.Contains(order.RequestedInstrument))
                    col.Add(order);
            }
            return col;
        }

		#endregion

		public new void Add(IOrder item)
		{
            if (!IsInitialized)
                base.Add(item);
            else
            {
                if ((parentOrder.IsStgOrder &&
                    (parentOrder.IsAmountBased && ((IStgAmtOrder)parentOrder).IsCurrencyConverted))
                    ||
                    (parentOrder.IsStgOrder && ((IStgOrder)parentOrder).IsTypeConverted) && item.OrderType != parentOrder.OrderType)
                {
                    item.ParentOrder = ParentOrder;
                    base.Add(item);
                }
                else if (item.OrderType == parentOrder.OrderType)
			    {
                    ((Order)parentOrder).Value += item.Value;
                    adjustSide();

                    // If Stichting -> keep Buy & Sell value
                    if (parentOrder.IsStgOrder)
                    {
                        if (item.Side == Side.Buy)
                        {
                            ((Order)parentOrder).valueToBuy += item.Value.Quantity;
                            if (item.IsSecurity && ((ISecurityOrder)item).ServiceCharge != null && ((ISecurityOrder)item).ServiceCharge.IsNotZero)
                                ((Order)parentOrder).serviceChargeForBuy += ((ISecurityOrder)item).ServiceCharge.Quantity;
                        }
                        else
                        {
                            ((Order)parentOrder).valueToSell += item.Value.Quantity;
                            if (item.IsSecurity && ((ISecurityOrder)item).ServiceCharge != null && ((ISecurityOrder)item).ServiceCharge.IsNotZero)
                                ((Order)parentOrder).serviceChargeForSell += ((ISecurityOrder)item).ServiceCharge.Quantity;
                        }
                    }

                    item.ParentOrder = ParentOrder;
				    base.Add(item);
			    }
                else
                {
                    throw new ApplicationException(string.Format("Can only add {0} Orders", parentOrder.OrderType.ToString()));
                }
            }
        }

        public new bool Remove(IOrder item)
        {
            return Remove(item, false);
        }

        public bool Remove(IOrder item, bool unApprove)
		{
			bool success = false;

			if (Contains(item))
			{
				// Check if Remove is allowed
                OrderStatus status = OrderStateMachine.GetOrderStatus(ParentOrder.Status);
                if (status != null && !status.IsEditable)
                    throw new ApplicationException(string.Format("This order (order#{0}) is not editable", parentOrder.OrderID.ToString()));
                
                int count = Count;

                // IF TypeConverted (only one child) -> just cancel the parent
                if (!(parentOrder.IsStgOrder && ((IStgOrder)parentOrder).IsTypeConverted))
                {
                    ((Order)parentOrder).Value -= item.Value;
                    adjustSide();

                    // If Stichting -> keep Buy & Sell value
                    if (parentOrder.IsStgOrder)
                    {
                        if (item.Side == Side.Buy)
                            ((Order)parentOrder).valueToBuy -= item.Value.Quantity;
                        else
                            ((Order)parentOrder).valueToSell -= item.Value.Quantity;
                    }
                }
                else
                {
                    // TypeConverted (only one child) -> set the parent value is 0
                    ((Order)parentOrder).Value = parentOrder.Value.ZeroedAmount();
                }

                item.ParentOrder = null;
                if (unApprove && item.Approved)
                    item.UnApprove();

                base.Remove(item);
                if (count > Count)
                {
                    success = true;
                }

                // Check if there are still children
                if (Count == 0)
                    OrderStateMachine.SetNewCancelStatus(((Order)parentOrder), OrderCancelStateEvents.Cancel);

            }
			return success;
        }


        private void adjustSide()
        {
            if (parentOrder.IsStgOrder && parentOrder.IsNetted)
            {
                ((Order)parentOrder).Side = (parentOrder.Value.Sign ? Side.Buy : Side.Sell);
            }
        }

		#region Private Variables

		private IOrder parentOrder;

		#endregion

	}
}
