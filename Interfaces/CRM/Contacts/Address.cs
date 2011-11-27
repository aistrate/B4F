using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.StaticData;


namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// Class holds address data
    /// </summary>
	public class Address
	{
		public Address()	{		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.CRM.Address">Address</see> class.
        /// </summary>
        public Address(string street, string houseNumber, string houseNumberSuffix,
					string postalCode, string city, ICountry country)
		{
			this.street = street;
			this.houseNumber = houseNumber;
			this.houseNumberSuffix = houseNumberSuffix;
			this.postalCode = postalCode;
			this.city = city;
			this.country = country;
		}

        /// <summary>
        /// Get/set street
        /// </summary>
		public string Street
		{
			get { return street; }
			set { street = value; }
		}

        /// <summary>
        /// Get/set housenumber
        /// </summary>
		public string HouseNumber
		{
			get { return houseNumber; }
			set { houseNumber = value; }
		}

        /// <summary>
        /// Get/set housenumber suffix
        /// </summary>
		public string HouseNumberSuffix
		{
			get { return houseNumberSuffix; }
			set { houseNumberSuffix = value; }
		}

        /// <summary>
        /// Get/set postal code
        /// </summary>
		public string PostalCode
		{
			get { return postalCode; }
			set { postalCode = value; }
		}

        /// <summary>
        /// Get/set city
        /// </summary>
		public string City
		{
			get { return city; }
			set { city = value; }
		}

        /// <summary>
        /// Get/set country
        /// </summary>
        public ICountry Country
		{
			get { return country; }
			set { country = value; }
		}

		private string street;
		private string houseNumber;
		private string houseNumberSuffix;
		private string city;	
		private string postalCode;
        private ICountry country;

        /// <summary>
        /// Overriding composition of name
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			String theAddress = "";

			theAddress += (Street == null) ? "" : Street.ToString() + " ";
			theAddress += (HouseNumber == null) ? "" : HouseNumber.ToString() + " ";
			theAddress += (HouseNumberSuffix == null) ? "" : HouseNumberSuffix.ToString() + " ";
			theAddress += (City == null) ? "" : City.ToString() + " ";
			theAddress += (PostalCode == null) ? "" : PostalCode.ToString() + " ";
			theAddress += (this.Country == null) ? "" : this.Country.InternationalName.ToString() + " ";
			return theAddress;
			
		}


	


	

	
	}
}
