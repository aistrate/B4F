using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Orders
{
	public class MonetaryOrder : Order, IMonetaryOrder
	{
		protected MonetaryOrder() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Initial value for this order</param>
        /// <param name="feeFactory">Fee factory to use for calculating transaction costs</param>
        public MonetaryOrder(IAccountTypeInternal account, Money value, IFeeFactory feeFactory)
			: base(account, value) 
		{
            this.requestedCurrency = account.AccountOwner.BaseCurrency;
            checkInitialValues();
            setCommission(feeFactory);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">User account</param>
        /// <param name="value">Initial value for this order</param>
        /// <param name="requestedCurrency">Currency that is requested</param>
        /// <param name="feeFactory">Fee factory to use for calculating transaction costs</param>
        public MonetaryOrder(IAccountTypeInternal account, Money value, ICurrency requestedCurrency, IFeeFactory feeFactory)
			: base(account, value)
		{
            this.requestedCurrency = requestedCurrency;
            checkInitialValues();
            setCommission(feeFactory);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent order</param>
        /// <param name="feeFactory">Fee factory to use for calculating transaction costs</param>
        internal MonetaryOrder(IOrderAmountBased parent, IFeeFactory feeFactory)
            : base(parent.Account, (parent.Amount - parent.Commission))
        {

            if (parent.TradedInstrument != null && parent.TradedInstrument.IsTradeable)
            {
                ITradeableInstrument instrument = (ITradeableInstrument)parent.TradedInstrument;
                this.requestedCurrency = instrument.CurrencyNominal;
                checkInitialValues();
                setCommission(feeFactory);
                this.moneyParent = parent;
            }
            else
                throw new ApplicationException("Can not insert a monetary order without a requested currency");
        }




        /// <summary>
        /// Gets the requested currency in the order
        /// </summary>
		public virtual ICurrency RequestedCurrency
		{
			get { return this.requestedCurrency; }
		}

        /// <summary>
        /// Gets the parent order for this order
        /// </summary>
        public virtual IOrder MoneyParent
		{
            get { return this.moneyParent; }
        	internal set { this.moneyParent = value; }
        }

        /// <summary>
        /// Returns the name qualifier for the parent order
        /// </summary>
        public virtual string DisplayParent
        {
            get 
            {
                string parent = "";
                if (MoneyParent != null)
                    parent = MoneyParent.RequestedInstrument.ToString() + " (order#" + MoneyParent.OrderID.ToString() + ")";
                return parent; 
            }
        }

        /// <summary>
        /// Method to cancel MoneyOrders
        /// </summary>
        /// <returns>Returns true when cancelled </returns>
        public override bool Cancel()
        {
            // This is an ugly solution.
            // In the OrderStateMachine class it was impossible to cast the MoneyOrder to an Order object
            // -> due to NHibernate
            bool retVal = false;

            if (MoneyParent != null)
            {
                OrderStatus status = OrderStateMachine.GetOrderStatus(Status);
                if (status.IsOpen)
                {
                    // Only allow child to get cancelled if not filled yet
                    if (Transactions != null && Transactions.Count > 0)
                        throw new ApplicationException("This order can not be cancelled since it is already filled.");

                    // Only allow the child to get cancelled if the parent is also cancelled/terminated
                    status = OrderStateMachine.GetOrderStatus(MoneyParent.Status);
                    if (status.IsOpen)
                    {
                        throw new ApplicationException("This order can not be cancelled.");
                    }

                    CancelStatus = OrderCancelStati.Cancelled;
                    Status = OrderStati.Terminated;
                    DateClosed = DateTime.Now;
                    retVal = true;
                }
            }
            else
                retVal = base.Cancel();
            return retVal;
        }

		private void checkInitialValues()
		{
			exRate = ((ICurrency)Value.Underlying).GetExRate(this.Side);
			if (base.Value != null && base.Value.Underlying != null)
			{
				if (!base.Value.IsMoney)
				{
					throw new ApplicationException("A monetary order should have a money value.");
				}
			}
			else
			{
				throw new ApplicationException("Value is mandatory.");
			}

            if (Value.Underlying.Equals(this.RequestedInstrument))
                throw new ApplicationException("A monetary order should have a requested currency different from the value.");
        }

        /// <summary>
        /// Overridden, fills the order.
        /// </summary>
        /// <param name="amount">Amount to fill order with</param>
        /// <param name="price">The price of the instrument</param>
        /// <param name="exRate">The exchange rate of the currency</param>
        /// <param name="counterParty">Counter party</param>
        /// <param name="transactionDate">Transaction date</param>
        /// <returns>Order execution information object, implements IOrderExecution</returns>
        //public IOrderExecution Fill(Money amount, Price price, decimal exRate, IAccount counterParty, DateTime transactionDate)
        //{
        //    IOrderExecution returnValue = null;
        //    InstrumentSize size;

        //    if (orderInitialCheck(ref amount, price, exRate, counterParty, out size))
        //    {
        //        orderCheckSide(this.Side, ref amount, ref size);
        //        returnValue = new OrderExecution(this, counterParty, size, amount, price, exRate, transactionDate, "", null, 0M);
        //        fillOrder(returnValue, size, price, amount, null);
        //    }
        //    return returnValue;
        //}

        /// <summary>
        /// Overridden, Returns the ratio that needs to be filled against the total value for this order.
        /// </summary>
        /// <returns>The fill ratio for this order</returns>
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
                    if (!(this.Value.Underlying.Equals(this.ParentOrder.Value.Underlying)))
                    {
                        // Is this code used?
                        decimal exRate = ExRate;
                        if (exRate == 0)
                        {
                            ICurrency fromCur = (ICurrency)this.Value.Underlying;
                            ICurrency toCur = (ICurrency)this.ParentOrder.Value.Underlying;
                            exRate = fromCur.GetExRate(toCur, this.Side);
                        }
                        totalValue /= exRate;
                    }
                    return this.Value.Abs().Quantity / totalValue;
                }
            }
            else
                return 1M;
		}

        internal override InstrumentSize fillOrderValue(InstrumentSize size, Money value, Money serviceCharge, Money accruedInterest)
		{
            if (IsSizeBased)
                return size;
            else
            {
                return MoneyMath.AdjustAmountForServiceCharge(value * -1, serviceCharge, Side, MathOperator.Add);
            }
		}

        /// <summary>
        /// Overridden, Is this order a monetary order?
        /// </summary>
		public override bool IsMonetary
		{
			get { return true; }
		}

        /// <summary>
        /// Overridden, Is this order fillable?
        /// </summary>
		public override OrderFillability IsFillable
		{
			get { return OrderFillability.True; }
		}

        /// <summary>
        /// Overridden, Is this order size based?
        /// </summary>
		public override bool IsSizeBased
		{
			get 
            {
                if (((ICurrency)Value.Underlying).IsBase)
                    return false;
                else
                    return true;
            }
		}

        /// <summary>
        /// Overridden, Is this order amount based?
        /// </summary>
		public override bool IsAmountBased
		{
            get { return !IsSizeBased; }
		}

        /// <summary>
        /// Overridden, Returns the amount of money for this order.
        /// </summary>
		public override Money Amount
		{
            get { return Value.GetMoney(); }
		}

        /// <summary>
        /// Overridden, Returns the gross amount of money for this order.
        /// </summary>
        public override Money GrossAmount
        {
            get { return Amount; }
        }

        /// <summary>
        /// Overridden, Returns the gross amount of money for this order.
        /// </summary>
        public override Money OpenAmount
        {
            get
            {
                if (FilledValue != null)
                    return Amount - FilledValue.GetMoney();
                else
                    return Amount;
            }
        }

        /// <summary>
        /// Overridden, Returns the requested instrument for this order.
        /// </summary>
		public override IInstrument RequestedInstrument
		{
			get
			{
				return (IInstrument)this.requestedCurrency;
			}
		}

        /// <summary>
        /// Overridden, Returns the order type for this order.
        /// </summary>
		public override OrderTypes OrderType
		{
			get { return OrderTypes.Monetary; }
		}

		#region Privates

		protected ICurrency requestedCurrency;
        private IOrder moneyParent;

		#endregion

	}
}
