using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments.Prices
{
    /// <summary>
    /// Class represents price of a instrument in time
    /// </summary>
    public class HistoricalPrice : IHistoricalPrice, IComparable
	{
		protected HistoricalPrice() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Prices.HistoricalPrice">HistoricalPrice</see> class.
        /// </summary>
        public HistoricalPrice(Price price, DateTime date)
		{
			this.price = price;
			this.date = date;
		}

        /// <summary>
        /// Unique identifier
        /// </summary>
		public virtual int Key
		{
			get { return this.key; }
			set { this.key = value; }
		}

        /// <summary>
        /// Get/set price
        /// </summary>
		public virtual Price Price
		{
			get { return this.price; }
			set { this.price = value; }
		}

        /// <summary>
        /// Get/set date
        /// </summary>
		public virtual DateTime Date
		{
			get { return this.date; }
			internal set { this.date = value; }
		}

        /// <summary>
        /// Get/set price when exchange opened
        /// </summary>
        public virtual Price OpenPrice
        {
            get { return this.openprice; }
            set { this.openprice = value; }
        }

        /// <summary>
        /// Get/set price when exchange closed
        /// </summary>
        public virtual Price ClosedPrice
        {
            get { return this.closedprice; }
            set { this.closedprice = value; }
        }

        /// <summary>
        /// Get/set highest price of the day
        /// </summary>
        public virtual Price HighPrice
        {
            get { return this.highprice; }
            set { this.highprice = value; }
        }

        /// <summary>
        /// Get/set lowest price of the day
        /// </summary>
        public virtual Price LowPrice
        {
            get { return this.lowprice; }
            set { this.lowprice = value; }
        }

        /// <summary>
        /// Price old flag
        /// </summary>
        public virtual bool IsOldDate
        {
            get 
            {
                IExchangeHolidayCollection holidays = null;
                if (Price != null && Price.Instrument.IsTradeable)
                {
                    ITradeableInstrument instrument = (ITradeableInstrument)Price.Instrument;
                    if (instrument.HomeExchange != null)
                        holidays = instrument.HomeExchange.ExchangeHolidays;
                }
                return Util.GetIsOldDate(Date, holidays); 
            }
        }

        public virtual bool WasOldDateBy(DateTime referenceDate)
        {
            return (referenceDate - this.Date.Date).Days >= 3;
        }

        public virtual IInstrumentsWithPrices Instrument
        {
            get { return Price.Get(e => e.Instrument) as IInstrumentsWithPrices; }
        }

        /// <summary>
        /// The date that the account was created.
        /// </summary>
        public DateTime CreationDate
        {
            get { return (creationDate.HasValue ? creationDate.Value : DateTime.MinValue); }
            set { creationDate = value; }
        }

		#region IComparable Members

        /// <summary>
        /// Equality method implementation, returns greater, equal or smaller
        /// </summary>
        /// <param name="obj">Object to compare with</param>
        /// <returns></returns>
        public virtual int CompareTo(object obj)
		{
			if (obj is HistoricalPrice)
			{
				HistoricalPrice temp = (HistoricalPrice)obj;

				return Date.CompareTo(temp.Date);
			}

			throw new ArgumentException("object is not a HistoricalPrice");
		}

        /// <summary>
        /// Greater than operator overload
        /// </summary>
        /// <param name="a">HistoricalPrice a</param>
        /// <param name="b">HistoricalPrice b</param>
        /// <returns>True is datetime of a is greater</returns>
		public static bool operator >(HistoricalPrice a, HistoricalPrice b)
		{
			return (a.CompareTo(b)) > 0;
		}

        /// <summary>
        /// Smaller than operator overload
        /// </summary>
        /// <param name="a">HistoricalPrice a</param>
        /// <param name="b">HistoricalPrice b</param>
        /// <returns>True is datetime of a is smaller</returns>
        public static bool operator <(HistoricalPrice a, HistoricalPrice b)
		{
			return (a.CompareTo(b)) < 0;
		}

		#endregion

        #region Overrides

        public override int GetHashCode()
        {
            int retVal = base.GetHashCode();
            if (Price != null)
            {
                retVal = Price.Instrument.Key * 100000;
                retVal += Date.GetHashCode();
            }
            return retVal;
        }

        public override bool Equals(object obj)
        {
            if (obj is HistoricalPrice)
            {
                HistoricalPrice temp = (HistoricalPrice)obj;
                return GetHashCode().Equals(temp.GetHashCode());
            }
            throw new ArgumentException("object is not a HistoricalPrice");
        }

        public override string ToString()
        {
            string retVal = base.ToString();
            if (Price != null)
                retVal = Date.ToShortDateString() + " " + Price.DisplayString;
            return retVal;
        }

        #endregion

        #region Private Variables

        private int key;
		private Price price;
		private DateTime date;
        private Price openprice;
        private Price closedprice;
        private Price highprice;
        private Price lowprice;
        private DateTime? creationDate;

		#endregion


		
	}
}
