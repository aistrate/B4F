using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.StaticData;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    /// <summary>
    /// Class holds address data
    /// </summary>
    [Serializable]
    public class WSAddress
    {
        private string street;
        private string houseNumber;
        private string houseNumberSuffix;
        private string city;
        private string postalCode;
        private string country;

        #region Constructors

        public WSAddress() { }

        #endregion

        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.CRM.Address">Address</see> class.
        /// </summary>
        public WSAddress(string street, string houseNumber, string houseNumberSuffix,
                    string postalCode, string city, string country)
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
        [XmlElement("Street")]
        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        /// <summary>
        /// Get/set housenumber
        /// </summary>
        [XmlElement("HouseNumber")]
        public string HouseNumber
        {
            get { return houseNumber; }
            set { houseNumber = value; }
        }

        /// <summary>
        /// Get/set housenumber suffix
        /// </summary>
        [XmlElement("HouseNumberSuffix")]
        public string HouseNumberSuffix
        {
            get { return houseNumberSuffix; }
            set { houseNumberSuffix = value; }
        }

        /// <summary>
        /// Get/set postal code
        /// </summary>
        [XmlElement("PostalCode")]
        public string PostalCode
        {
            get { return postalCode; }
            set { postalCode = value; }
        }

        /// <summary>
        /// Get/set city
        /// </summary>
        [XmlElement("City")]
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        /// <summary>
        /// Get/set country
        /// </summary>
        [XmlElement("Country")]
        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        #endregion

        #region Public Methods

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
            theAddress += (Country == null) ? "" : Country.ToString() + " ";
            return theAddress;

        }

        /// <summary>
        /// Returns a string with all attributes seperated by a |. This can be used to import into an Excel file
        /// </summary>
        /// <returns>A string containing structured request information</returns>
        public string WriteToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                street, houseNumber, houseNumberSuffix, city, postalCode, country);
        }

        #endregion


    }
}
