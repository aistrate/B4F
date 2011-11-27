using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// Class respresenting a country
    /// </summary>
    public class Country : ICountry
    {
        public Country() { }

        /// <summary>
        /// Initializes country object
        /// </summary>
        /// <param name="CountryName">Dutch Country Name</param>
        /// <param name="Iso2">ISO two digit standard</param>
        /// <param name="Iso3">ISO three digit standard</param>
        /// <param name="Iso3NumCode">ISO numerical code</param>
        /// <param name="InternationalName">International Country Name</param>
        public Country(string CountryName, string Iso2, string Iso3, string Iso3NumCode, string InternationalName)
        {
            this.InternationalName = InternationalName;
            this.Iso2 = Iso2;
            this.Iso3 = Iso2;
            this.Iso3NumCode = Iso3NumCode;
            this.CountryName = CountryName;
		}

        /// <summary>
        /// Get/set unique identifier
        /// </summary>
		public virtual Int32 Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Get/set name of country
        /// </summary>
		public virtual string CountryName
        {
            get { return countryName; }
            internal set { countryName = value; }
        }


        /// <summary>
        /// Get/set ISO two digit standard
        /// </summary>
		public virtual string Iso2
        {
            get { return iso2; }
            internal set { iso2 = value; }
        }

        /// <summary>
        /// Get/set ISO two three standard
        /// </summary>
        public virtual string Iso3
        {
            get { return iso3; }
            internal set { iso3 = value; }
        }

        /// <summary>
        /// Get/set international country name
        /// </summary>
		public virtual string InternationalName
        {
            get { return internationalName; }
            internal set { internationalName = value; }
        }

        /// <summary>
        /// Get/set ISO numerical three digits standard
        /// </summary>
		public virtual string Iso3NumCode
        {
            get { return iso3NumCode; }
            internal set { iso3NumCode = value; }
        }

        /// <summary>
        /// The holidays that belong to this country
        /// </summary>
        public virtual ICountryHolidayCollection CountryHolidays
        {
            get
            {
                if (countryHolidays == null)
                    this.countryHolidays = new CountryHolidayCollection(bagOfHolidays);
                return countryHolidays;
            }
        }
		
		#region OverRides

        /// <summary>
        /// Overridden creation of a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
		{
			return this.key.GetHashCode();
		}

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns></returns>
		public override string ToString()
        {
            return CountryName.ToString();
		}

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if equal, false if not equal.</returns>
        public override bool Equals(object obj)
		{
			if (((obj != null) && (obj is Country)) && (this.Key == ((Country) obj).Key))
				return true;
			else
				return false;
		}

		#endregion

		#region Private Variables

		private Int32 key;
		private string iso2;
		private string iso3;
		private string countryName;
		private string internationalName;
		private string iso3NumCode;
        private IList bagOfHolidays = new ArrayList();
        private ICountryHolidayCollection countryHolidays;

		#endregion

	}
}
