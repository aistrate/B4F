using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.OrderRouteMapper;
using B4F.TotalGiro.Routes;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Class to hold an aggregated amount order on Stichting level, inherits from AggregateAmtOrder
    /// </summary>
    public class StgAmtOrder : AggregateAmtOrder, IStgAmtOrder
    {
		protected StgAmtOrder() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Value of the order</param>
        /// <param name="tradedInstrument">The traded instrument</param>
        /// <param name="routeMapper">Route information</param>
		public StgAmtOrder(IAccountTypeInternal account, Money value, IInstrument tradedInstrument, IOrderRouteMapper routeMapper)
			: base(account, value, tradedInstrument)
		{
			setInitialValues(routeMapper);
            setServiceChargeForAmountBasedOrder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="childOrder">Child order to create for this stichting order</param>
        /// <param name="routeMapper">Route information</param>
        public StgAmtOrder(IAccountTypeInternal account, IAggregateAmtOrder childOrder, IOrderRouteMapper routeMapper)
            : base(account, (IOrderAmountBased)childOrder)
        {
            setInitialValues(routeMapper);
        }

        internal StgAmtOrder(IStgSizeOrder childOrder, Money value)
            : base(childOrder.Account, value, childOrder.TradedInstrument)
        {
            // Used for TypeConversion

            // Some check
            if (childOrder.ParentOrder != null)
                throw new ApplicationException("This order has a parent order and can no longer be converted.");

            this.IsTypeConverted = true;
            childOrder.ResetPlacedValue();
            this.Route = childOrder.Route;
            base.Side = childOrder.Side;
            this.ChildOrders.Add(childOrder);
            this.exRate = (childOrder.ExRate != 0m) ? childOrder.ExRate : 1m ;
            Validate();
        }

        internal StgAmtOrder(IStgAmtOrder childOrder, decimal exRate, Money convertedAmount)
            : base(childOrder.Account, convertedAmount, childOrder.TradedInstrument)
        {
            // Used for Fx Conversion

            // Some checks
            if (childOrder.ParentOrder != null)
                throw new ApplicationException("This order has a parent order and can no longer be converted.");
            if (exRate == 0M)
                throw new ApplicationException("The exchange rate can not be 0.");

            this.IsCurrencyConverted = true;
            this.Route = childOrder.Route;
            base.Side = childOrder.Side;
            this.ChildOrders.Add(childOrder);
            childOrder.Approve();
            childOrder.SetExRate(exRate);
            Validate();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="childOrder">Child order to create for this stichting order</param>
        public StgAmtOrder(IStgAmtOrder childOrder)
            : base(childOrder.Account, (IOrderAmountBased)childOrder)
        {
            // Used for Netting
            this.Route = childOrder.Route;
            this.exRate = childOrder.ExRate;
            this.IsNetted = true;
        }

		private void setInitialValues(IOrderRouteMapper routeMapper)
		{
			this.Route = routeMapper.GetRoute(this);
		}

        /// <summary>
        /// Is this an aggregated order (true)
        /// </summary>
        public override bool IsAggregateOrder
		{
			get { return true; }
        }

        /// <summary>
        /// Is this a stichting order? (true)
        /// </summary>
        public override bool IsStgOrder
        {
            get { return true; }
        }

        /// <summary>
        /// Is the order fillable
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

		#region IStgOrder Members

        public new IRoute Route
        {
            get { return base.Route; }
            set { base.Route = value; }
        }

        public virtual bool IsSendable
        {
            get
            {
                bool retVal = false;

                if (Value.Underlying.Equals(((ITradeableInstrument)RequestedInstrument).CurrencyNominal))
                {
                    retVal = base.isSendable;
                    if (retVal)
                        retVal = !NeedsTypeConversion;
                }
                else
                    retVal = !NeedsCurrencyConversion;
                return retVal;
            }
        }

        public virtual bool NeedsTypeConversion
        {
            get
            {
                bool retVal = false;

                IInstrumentExchange ie = getInstrumentExchange(true);
                if (ie != null)
                {
                    if (Side == Side.Buy && !ie.DoesSupportAmountBasedBuy)
                        retVal = true;
                    else if (Side == Side.Sell && !ie.DoesSupportAmountBasedSell)
                        retVal = true;
                }
                return retVal;
            }
        }

        public virtual bool IsEditable
        {
            get { return false; }
        }

        public virtual bool IsCurrencyConverted
        {
            get { return this.isCurrencyConverted; }
            internal set { this.isCurrencyConverted = value; }
        }

        public virtual bool NeedsCurrencyConversion
        {
            get 
            { 
                bool differentCurrency = !this.TradedInstrument.CurrencyNominal.Key.Equals(Amount.Underlying.Key);
                if (differentCurrency && !this.TradedInstrument.CurrencyNominal.IsActive && this.TradedInstrument.CurrencyNominal.ParentInstrument != null)
                    return false;
                else
                    return differentCurrency;
            }
        }

        public virtual new bool IsTypeConverted
        {
            get { return base.IsTypeConverted; }
            set { base.IsTypeConverted = value; }
        }

        public virtual new InstrumentSize ConvertedValue
        {
            get { return base.ConvertedValue; }
        }

        public virtual new ISecurityOrder Convert(Price price, IOrderRouteMapper routeMapper)
        {
            InstrumentSize value = Amount.CalculateSize(price);
            return new StgSizeOrder(this, value, price, routeMapper);
        }

        public virtual IStgSizeOrder ConvertBondOrder(Price price, DateTime settlementDate, IOrderRouteMapper routeMapper)
        {
            if (TradedInstrument.SecCategory.Key != SecCategories.Bond)
                throw new ApplicationException("This is not a bond order.");
            
            IBond bond = (IBond)TradedInstrument;
            if (bond.DoesPayInterest)
            {
                // Calculate backwards the number of bonds
                InstrumentSize value = bond.CalculateSizeBackwards(Amount, price, settlementDate);
                StgSizeOrder convertedOrder = new StgSizeOrder(this, value, price, routeMapper);
                AccruedInterestDetails calc = bond.AccruedInterest(value, settlementDate, null);
                if (calc.IsRelevant)
                    convertedOrder.AccruedInterest = calc.AccruedInterest;
                return convertedOrder;
            }
            else
                return (IStgSizeOrder)this.Convert(price, routeMapper);
        }

        public IStgAmtOrder ConvertFx(decimal exRate, Money convertedAmount)
        {
            return new StgAmtOrder(this, exRate, convertedAmount);
        }

        public virtual InstrumentSize ValueToBuy
        {
            get { return new InstrumentSize(valueToBuy, Value.Underlying); }
        }

        public virtual InstrumentSize ValueToSell
        {
            get { return new InstrumentSize(valueToSell, Value.Underlying); }
        }

        public virtual Money ServiceChargeForBuy
        {
            get { return new Money(serviceChargeForBuy, (ICurrency)Value.Underlying); }
        }

        public virtual Money ServiceChargeForSell
        {
            get { return new Money(serviceChargeForSell, (ICurrency)Value.Underlying); }
        }

        public new bool Send()
		{
            return base.Send();
		}

        public new bool Place()
        {
            return base.Place();
        }

        public new bool Reset()
        {
            return base.reset();
        }

        public void SetIsNetted(bool isNetted)
        {
            this.IsNetted = isNetted;
        }

        /// <summary>
        /// Change the order route. This can only be done on stichting level aggregated orders which have the new status
        /// </summary>
        /// <returns>True if successful</returns>
        public virtual bool ChangeRoute(IRoute newroute)
        {
            return changeRoute(newroute);
        }

		#endregion

        #region Privates

        private bool isCurrencyConverted = false;

        #endregion

    }
}
