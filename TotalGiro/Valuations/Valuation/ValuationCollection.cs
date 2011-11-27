using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationCollection : GenericCollection<IValuation>
    {
        internal ValuationCollection() { }

        internal ValuationCollection(IList bagOfValuations)
            : base(bagOfValuations)
		{
            // check whether all valuations are from the same account
            foreach (IValuation valuation in bagOfValuations)
                checkConsistency(valuation);
		}

        private void checkConsistency(IValuation item)
        {
            if (this.account == null)
                this.account = item.Account;
            else if (!this.account.Equals(item.Account))
                throw new ApplicationException("All valuations should be from the same account.");

            if (Util.IsNullDate(this.date))
                this.date = item.Date;
            else if (!this.date.Equals(item.Date))
                throw new ApplicationException("All valuations should be from the same date.");
        }

        /// <summary>
        /// The account
        /// </summary>
        public IAccountTypeInternal Account
        {
            get { return account; }
        }

        /// <summary>
        /// The date of the valuation
        /// </summary>
        public DateTime Date
        {
            get { return this.date; }
        }

        public Money TotalValue
        {
            get
            {
                Money total = null;
                foreach (IValuation valuation in this)
                    total += valuation.BaseMarketValue;
                return total;
            }
        }

        #region Overrides

        public override void Add(IValuation item)
        {
            checkConsistency(item);
            base.Add(item);
        }

        #endregion

        #region Privates

        private IAccountTypeInternal account;
        private DateTime date = DateTime.MinValue;

        #endregion

    }
}
