using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Fees.CommCalculations
{
	/// <summary>
	/// Abstract class from which all Commission Calculation classes are derived.
	/// </summary>
    abstract public class CommCalc: ICommCalc
	{
		protected CommCalc() { }

        public CommCalc(string name, ICurrency commCurrency, Money minValue, Money maxValue,
			Money fixedSetup)
		{
			Name = name;
			CommCurrency = commCurrency;
			MinValue = minValue;
			MaxValue = maxValue;
			FixedSetup = fixedSetup;

			checkCurrencies();
		}

		private void checkCurrencies()
		{
			if (CommCurrency == null)
			{
				throw new ApplicationException("Commission currency is mandatory");
			}

			checkCurrenciesSub(FixedSetup);
			checkCurrenciesSub(MinValue);
			checkCurrenciesSub(MaxValue);

            if (MinValue != null && MinValue.IsNotZero && MaxValue != null && MaxValue.IsNotZero)
			{
				if (MinValue > MaxValue)
				{
					throw new ApplicationException("Minimum Value can not be more than Maximum Value");
				}
			}
		}

		protected void checkCurrenciesSub(Money value)
		{
			if (value != null)
			{
				if (!CommCurrency.Equals(value.Underlying))
				{
					throw new ApplicationException("All values should be in the same currency");
				}
			}
		}

		/// <summary>
		/// The name of the commission calculation.
		/// </summary>
        public virtual string Name
		{
			get { return this.calcName; }
			set { this.calcName = value; }
		}

        /// <summary>
        /// The currency of the commission.
        /// </summary>
		public virtual ICurrency CommCurrency
		{
			get { return this.commCurrency; }
			set { this.commCurrency = value; }
		}

        /// <summary>
        /// The ID of the commission calculation.
        /// </summary>
		public virtual Int32 Key
		{
			get { return this.key; }
			set { this.key = value; }
		}

		/// <summary>
        /// A fixed amount always added to the commission.
		/// </summary>
        public virtual Money FixedSetup
		{
			get { return this.fixedSetup; }
			set { this.fixedSetup = value; }
		}

        /// <summary>
        /// The minimum value of the calculation.
        /// </summary>
		public virtual Money MinValue
		{
			get { return this.minValue; }
			set { this.minValue = value; }
		}

        /// <summary>
        /// The maximum value of the calculation.
        /// </summary>
		public virtual Money MaxValue
		{
			get { return this.maxValue; }
			set { this.maxValue = value; }
		}

        /// <summary>
        /// Indicates whether the commission calculation is flat, slab, size based, or simple.
        /// </summary>
		public abstract FeeCalcTypes CalcType { get; }

        /// <summary>
        /// The Asset manager this calculation belongs to.
        /// </summary>
        public virtual IAssetManager AssetManager
        {
            get { return this.assetManager; }
            set { this.assetManager = value; }
        }

        /// <summary>
        /// Calculates the commission for a given order.
        /// </summary>
        /// <param name="client">The order for which to calculate the commission.</param>
        /// <returns>The value of the commission.</returns>
		abstract public Money Calculate(ICommClient client);

        /// <summary>
        /// If this is a bond amount based order -> tweak the commission a bit for accrued interest
        /// </summary>
        /// <param name="client"></param>
        /// <param name="fee"></param>
        protected internal virtual void AdjustBondCommission(ICommClient client, ref Money fee)
        {
            decimal sign = client.Side == Side.Buy ? 1M : -1M;
            if (!client.AmountIsNett && !client.IsValueInclComm && client.AccruedInterest != null && client.AccruedInterest.IsNotZero)
                fee = client.GrossAmount * (fee.Quantity / (client.Amount + client.AccruedInterest + (fee * sign)).Quantity);
        }

		/// <summary>
		/// Calculates the amount for a size-based order, by finding out the price first.
		/// </summary>
        /// <param name="client">The order for which to calculate.</param>
		/// <returns>The calculated amount.</returns>
        protected Money GetAmountSizeBasedOrder(ICommClient client)
		{
            Price price = client.Price;
			if (price == null)
			{
				IPriceDetail priceDetail = client.TradedInstrument.CurrentPrice;
                if (priceDetail != null)
                {
                    price = priceDetail.Price;
                    SetCommissionInfoOnOrder(client, string.Format("Use current price {0} from date {1}", price.ToString(), priceDetail.Date.ToShortDateString()));
                }
			}
			else if (client.Price != null)
			{
                price = client.Price;
                SetCommissionInfoOnOrder(client, string.Format("Use price {0} from order", client.Price.ToString()));
			}

            if (price == null)
            {
                SetCommissionInfoOnOrder(client, "No price available so the commission is €0");
                return null;
            }
            else
                return client.Value.CalculateAmount(price);
		}

        protected Money addFixMinMax(Money fee, ICommClient client)
		{
			// Add Fixed setup
			if (FixedSetup != null && FixedSetup.IsNotZero)
			{
				fee += FixedSetup;
                SetCommissionInfoOnOrder(client, string.Format("Add setup {0}", FixedSetup.ToString()));
			}

			// Check Minimum Value
			if (MinValue != null && MinValue.IsNotZero && fee < MinValue)
			{
				// Extra check -> does the minimum size not exceed the order value
                Money orderAmount = client.Amount;
                if (orderAmount != null && orderAmount.IsNotZero)
                {
                    orderAmount = orderAmount.Abs();
                    if (!orderAmount.Underlying.Equals(this.CommCurrency))
                        orderAmount = orderAmount.Convert(this.CommCurrency);
                
                    if (orderAmount > fee)
                    {
                        fee = MinValue;
                        SetCommissionInfoOnOrder(client, string.Format("Calculated commission {0} is smaller than minimum value {1}", fee.ToString(), MinValue.ToString()));
                    }
                    else
                        SetCommissionInfoOnOrder(client, string.Format("Order amount {0} is smaller than minimum value of the commission {1}, so it is not applied", orderAmount.ToString(), MinValue.ToString()));
                }
			}

			// Check Maximum Value
			if (MaxValue != null && MaxValue.IsNotZero && fee > MaxValue)
			{
                SetCommissionInfoOnOrder(client, string.Format("Calculated commission {0} exceeds the maximum value {1}", fee.ToString(), MaxValue.ToString()));
                fee = MaxValue;
            }
			return fee;
		}

		/// <summary>
		/// Converts an amount to the currency of a given order.
		/// </summary>
		/// <param name="fee">The amount to convert.</param>
		/// <param name="client">The order whose currency to convert to.</param>
		/// <returns></returns>
        protected Money ConvertToOrderCurrency(Money fee, ICommClient client)
		{
			if (fee != null)
			{
                if ((ICurrency)fee.Underlying != client.OrderCurrency)
                    fee = fee.Convert(client.OrderCurrency);
				return fee.Round();
			}
			else
                return new Money(0, client.OrderCurrency);
		}

		/// <summary>
		/// Adds a message to field CommissionInfo of the order.
		/// </summary>
		/// <param name="client">The order to which to add the message.</param>
		/// <param name="message">The message to add.</param>
        protected void SetCommissionInfoOnOrder(ICommClient client, string message)
		{
            client.CommissionInfo += System.Environment.NewLine + message;
		}

        protected Money getCommAmount(Money amount)
        {
            Money result = new Money(0m, CommCurrency);
            Money comAmount = amount.Abs();
            if (!comAmount.Underlying.Equals(this.CommCurrency))
                comAmount = comAmount.Convert(this.CommCurrency);
            return comAmount;
        }

        /// <summary>
        /// A collection of child <b>CommCalcLines</b> objects belonging to the Commission Calculation.
        /// </summary>
        public virtual ICommCalcLineCollection CommLines
		{
            get
            {
                ICommCalcLineCollection col = (ICommCalcLineCollection)lines.AsList();
                if (col.Parent == null) col.Parent = this;
                col.ArrangeLines();
                return col;
            }
		}
	
		#region Override

		/// <summary>
        /// A string representation of the commission calculation; returns the value of property <b>Name</b>.
		/// </summary>
        /// <returns>The commission calculation's <b>Name</b> property value.</returns>
        public override string ToString()
		{
			return this.Name;
		}

        /// <summary>
        /// Hash function for this type. 
        /// </summary>
        /// <returns>A hash code for the current CommCalc object.</returns>
		public override int GetHashCode()
		{
			return this.key.GetHashCode();
		}

		#endregion


		#region Private Variables

		private int key;
		private string calcName;
		private ICurrency commCurrency;
		private Money fixedSetup;
		private Money minValue;
		private Money maxValue;
		private FeeCalcTypes feeCalcType;
        private IDomainCollection<CommCalcLine> lines = new CommCalcLineCollection();
        private IAssetManager assetManager;

		#endregion

	}
}
