using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace B4F.TotalGiro.StaticData
{
	public class TelephoneNumber
    {
        #region Constructors

        public TelephoneNumber()	{	}
		public TelephoneNumber(string number)
		{
            this.Number = number;
		}

		public TelephoneNumber(string CountryCode, string AreaCode, string LocalNumber)
		{
            this.CountryCode = countryCode;
            this.AreaCode = areaCode;
            this.LocalNumber = LocalNumber;
        }

        #endregion

        #region Properties

        public virtual string Number
        {
            get 
            {
                if (this.number != null && this.number.Length > 0)
                    return number;
                else
                    return StandardizedNumber;
            }
            set 
            {
                this.number = value;
            }
        }

        public virtual string StandardizedNumber
        {
            get
            {
                checkNeedForDisection();
                if (this.countryCode != "" || this.areaCode != "" || this.localNumber != "")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("");

                    if (this.CountryCode != null && CountryCode.Length > 0)
                    {
                        sb.Append("+" + this.CountryCode);
                        if (this.AreaCode != null && this.AreaCode.Length > 0)
                            sb.Append(" (0)" + this.AreaCode.Substring(1));
                    }
                    else
                    {
                        if (this.AreaCode != null && this.AreaCode.Length > 0)
                            sb.Append(this.AreaCode);
                    }
                    sb.Append(" " + this.LocalNumber);
                    return sb.ToString().Trim();
                }
                else
                    return number;
            }
        }

        public virtual string CountryCode
		{
			get 
            {
                checkNeedForDisection();
                return countryCode; 
            }
			set { countryCode = value; }
		}

        public virtual string AreaCode
		{
			get 
            {
                checkNeedForDisection();
                return areaCode; 
            }
			set { areaCode = value; }
		}

        public virtual string LocalNumber
        {
            get 
            {
                checkNeedForDisection();
                return localNumber; 
            }
            set { localNumber = value; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return this.Number;
        }

        #endregion

        #region Helpers

        private void checkNeedForDisection()
        {
            if (this.countryCode == "" && this.areaCode == "" && this.localNumber == "")
                disectNumber();
        }

        private bool disectNumber()
        {
            bool retVal = false;
            try
            {
                if (this.number != null && this.number.Length > 0)
                {
                    // check valid (no alpha numeric)
                    Match matchAlphaNumeric = Regex.Match(this.number, @"[a-z]|[A-Z]", RegexOptions.IgnoreCase);
                    if (matchAlphaNumeric.Success)
                        return false;
                    
                    string temp = this.number.Replace("(", "").Replace(")", "").Replace(" ", ""); ;
                    Match match = Regex.Match(this.number, @"^\+[0-9]{2}|^00[0-9]{2}|^\+[0-9]\-|^00[0-9]\-", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string countryCode = match.Groups[0].Value;

                        if (countryCode.Length > 0)
                        {
                            temp = temp.Substring(countryCode.Length);
                            if (countryCode.Substring(0, 1) == "+")
                                CountryCode = countryCode.Substring(1).Replace("-", "");
                            else
                                CountryCode = countryCode.Substring(2).Replace("-", "");
                        }
                    }

                    if (temp.Length > 1)
                    {
                        if (temp.Contains("-"))
                        {
                            AreaCode = temp.Substring(0, temp.IndexOf("-"));
                            // if no 0 at start add it
                            if (CountryCode != null && CountryCode == "31")
                            {
                                if (AreaCode.Substring(0, 1) != "0")
                                    AreaCode = "0" + AreaCode;
                            }
                            LocalNumber = temp.Substring(temp.IndexOf("-") + 1);
                        }
                        else
                        {
                            LocalNumber = temp;
                            if (CountryCode != null && CountryCode == "31")
                            {
                                if (LocalNumber.Substring(0, 1) != "0")
                                    LocalNumber = "0" + LocalNumber;
                            }
                        }
                        retVal = true;
                    }
                }
                return retVal;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Privates

        private string number;
		private string countryCode = "";
        private string areaCode = "";
        private string localNumber = "";

        #endregion

	}
}
