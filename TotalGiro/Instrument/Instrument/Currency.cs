using System;
using System.Collections;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments.History;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class representing a currency
    /// </summary>
    public class Currency : Instrument, ICurrency
    {
		public Currency() 
        {
            initialize();
        }

        /// <exclude/>
		internal Currency(Currency existingCurrency)
		{
            initialize();
            AltSymbol = existingCurrency.AltSymbol;
			CountryOfOrigin = existingCurrency.CountryOfOrigin;
			Symbol = existingCurrency.Symbol;
			base.DecimalPlaces = existingCurrency.DecimalPlaces;
			this.settlementCurrencyID = existingCurrency.settlementCurrencyID;
			EuroMember = existingCurrency.EuroMember;
		}

        /// <exclude/>
        public Currency(string symbol, ICountry countryOfOrigin)
        {
            initialize();
            this.symbol = symbol;
            this.countryOfOrigin = countryOfOrigin;
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.Cash;
        }

        public override bool CalculateCosts(IOrder order, IFeeFactory feeFactory)
        {
            checkCostCalculater(order, feeFactory);
            Commission commDetails = feeFactory.CalculateCommission(order);
            if (commDetails != null)
            {
                order.CommissionDetails = commDetails;
                order.CommissionDetails.Parent = (ICommissionParent)order;
            }
            return true;
        }

        public override bool CalculateCosts(IOrderAllocation transaction, IFeeFactory feeFactory, IGLLookupRecords lookups)
        {
            //checkCostCalculater(transaction, feeFactory);
            //Commission commDetails = feeFactory.CalculateCommission(transaction);
            //if (commDetails != null)
            //{
            //    transaction.CommissionDetails = commDetails;
            //    transaction.CommissionDetails.Parent = (ICommissionParent)transaction;
            //}
            return true;
        }

        /// <summary>
        /// Get/set three-letter currency abbreviation
        /// </summary>
		public virtual string Symbol
        {
            get { return symbol; }
            internal set { symbol = value; }
        }

        /// <summary>
        /// Get/set currency symbol
        /// </summary>
		public virtual string AltSymbol
        {
            get { return altSymbol; }
            internal set { altSymbol = value; }
        }

        /// <summary>
        /// Get asset type
        /// </summary>
        public virtual IAssetClass AssetClass
        {
            get { return this.assetClass; }
            set { this.assetClass = value; }
        }

        /// <summary>
        /// Get/set country of origin
        /// </summary>
		public virtual ICountry CountryOfOrigin
        {
            get {return countryOfOrigin; }
            internal set { countryOfOrigin = value; }
        }

        /// <summary>
        /// Get/set Euromembership true or false
        /// </summary>
		public virtual bool EuroMember
		{
			get { return euroMember; }
			set { euroMember = value; }
		}

        public virtual decimal LegacyExchangeRate { get; set; }

        /// <summary>
        /// Get/set default currency
        /// </summary>
		public virtual ICurrency BaseCurrency
		{
			get { return baseCurrency; }
			set { baseCurrency = value; }
		}

        /// <summary>
        /// Is this the base currency
        /// </summary>
		public virtual bool IsBase
		{
			get { return (this == baseCurrency); }
		}

        /// <summary>
        /// Get/set exchange rate
        /// </summary>
		public virtual IExRate ExchangeRate
		{
			get { return exchangeRate; }
			set { exchangeRate = value; }
		}

        /// <summary>
        /// Get/set settlement currency
        /// </summary>
		public virtual ICurrency SettlementCurrency
		{
			get { return settlementCurrency; }
			set { settlementCurrency = value; }
		}

        /// <summary>
        /// Predicts size of instrument based on amount of money
        /// </summary>
        /// <param name="inputAmount">Amount</param>
        /// <returns>PredictedSize object</returns>
        public override PredictedSize PredictSize(Money inputAmount)
        {
            PredictedSize retVal = new PredictedSize(PredictedSizeReturnValue.NoRate);

            if (ExchangeRate != null)
            {
                retVal.RateDate = ExchangeRate.RateDate;
                retVal.Size = inputAmount.Convert(this);
                retVal.Rate = ExchangeRate.Rate.ToString();
            }
            return retVal;
        }

        /// <summary>
        /// Get tradeable flag
        /// </summary>
        public override bool IsTradeable
        {
            get { return false; }
        }

        /// <summary>
        /// Get IsWithPrice flag
        /// </summary>
        public override bool IsWithPrice
        {
            get { return false; }
        }

        /// <summary>
        /// Get cash flag
        /// </summary>
        public override bool IsCash
        {
            get { return true; }
        }

        /// <summary>
        /// Is this Currency Obsolete?
        /// </summary>
        public virtual bool IsObsoleteCurrency
        {
            get { return (ParentInstrument != null && !IsActive && this.LegacyExchangeRate != 0M); }
        }

		//public override bool Equals(object obj)
		//{
		//    if (obj is ICurrency)
		//    {
		//        return this.Key.Equals(((ICurrency)obj).Key);
		//    }
		//    else
		//        return false;
		//}

		#region ExRate

        /// <exclude/>
        public IExRate GetHistoricalExRate(IDalSession session, DateTime date)
        {
            return HistoricalExRateMapper.GetNearestHistoricalExRate(session, this, date);
        }

        /// <summary>
        /// Get exchange rate by bid or ask
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
		public decimal GetExRate(Side side)
		{
			if (this.BaseCurrency == null)
                throw new ApplicationException(string.Format("No base currency available on currency {0}", this.ToString()));

            if (this.BaseCurrency.ExchangeRate == null)
                throw new ApplicationException(string.Format("Could not find the current exchange rate for currency {0}", this.BaseCurrency.ToString()));
            
            return Money.GetExRate(this.ExchangeRate, this.BaseCurrency.ExchangeRate, side);
		}

        /// <summary>
        /// Get exchange rate by currency and bid or ask
        /// </summary>
        /// <param name="toCurrency"></param>
        /// <param name="side"></param>
        /// <returns></returns>
		public decimal GetExRate(ICurrency toCurrency, Side side)
		{
            if (this.IsBase)
                return 1M;
            else
                return Money.GetExRate(this.ExchangeRate, toCurrency.ExchangeRate, side);
		}

		#endregion

        /// <summary>
        /// Overridden composition of name
        /// </summary>
        /// <returns>Name</returns>
		public override string ToString()
        {
            return symbol.ToString();
        }

        /// <summary>
        /// Overridden get/set decimals of number
        /// </summary>
		public override int DecimalPlaces
		{
			get
			{
				int places = base.DecimalPlaces;
				if (places == 0)
					places = 2;
				return places;
			}
			set { base.DecimalPlaces = value; }
		}

        /// <summary>
        /// Change screen format of a number
        /// </summary>
        /// <param name="Quantity">Quantity</param>
        public override string DisplayToString(decimal quantity)
		{
            return DisplayToString(quantity, AltSymbol, DecimalPlaces);
		}

        public static string DisplayToString(decimal quantity, string altSymbol)
        {
            return DisplayToString(quantity, altSymbol, 2);
        }

        public static string DisplayToString(decimal quantity, string altSymbol, int decimalPlaces)
        {
            string sign = "";
            decimal displayQuantity = quantity;

            if (quantity < 0)
            {
                sign = "-";
                displayQuantity = Math.Abs(displayQuantity);
            }

            if (decimalPlaces > 0)
            {
                string places = new string('0', decimalPlaces);
                string format = altSymbol + " " + sign + "#,##0." + places;
                return displayQuantity.ToString(format);
            }
            else
            {
                return displayQuantity.ToString(altSymbol + " " + sign + "#,##0.00");
            }
        }

        /// <summary>
        /// Get/set historical exchange rates
        /// </summary>
		public virtual IExRateCollection HistoricalExRates
		{
            get
            {
                HistoricalExRateCollection items = (HistoricalExRateCollection)this.historicalExRates.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }


		//#region IExRate Members

		//ICurrency IExRate.Currency
		//{
		//    get { return this; }
		//}

		//decimal IExRate.Rate
		//{
		//    get { return Rate; }
		//}

		//DateTime IExRate.RateDate
		//{
		//    get { return RateDate; }
		//}

		//decimal IExRate.Bid
		//{
		//    get { return Bid; }
		//}

		//decimal IExRate.Ask
		//{
		//    get { return Ask; }
		//}

		//decimal IExRate.PriceFactor
		//{
		//    get { return PriceFactor; }
		//}

		//#endregion

        public void SetObsolete(DateTime changeDate, ICurrency succeederCurrency, decimal legacyExchangeRate)
        {
            if (ParentInstrument != null)
                throw new ApplicationException(string.Format("This instrument already has been transformed to {0} (key {1})", ParentInstrument.Name, ParentInstrument.Key.ToString()));

            this.ParentInstrument = succeederCurrency;
            this.InActiveDate = changeDate;
            this.LegacyExchangeRate = legacyExchangeRate;
            InstrumentHistory history = new InstrumentsHistoryConversion(this, succeederCurrency, changeDate, 1M, 1, false);
            this.HistoricalTransformations.Add(history);
        }

        public override bool Validate()
        {
            if (this.altSymbol == string.Empty)
                throw new ApplicationException("The altSymbol is mandatory.");
            if (this.symbol == string.Empty)
                throw new ApplicationException("The Symbol is mandatory.");
            if (this.countryOfOrigin == null)
                throw new ApplicationException("The Country Of Origin is mandatory.");
            if (this.baseCurrency == null)
                throw new ApplicationException("The Base Currency is mandatory.");
            return base.validate();
        }


		#region Private Variables

		private bool euroMember;
		private ICountry countryOfOrigin;
		private string altSymbol;
		private string symbol;
        private IAssetClass assetClass;
        protected int settlementCurrencyID;
		private IExRate exchangeRate;
        private IDomainCollection<IExRate> historicalExRates;
		private ICurrency settlementCurrency;
		private ICurrency baseCurrency;

		#endregion
	}
}
