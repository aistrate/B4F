using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Size based order. This is an order that has to be filled with the exact size of the order.
    /// </summary>
	public class OrderSizeBased : SecurityOrder , IOrderSizeBased
	{
		internal OrderSizeBased() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Instrument size</param>
        /// <param name="isClosure">Is this order closing a position.</param>
        /// <param name="feeFactory">The set of rules to use for calculating transaction costs.</param>
        /// <param name="doNotChargeCommission">parameter that decides whether commission should be charged</param>
        public OrderSizeBased(IAccountTypeInternal account, InstrumentSize value, bool isClosure, IFeeFactory feeFactory, bool doNotChargeCommission)
            : this(account, value, isClosure, feeFactory, doNotChargeCommission, OrderActionTypes.NoAction)
		{
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Instrument size</param>
        /// <param name="isClosure">Is this order closing a position.</param>
        /// <param name="feeFactory">The set of rules to use for calculating transaction costs.</param>
        /// <param name="doNotChargeCommission">parameter that decides whether commission should be charged</param>
        /// <param name="actionType">The action type that created this order.</param>
        public OrderSizeBased(IAccountTypeInternal account, InstrumentSize value, bool isClosure, IFeeFactory feeFactory, bool doNotChargeCommission, OrderActionTypes actionType)
            : base(account, value, (ITradeableInstrument)value.Underlying, doNotChargeCommission)
        {
            this.ActionType = actionType;
            this.isClosure = isClosure;
            setCommission(feeFactory);

            // Accrued Interest for Client Orders
            if (account.IsAccountTypeCustomer && TradedInstrument.SecCategory.Key == SecCategories.Bond)
            {
                IBond bond = (IBond)TradedInstrument;
                if (bond.DoesPayInterest)
                {
                    IExchange exchange = bond.DefaultExchange ?? bond.HomeExchange;
                    AccruedInterestDetails calc = bond.AccruedInterest(value, bond.GetSettlementDate(DateTime.Today, exchange), exchange);
                    if (calc.IsRelevant)
                        this.AccruedInterest = calc.AccruedInterest.Abs() * (decimal)this.Side * -1M;
                }
            }
        }

        /// <summary>
        /// Is the order closing a position.
        /// </summary>
		public bool IsClosure
		{
			get { return isClosure; }
			set { isClosure = value; }
		}

        /// <summary>
        /// Is the order size based (always true)
        /// </summary>
		public override bool IsSizeBased
		{
			get { return true; }
		}

        /// <summary>
        /// Returns order type (size based)
        /// </summary>
		public override OrderTypes OrderType
		{
			get { return OrderTypes.SizeBased; }
		}

        /// <summary>
        /// Returns the fill ratio of this order which is calculated by dividing the value
        /// of the order by the value of it's parent order.
        /// </summary>
        /// <returns></returns>
        public override decimal GetChildRatio()
		{
            if (this.ParentOrder != null)
            {
                // Bypass calculation if just one child
                if (this.ParentOrder.ChildOrders.Count == 1)
                    return 1M;
                else
                    return this.Value.Abs() / (this.ParentOrder.Value.Abs());
            }
            else
                return 1;
		}

        /// <summary>
        /// Returns the requested (or offered) instrument
        /// </summary>
		public override IInstrument RequestedInstrument
		{
			get
			{
				return this.Value.Underlying;
			}
		}

        /// <summary>
        /// Returns the amount of the order. For a size based order, the price is multiplied
        /// by the size of the instrument.
        /// </summary>
		public override Money Amount
		{
			get
			{
				Money amount = null;

                if (base.Price != null && base.Value != null)
                {
                    amount = base.Value.CalculateAmount(base.Price);
                }
                else
                {
                    ICurrency currency = ((ITradeableInstrument)Value.Underlying).CurrencyNominal;
                    amount = new Money(0, currency);
                }
				return amount;
			}
		}

        /// <summary>
        /// Returns the gross amount, that is the amount minus the commission.
        /// In case of a sell, the amount is returned since the commission is part
        /// of the amount.
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
        /// Returns the amount of the order that has not been filled yet.
        /// </summary>
        public override Money OpenAmount
        {
            get 
            {
                if (FilledValue != null)
                    return Amount - FilledValue.CalculateAmount(base.Price);
                else
                    return Amount;
            }
        }

        /// <summary>
        /// Is the order fillable?
        /// </summary>
		public override OrderFillability IsFillable
		{
			get { return OrderFillability.True; }
		}

        internal override InstrumentSize fillOrderValue(InstrumentSize size, Money value, Money serviceCharge, Money accruedInterest)
		{
			return size;
		}
        public override ISecurityOrder Convert(Price price, B4F.TotalGiro.OrderRouteMapper.IOrderRouteMapper routeMapper)
        {
            throw new ApplicationException("Client Orders may not be converted at this time");
        }
        

        #region Privates

        private bool isClosure;

        #endregion
    }
}
