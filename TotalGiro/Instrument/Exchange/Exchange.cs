using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Collections;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class represents Exchange for stocks, mutual funds etc.
    /// </summary>
    public class Exchange : IExchange 
    {
        /// <summary>
        /// Initializes Exchange object
        /// </summary>
        /// <param name="ExchangeName">Name</param>
        /// <param name="defaultCurrency">Default currency</param>
        /// <param name="defaultCountry">Default country</param>
        /// <param name="defaultSettlementPeriod">Default settlement period</param>
        public Exchange(string ExchangeName, ICurrency defaultCurrency, ICountry defaultCountry, short defaultSettlementPeriod)
        {
            this.exchangeName = ExchangeName;
            this.defaultCountry = defaultCountry;
            this.DefaultCurrency = defaultCurrency;
            this.defaultSettlementPeriod = defaultSettlementPeriod;
        }
        protected Exchange() { }

        /// <summary>
        /// Get/set unique identifier
        /// </summary>
        public virtual Int32 Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Get/set default currency
        /// </summary>
		public virtual ICurrency DefaultCurrency
        {
            get {return defaultCurrency; }
            set { defaultCurrency = value; }
        }

        /// <summary>
        /// Get/set name of exchange
        /// </summary>
		public virtual string ExchangeName
        {
            get { return exchangeName; }
            set { exchangeName = value; }
        }

        /// <summary>
        /// Get/set default country
        /// </summary>
		public virtual ICountry DefaultCountry
        {
            get { return defaultCountry; }
            set { defaultCountry = value; }
        }

        /// <summary>
        /// Get/set default settlement period
        /// </summary>
		public virtual short DefaultSettlementPeriod
        {
            get { return defaultSettlementPeriod; }
            set { defaultSettlementPeriod = value; }
        }

        /// <summary>
        /// Get/set number of decimals for the exchange
        /// </summary>
        public virtual byte DefaultNumberOfDecimals
        {
            get { return numberofdecimals; }
            set { numberofdecimals = value; }
        }

        public virtual IExchangeHolidayCollection ExchangeHolidays
        {
            get
            {
                if (exchangeHolidays == null)
                    this.exchangeHolidays = new ExchangeHolidayCollection(bagOfExchangeHolidays);
                return exchangeHolidays;
            }
        }
		
		#region OverRides

        // <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>Name</returns>
		public override string ToString()
        {
            return this.ExchangeName.ToString();
		}

        /// <summary>
        /// Overridden creation of a hashcode.
        /// </summary>
        /// <returns>Unique value</returns>
        public override int GetHashCode()
		{
			return this.key.GetHashCode();
		}

		#endregion

		#region Equality

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="lhs">First exchange</param>
        /// <param name="rhs">Second exchange</param>
        /// <returns>true if equal, false if not equal.</returns>
		public static bool operator ==(Exchange lhs, Exchange rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (lhs.Key == rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Overridden inequality operator
        /// </summary>
        /// <param name="lhs">First exchange</param>
        /// <param name="rhs">Second exchange</param>
        /// <returns>true if equal, false if not equal.</returns>
		public static bool operator !=(Exchange lhs, Exchange rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				if (lhs.Key != rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if equal, false if not equal.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Exchange))
			{
				return false;
			}
			return this == (Exchange)obj;
		}

		#endregion

		#region Private Variables

		private Int32 key;
        private string exchangeName;
        private ICurrency defaultCurrency;
        private ICountry defaultCountry;
        private short defaultSettlementPeriod;
        private byte numberofdecimals;
        private IList bagOfExchangeHolidays = new ArrayList();
        private IExchangeHolidayCollection exchangeHolidays;

		#endregion

		
	}
}
