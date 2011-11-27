using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationTotalPortfolio : IValuationTotalPortfolio
    {
        /// <summary>
        /// The Key of the valuation. It is a description of the account + date
        /// </summary>
        public virtual string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return account; }
        }

        /// <summary>
        /// The date of the valuation
        /// </summary>
        public virtual DateTime Date
        {
            get { return this.date; }
        }

        /// <summary>
        /// This is the Total Value of the portfolio on the specific day in base currency.
        /// </summary>
        public virtual Money TotalValue
        {
            get { return totalValue; }
        }

        #region Privates

        private string key;
        private IAccountTypeInternal account;
        private DateTime date;
        private Money totalValue;

        #endregion
    }
}
