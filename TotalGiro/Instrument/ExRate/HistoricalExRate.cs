using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments.ExRates
{
    /// <summary>
    /// Class represents currency exchange rates relative to the system base currency through time
    /// </summary>
    public class HistoricalExRate : IHistoricalExRate, IComparable
    {
        #region Constructor

        public HistoricalExRate() { }

        /// <summary>
        /// Initializes historical exchange rate
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <param name="rate">Rate relative to system base currency</param>
        /// <param name="rateDate">Point in time</param>
        public HistoricalExRate(ICurrency currency, decimal rate, DateTime rateDate)
        {
            this.currency = currency;
            this.rate = rate;
            this.rateDate = rateDate;
        }	
		
        /// <summary>
        /// Initializes historical exchange rate
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <param name="rate">Rate relative to system base currency</param>
        /// <param name="rateDate">Point in time</param>
        /// <param name="bid">Percentage paid for commission</param>
        /// <param name="ask">Percentage paid for commission</param>
        /// <param name="priceFactor">Helper fraction</param>
		public HistoricalExRate(ICurrency currency, decimal rate, DateTime rateDate, decimal bid, decimal ask, decimal priceFactor)
            : this(currency, rate, rateDate)
		{
			this.bid = bid;
			this.ask = ask;
			this.priceFactor = priceFactor;
        }

        #endregion

        #region Props

        /// <summary>
        /// Get/set unique identifier
        /// </summary>
		public virtual int Key
		{
			get { return key; }
			set { key = value; }
		}

        /// <summary>
        /// Get/set currency
        /// </summary>
		public virtual ICurrency Currency
		{
			get { return currency; }
			internal set { currency = value; }
		}

        /// <summary>
        /// Get/set rate
        /// </summary>
		public virtual decimal Rate
		{
			get { return rate; }
			set { rate = value; }
		}

        /// <summary>
        /// Get/set date
        /// </summary>
		public virtual DateTime RateDate
		{
			get { return rateDate; }
			internal set { rateDate = value; }
		}

        /// <summary>
        /// Bid is the percentage which you add at the rate
        /// </summary>
		public virtual decimal Bid
		{
			get { return bid; }
			internal set { bid = value; }
		}

       /// <summary>
        /// Ask is the percentage which you add at the rate
        /// </summary>
        public virtual decimal Ask
		{
			get { return ask; }
			internal set { ask = value; }
		}

        /// <summary>
        /// Helper fraction
        /// </summary>
		public virtual decimal PriceFactor
		{
			get { return (priceFactor <= 0 ? 1 : priceFactor); }
			internal set { priceFactor = value; }
        }

        /// <summary>
        /// Check if rate's date old
        /// </summary>
        public virtual bool IsOldDate
        {
            get { return Util.GetIsOldDate(RateDate, null); }
        }

        public virtual bool WasOldDateBy(DateTime referenceDate)
        {
            return (referenceDate - this.RateDate.Date).Days >= 3;
        }

        /// <summary>
        /// The date that the account was created.
        /// </summary>
        public DateTime CreationDate
        {
            get { return (creationDate.HasValue ? creationDate.Value : DateTime.MinValue); }
            set { creationDate = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get bid or ask-neutral rate
        /// </summary>
        /// <returns>Rate relative to base currency</returns>
        public virtual decimal GetExRate()
        {
            return HistoricalExRate.CalculateExRate(Rate, Ask, Bid, PriceFactor, 0M);
        }

        /// <summary>
        /// Get rate depending of bid or ask
        /// </summary>
        /// <param name="side">Bid or ask</param>
        /// <returns>Rate relative to base currency</returns>
        public virtual decimal GetExRate(Side side)
        {
            return HistoricalExRate.CalculateExRate(Rate, Ask, Bid, PriceFactor, side);
        }

        internal static decimal CalculateExRate(decimal rate, decimal ask, decimal bid, decimal priceFactor, Side side)
        {
            decimal retVal;

            retVal = rate;

            if (side == Side.Buy)
            {
                // Buy -> Ask
                // Ask is the percentage which you add at the rate
                retVal += (retVal * ask / 100);
            }
            else if (side == Side.Sell)
            {
                // Sell -> Bid
                // Bid is the percentage which you subtract at the rate
                retVal -= (retVal * bid / 100);
            }
            retVal *= priceFactor;
            return retVal;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Overridden creation of a hashcode
        /// </summary>
        /// <returns>Number</returns>
		public override int GetHashCode()
		{
			return this.Key.GetHashCode();
		}

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return ((this.RateDate != null) ? this.RateDate.ToShortDateString() + " " : "")
				+ ((this.Rate != 0) ? this.Rate.ToString() : "--");
		}

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        /// <returns>if true returns true else false</returns>
		public override bool Equals(object obj)
		{
			if ((obj != null) && (obj is HistoricalExRate))
			{
				HistoricalExRate testValue = (HistoricalExRate)obj;
				if ((testValue.Currency.Equals(this.currency)) && (testValue.RateDate == this.RateDate))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
				return false;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Overriden equality operator
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>if this.obj is equal to obj returns 0 
        /// elseif this.obj is smaller returns negative number
        /// else a positive number
        /// </returns>
        public virtual int CompareTo(object obj)
		{
			if (obj is HistoricalExRate)
			{
				HistoricalExRate temp = (HistoricalExRate)obj;

				return RateDate.CompareTo(temp.RateDate);
			}

			throw new ArgumentException("object is not a HistoricalExRate");
		}

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="a">First historical exchange rate to compare</param>
        /// <param name="b">Second historical exchange rate to compare</param>
        /// <returns>True or false</returns>
		public static bool operator >(HistoricalExRate a, HistoricalExRate b)
		{
			return (a.CompareTo(b)) > 0;
		}

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="a">First historical exchange rate to compare</param>
        /// <param name="b">Second historical exchange rate to compare</param>
        /// <returns>True or false</returns>
        public static bool operator <(HistoricalExRate a, HistoricalExRate b)
		{
			return (a.CompareTo(b)) < 0;
		}

		#endregion

		#region Private Variables

		private int key;
		private ICurrency currency;
		private decimal rate;
		private DateTime rateDate;
		private decimal bid;
		private decimal ask;
		private decimal priceFactor = 1;
        private DateTime? creationDate;

		#endregion

    }
}
