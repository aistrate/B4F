using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.Currencies
{
    /// <summary>
    /// Structure to hold exchange rate/ date combination.
    /// </summary>
    public struct exRateDate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rateDate">Date of the currency exchange rate</param>
        /// <param name="rateCurrency">Currency</param>
        /// <param name="rate">exchange rate</param>
        public exRateDate(DateTime rateDate, string rateCurrency)
        {
            this.rateDate = rateDate;
            this.rateCurrency = rateCurrency;
            this.rate = 0M;
        }

        public decimal rate;
        public DateTime rateDate;
        public string rateCurrency;

        /// <summary>
        /// Overridden creation of a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return new Random().Next();
        }

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="obj">exRateDate object to compare to</param>
        /// <returns>true if equal, false if not equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is exRateDate)
            {
                exRateDate newobj = (exRateDate)obj;
                return ((this.rateCurrency == newobj.rateCurrency) && (this.rateDate == newobj.rateDate));
            }
            else return false;
        }
    }
}
