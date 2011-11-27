using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.OrderRouteMapper;
using B4F.TotalGiro.Routes;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Class to hold an aggregated sizebased order on Stichting level, inherits from AggregateSizeOrder
    /// </summary>
    public class StgSizeOrder : AggregateSizeOrder, IStgSizeOrder
    {
		internal StgSizeOrder() {	}

        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="account">User account</param>
        ///// <param name="value">Value of the order</param>
        ///// <param name="price">Price for the instrument</param>
        ///// <param name="routeMapper">Route information</param>
        //public StgSizeOrder(IAccountTypeInternal account, InstrumentSize value, IOrderRouteMapper routeMapper)
        //    : base(account, value, false) 
        //{
        //    setInitialValues(routeMapper);
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="childOrder">Child order to create for this stichting order</param>
        /// <param name="routeMapper">Route information</param>
        public StgSizeOrder(IAccountTypeInternal account, IAggregateSizeOrder childOrder, IOrderRouteMapper routeMapper)
			: base(account, (IOrderSizeBased)childOrder) 
		{
			setInitialValues(routeMapper);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="childOrder">Child order to create for this stichting order</param>
        public StgSizeOrder(IStgSizeOrder childOrder)
            : base(childOrder.Account, (IOrderSizeBased)childOrder)
        {
            // Used for Netting
            this.Route = childOrder.Route;
            this.exRate = childOrder.ExRate;
            this.IsNetted = true;
        }

        internal StgSizeOrder(IStgAmtOrder childOrder, InstrumentSize value, Price price, IOrderRouteMapper routeMapper)
            : base(childOrder.Account, value, false)
        {
            // Used for TypeConversion

            // Some check
            if (childOrder.ParentOrder != null)
                throw new ApplicationException("This order has a parent order and can no longer be converted.");

            this.IsTypeConverted = true;
            setInitialValues(routeMapper);
            this.price = price;
            this.ChildOrders.Add(childOrder);
            childOrder.Approve();
            //this.CommissionDetails = childOrder.CommissionDetails;
            this.exRate = childOrder.ExRate;
            Validate();
        }

		private void setInitialValues(IOrderRouteMapper routeMapper)
		{
            this.Route = routeMapper.GetRoute(this);
        }

        public virtual bool NeedsCurrencyConversion
        {
            get { return false; }
        }

        /// <summary>
        /// Is the order size based?
        /// </summary>
        public override bool IsSizeBased
        {
            get { return true; }
        }

        /// <summary>
        /// Is this order an aggregated order
        /// </summary>
        public override bool IsAggregateOrder
        {
            get { return true; }
        }

        /// <summary>
        /// Is this a stichting level order?
        /// </summary>
        public override bool  IsStgOrder
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the order type
        /// </summary>
		public override OrderTypes OrderType
		{
			get { return OrderTypes.SizeBased; }
		}

        public override InstrumentSize OpenValue
        {
            get 
            {
                if (isFilled)
                    return Value.Clone(0M);
                else
                {
                    if (FilledValue == null)
                        return PlacedValue;
                    else
                    {
                        InstrumentSize diff = PlacedValue - FilledValue;
                        if (diff.IsZero || diff.IsWithinTolerance(0.02M))
                            return PlacedValue.Clone(0M);
                        else
                            return (PlacedValue - FilledValue);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if this order is in a state that is fillable
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
            OrderValidationResult result = new OrderValidationResult(OrderValidationSubType.Success, "");
            short? decimals = null;
            decimal tickSize = 0M;

            IInstrumentExchange ie = getInstrumentExchange(true);
            if (ie != null)
            {
                decimals = ie.NumberOfDecimals;
                tickSize = ie.TickSize;
            }

            if (decimals == null && Route != null && Route.Exchange != null)
                decimals = Route.Exchange.DefaultNumberOfDecimals;

            if (tickSize > 0 && this.OrderType == OrderTypes.SizeBased)
                AdjustToTickSize(tickSize);
            else if (this.OrderType == OrderTypes.SizeBased && this.RequestedInstrument.SecCategory.Key == SecCategories.Bond)
                AdjustToTickSize(((IBond)this.RequestedInstrument).NominalValue.Quantity);
            else if (decimals != null)
                SetNumberOfDecimals((short)decimals);
            return result;
        }

        #region IStgOrder Members

        public new IRoute Route
        {
            get { return base.Route; }
            set { base.Route = value; }
        }

        public virtual bool IsSendable
        {
            get { return true; }
        }

        public virtual bool IsEditable
        {
            get 
            {
                bool retVal = false;
                if ((int)Status < (int)OrderStati.Routed)
                    retVal = true;
                return retVal;
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
            Money value = this.Value.CalculateAmount(price);
            return new StgAmtOrder(this, value);
        }

        public virtual InstrumentSize ValueToBuy
        {
            get { return new InstrumentSize(valueToBuy, Value.Underlying); }
        }

        public virtual InstrumentSize ValueToSell
        {
            get { return new InstrumentSize(valueToSell, Value.Underlying); }
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

        public bool SetNumberOfDecimals(short decimals)
        {
            bool hasChanged = false;

            if (IsEditable && decimals >= 0)
            {
                if (Value.NumberOfDecimals > decimals)
                {
                    PlacedValue = Value.Round(decimals);
                    hasChanged = true;
                }
                else if (Value.NumberOfDecimals <= decimals)
                {
                    PlacedValue = null;
                    hasChanged = true;
                }
                return hasChanged;
            }
            else
                throw new ApplicationException("The order is not editable at this stage.");
        }

        public void AdjustToTickSize(decimal tickSize)
        {
            if (IsEditable && tickSize > 0)
            {
                int factor = System.Convert.ToInt32(Value.Quantity / tickSize);
                if (factor == 0 && Value.Quantity != 0)
                    factor = 1 * (Value.Sign ? 1 : -1);
                PlacedValue = Value.Clone(tickSize * System.Convert.ToDecimal(factor));
            }
            else
                throw new ApplicationException("The order is not editable at this stage.");
        }

        public void ResetPlacedValue()
        {
            if (this.placedValue != null)
            {
                PlacedValue = Value;
            }
        }

        #endregion

    }
}
