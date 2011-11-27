using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Amount based order. This is an order that has to be filled with the requested amount of the order.
    /// </summary>
	public class OrderAmountBased : SecurityOrder, IOrderAmountBased
	{
		protected OrderAmountBased() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Initial value</param>
        /// <param name="tradedInstrument">Traded instrument</param>
        /// <param name="isValueInclComm">Is in the value the commission included?</param>
        /// <param name="feeFactory">Fee factory to use for calculating transaction costs</param>
        /// <param name="doNotChargeCommission">parameter that decides whether commission should be charged</param>
        public OrderAmountBased(IAccountTypeInternal account, Money value, IInstrument tradedInstrument, bool isValueInclComm, IFeeFactory feeFactory, bool doNotChargeCommission)
            : this(account, value, tradedInstrument, isValueInclComm, feeFactory, doNotChargeCommission, OrderActionTypes.NoAction)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Initial value</param>
        /// <param name="tradedInstrument">Traded instrument</param>
        /// <param name="isValueInclComm">Is in the value the commission included?</param>
        /// <param name="feeFactory">Fee factory to use for calculating transaction costs</param>
        /// <param name="doNotChargeCommission">parameter that decides whether commission should be charged</param>
        /// <param name="actionType">The action type that created this order.</param>
        public OrderAmountBased(IAccountTypeInternal account, Money value, IInstrument tradedInstrument, bool isValueInclComm, IFeeFactory feeFactory, bool doNotChargeCommission, OrderActionTypes actionType)
            : base(account, value, (ITradeableInstrument)tradedInstrument, doNotChargeCommission)
        {
            this.ActionType = actionType;
            this.IsValueInclComm = isValueInclComm;
            checkInitialValues();
            setCommission(feeFactory);
            // If same currency -> ExRate = 1
            exRate = ((ICurrency)Value.Underlying).GetExRate(TradedInstrument.CurrencyNominal, this.Side);
        }

		private void checkInitialValues()
		{
			if (base.Value != null && base.Value.Underlying != null)
			{
                if (!base.Value.IsMoney)
                {
                    throw new ApplicationException("A value based order should have a money value.");
                }
			}
			else
			{
				throw new ApplicationException("Value is mandatory.");
			}
		}

        protected override void setCommission(IFeeFactory feeFactory)
        {
            if (!DoNotChargeCommission)
            {
                base.setCommission(feeFactory);
                if (IsValueInclComm)
                {
                    // Only subtract commission from value for buy orders 
                    // For sell orders you need to sell the whole lot
                    if (Value.Sign)
                        Value += Commission;
                }
                else
                {
                    // Add the commission from value for sell orders 
                    // You need to sell more to get the commission + req. value back
                    // Buy orders -> just buy the req. value and pay the commission from the cash position
                    if (!Value.Sign)
                        Value += Commission;
                }
            }
        }

        /// <summary>
        /// The value added with the ServiceCharge
        /// The following calculation has been used:
        /// side  servCh value  fillOrderValue
        /// ----  ---    -----  -----------
        /// buy    -5   -100     -105
        /// sell   -5    100      105
        /// </summary>
        internal override InstrumentSize fillOrderValue(InstrumentSize size, Money value, Money serviceCharge, Money accruedInterest)
		{
            Money orderValue = MoneyMath.AdjustAmountForServiceCharge(value * -1, serviceCharge, Side, MathOperator.Add);
            orderValue += (accruedInterest * -1);
            return (InstrumentSize)orderValue;
        }

        /// <summary>
        /// Is the order amount based (always true)
        /// </summary>
        public override bool IsAmountBased
        {
            get { return true; }
        }

        /// <summary>
        /// Overridden, gets the requested instrument for this order
        /// </summary>
		public override IInstrument RequestedInstrument
		{
			get
			{
				return this.TradedInstrument;
			}
		}
		
        /// <summary>
        /// Overridden, gets the amount for this order
        /// </summary>
		public override Money Amount
		{ 
			get { return Value.GetMoney(); }
		}

        /// <summary>
        /// Overridden, gets the gross amount for this order.
        /// </summary>
        public override Money GrossAmount
        {
            get
            {
                if (Value.Sign)
                {
                    // Buy -> Add the commission to the nett value
                    return (Amount - Commission);
                }
                else
                {
                    // Sell -> Value already contains the commission
                    return Amount;
                }
            }
        }

        /// <summary>
        /// Overridden, gets the open amount for this order.
        /// </summary>
        public override Money OpenAmount
        {
            get 
            {
                if (FilledValue != null)
                {
                    if (Status >= OrderStati.Filled)
                        return new Money(0, (ICurrency)Value.Underlying);
                    else
                        return OpenValue.GetMoney();
                }
                else
                    return ClientAmount;
            }
        }

        /// <summary>
        /// Gets the client amount, the following calculation has been used:
        /// flag  side  req  com  value  clientvalue
        /// ----  ----  ---  ---  -----  -----------
        /// incl  buy   100    5    95      95
        /// incl  sell  100    5   100      95
        /// excl  buy   100    5   100     100
        /// excl  sell  100    5   105     100
        ///
        /// When Value includes the commission:
        /// Only subtract commission from value for buy orders 
        /// For sell orders you need to sell the whole lot
        /// When Value does not include the commission:
        /// For sell orders you need to sell the whole lot + the commission
        /// </summary>
		public virtual Money ClientAmount
		{
			get 
            {
                if (Value.Sign)
                {
                    // Buy -> Client gets the nett amount -> is value since the commission is already deducted
                    return Value.GetMoney();
                }
                else
                {
                    // Sell -> Client gets the nett amount -> the commission has not been deducted yet
                    return (Value.GetMoney() - Commission);
                }
            }
		}

        /// <summary>
        /// Is in the value the commission included?
        /// </summary>
        public virtual bool IsValueInclComm
        {
            get { return this.isValueInclComm; }
            protected set { this.isValueInclComm = value; }
        }

        /// <summary>
        /// Overridden, Determines the open value for this order.
        /// </summary>
		public override InstrumentSize OpenValue
		{
			get 
            {
                if (isFilled)
                    return Value.Clone(0M);
                else
                {
                    if (FilledValue == null)
                        return Value;
                    else if (FilledValue != null && Value.Underlying.Equals(FilledValue.Underlying))
                    {
                        InstrumentSize diff = Value - FilledValue;
                        if (diff.IsZero || diff.IsWithinTolerance(0.02M))
                            return Value.Clone(0M);
                        else
                            return (Value - FilledValue);
                    }
                    else
                    {
                        decimal exRate = ExRate;
                        if (exRate == 0)
                        {
                            ICurrency fromCur = (ICurrency)Value.Underlying;
                            ICurrency toCur = (ICurrency)FilledValue.Underlying;
                            exRate = fromCur.GetExRate(toCur, this.Side);
                            if (exRate == 0) exRate = 1;
                        }
                        InstrumentSize convFV = new InstrumentSize(FilledValue.Quantity / exRate, Value.Underlying);

                        InstrumentSize diff = Value - convFV;
                        if (diff.IsZero || diff.IsWithinTolerance(0.02M))
                            return Value.Clone(0M);
                        else
                            return (Value - convFV);
                    }
                }
            }
		}

        /// <summary>
        /// Overridden, calculates the ratio that has been filled against the total amount of the order.
        /// </summary>
        /// <returns>The fill ratio</returns>
        public override decimal GetChildRatio()
        {
            if (this.ParentOrder != null)
            {
                // Bypass calculation if just one child
                if (this.ParentOrder.ChildOrders.Count == 1)
                    return 1M;
                else
                {
                    decimal totalValue = this.ParentOrder.Value.Abs().Quantity;
                    if (IsStgOrder && ((IStgAmtOrder)this.ParentOrder).IsCurrencyConverted && ExRate != 0)
                        totalValue /= ExRate;

                    return this.Value.Abs().Quantity / totalValue;
                }
            }
            else
                return 1M;
        }

        public override ISecurityOrder Convert(Price price, B4F.TotalGiro.OrderRouteMapper.IOrderRouteMapper routeMapper)
        {
            throw new ApplicationException("Client Orders may not be converted at this time");
        }

        /// <summary>
        /// Overridden, Is this order fillable (always true)
        /// </summary>
		public override OrderFillability IsFillable
		{
			get { return OrderFillability.True; }
		}

		#region Privates

        //private IMonetaryOrder monetaryOrder;
        private bool isValueInclComm = true;

		#endregion
	}
}
