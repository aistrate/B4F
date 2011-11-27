using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.OrderRouteMapper;
using B4F.TotalGiro.Routes;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Class to hold an aggregated money order on Stichting level, inherits from AggregateMonetaryOrder
    /// </summary>
    public class StgMonetaryOrder : AggregateMonetaryOrder, IStgMonetaryOrder
    {
		protected StgMonetaryOrder() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Value of the order</param>
        /// <param name="routeMapper">Route information</param>
		public StgMonetaryOrder(IAccountTypeInternal account, Money value, IOrderRouteMapper routeMapper)
			: base(account, value)
		{
			setInitialValues(routeMapper);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="childOrder">Child order to create for this stichting order</param>
        /// <param name="routeMapper">Route information</param>
        public StgMonetaryOrder(IAccountTypeInternal account, IAggregateMonetaryOrder childOrder, IOrderRouteMapper routeMapper)
			: base(account, (IMonetaryOrder)childOrder)
		{
			setInitialValues(routeMapper);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="childOrder">Child order to create for this stichting order</param>
        public StgMonetaryOrder(IStgMonetaryOrder childOrder)
            : base(childOrder.Account, (IMonetaryOrder)childOrder)
        {
            // Used for Netting
            this.Route = childOrder.Route;
            this.exRate = childOrder.ExRate;
            this.IsNetted = true;
        }

        internal StgMonetaryOrder(IStgMonetaryOrder childOrder, decimal conversionRate, IOrderRouteMapper routeMapper)
            : base(childOrder.Account, childOrder.Amount.Convert(Math.Abs(conversionRate), childOrder.RequestedCurrency), (ICurrency)childOrder.Value.Underlying)
        {
            // Used for TypeConversion

            // Some check
            if (childOrder.ParentOrder != null)
                throw new ApplicationException("This order has a parent order and can no longer be converted.");

            setInitialValues(routeMapper);
            this.ChildOrders.Add(childOrder);
            childOrder.SetExRate(conversionRate);
            childOrder.Approve();
            //requestedCurrency = (ICurrency)childOrder.Value.Underlying;
            this.IsTypeConverted = true;
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
        /// Is this a stichting order (true)
        /// </summary>
        public override bool IsStgOrder
        {
            get { return true; }
        }

        /// <summary>
        /// Is this order fillable
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

        public override bool UnApprove()
        {
            bool retVal = false;

            if (((MoneyParent != null && IsAmountBased) || (MoneyParent == null && IsSizeBased)) && 
                ParentOrder == null && !IsNetted && Status < OrderStati.PartFilled)
            {
                // Only allow unapprove when a stichting MoneyOrder that is not aggregated/netted and not filled
                this.Approved = false;
                this.ApprovalDate = DateTime.MinValue;
                this.Status = OrderStati.New;
                return true;
            }
            else
                return base.UnApprove();
        }

        //public IStgOrder Convert(Price price, IOrderRouteMapper routeMapper)
        //{
        //    throw new ApplicationException("Cannot convert Money Orders");
        //}

        #region IStgOrder Members

        public new IRoute Route
        {
            get { return base.Route; }
            set { base.Route = value; }
        }

        public virtual bool IsSendable
        {
            get { return base.isSendable; }
        }

        public virtual bool IsEditable
        {
            get { return false; }
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

        public IStgMonetaryOrder Convert(decimal conversionRate, IOrderRouteMapper routeMapper)
        {
            return new StgMonetaryOrder(this, conversionRate, routeMapper);
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

        public virtual bool ChangeRoute(IRoute newroute)
        {
			throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool NeedsCurrencyConversion
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

    }
}
