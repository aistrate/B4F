using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public class DepositWithdrawal : IDepositWithdrawal
    {
        /// <summary>
        /// The Key
        /// </summary>
        public virtual int Key
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
        /// The amount deposited on this date. In instrument currency
        /// </summary>
        public virtual Money Deposit
        {
            get { return this.deposit; }
        }

        /// <summary>
        /// The amount withdrawn on this date. In instrument currency
        /// </summary>
        public virtual Money WithDrawal
        {
            get { return this.withDrawal; }
        }

        #region Privates

        private int key;
        private IAccountTypeInternal account;
        private DateTime date;
        private Money deposit;
        private Money withDrawal;

        #endregion

    }
}
