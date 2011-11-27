using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.StaticData
{
    public class Address : IEquatable<Address>
    {
        #region Constructors

        public Address() { }

        public Address(string street, string houseNumber, string houseNumberSuffix,
                       string postalCode, string city, ICountry country)
        {
            Street = street;
            HouseNumber = houseNumber;
            HouseNumberSuffix = houseNumberSuffix;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }

        public Address(string city, ICountry country)
        {
            City = city;
            Country = country;
        }

        #endregion

        #region Properties

        public virtual string Street { get; set; }
        public virtual string HouseNumber { get; set; }
        public virtual string HouseNumberSuffix { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string City { get; set; }
        public virtual ICountry Country { get; set; }
        
        public int CountryId
        {
            get { return Country != null ? Country.Key : countryId; }
            set { countryId = value; }
        }
        private int countryId = int.MinValue;

        public string StreetAddressLine
        {
            get
            {
                return string.Format("{0} {1}{2}{3}", Street,
                                                      HouseNumber, 
                                                      !string.IsNullOrEmpty(HouseNumberSuffix) ? "-" : "",
                                                      HouseNumberSuffix).Trim();
            }
        }

        public string CityAddressLine
        {
            get
            {
                string formattedPostalCode = Regex.Replace(PostalCode ?? "", @"(\d{4})\s*([a-zA-Z]{2})", "$1 $2").Trim();
                return string.Format("{0}  {1}", formattedPostalCode, City).Trim();
            }
        }

        public string CountryAddressLine
        {
            get { return CountryIso2 != "NL" ? Country.GetS(c => c.CountryName).Trim() : ""; }
        }

        private string CountryIso2
        {
            get { return Country.GetS(c => c.Iso2).ToUpper(); }
        }

        public string AddressLine1 { get { return StreetAddressLine; } }
        public string AddressLine2 { get { return CityAddressLine; } }
        public virtual string AddressLine3 { get { return Country.GetS(c => c.InternationalName); } }

        public virtual string DisplayAddress
        {
            get 
            {
                string s = "";
                if (City != null)
                    s += City;
                if (StreetAddressLine != "")
                    s += " " + StreetAddressLine;
                return s.Trim();
            }
        }

        public bool IsComplete
        {
            get { return MissingPropertyNames.Length == 0; }
        }

        public void AssertIsComplete()
        {
            string[] missing = MissingPropertyNames;

            if (missing.Length > 0)
                throw new ApplicationException(string.Format("Incomplete address (field{0} {1} {2} missing).",
                                                             missing.Length == 1 ? "" : "s",
                                                             missing.JoinStrings("", "", ", ", " and "),
                                                             missing.Length == 1 ? "is" : "are"));
        }

        private string[] MissingPropertyNames
        {
            get
            {
                var minimalProperties = new Dictionary<string, string>()
                {
                    { "Street", Street },
                    { "House Number", HouseNumber },
                    { "Postal Code", PostalCode },
                    { "City", City }
                };
                
                return minimalProperties.Where(kv => string.IsNullOrEmpty(kv.Value))
                                        .Select(kv => kv.Key)
                                        .ToArray();
            }
        }
        
        public bool ContainsData
        {
            get { return StreetAddressLine != ""; }
        }

        public bool IsEmpty
        {
            get
            {
                string all = string.Format("{0}{1}{2}{3}{4}", Street, HouseNumber, HouseNumberSuffix, City, PostalCode);
                return all == "" && Country == null && CountryId == int.MinValue; ;
            }
        }

        #endregion

        #region Initialize Country

        public bool CountryNeedsInitialization
        {
            get
            {
                bool initialization = false;
                if (Country == null && CountryId != int.MinValue)
                    initialization = true;
                return initialization;
            }
        }

        #endregion

        #region System.Object overrides

        public override string ToString()
        {
            string address = "";

            address += string.IsNullOrEmpty(Street)            ? "" : Street + " ";
            address += string.IsNullOrEmpty(HouseNumber)       ? "" : HouseNumber + " ";
            address += string.IsNullOrEmpty(HouseNumberSuffix) ? "" : HouseNumberSuffix + " ";
            address += string.IsNullOrEmpty(City)              ? "" : City + " ";
            address += string.IsNullOrEmpty(PostalCode)        ? "" : PostalCode + " ";
            address += Country == null                         ? "" : Country.InternationalName + " ";

            return address.TrimEnd();
        }

        bool IEquatable<Address>.Equals(Address other)
        {
            return other != null &&
                   this.ToString().Replace(" ", "").Equals(other.ToString().Replace(" ", ""));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<Address>)this).Equals(obj as Address);
        }

        #endregion
    }
}
