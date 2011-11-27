using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using System.Collections;

namespace B4F.TotalGiro.Accounts.Withdrawals
{
    public class WithdrawalRuleCollection : GenericCollection<IWithdrawalRule>, IWithdrawalRuleCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Withdrawals.Withdrawal">Withdrawal</see> class.
        /// </summary>
        /// <param name="parentAccount">The account that owns the collection of instructions</param>
        /// <param name="bagOfInstructions">The hibernate collection of Withdrawals</param>
        internal WithdrawalRuleCollection(ICustomerAccount parentAccount, IList bagOfWithdrawals)
            : base(bagOfWithdrawals)
		{
			this.parentAccount = parentAccount;
		}

        /// <summary>
        /// The account that owns the collection of Withdrawals
        /// </summary>
        public ICustomerAccount ParentAccount
		{
			get { return this.parentAccount; }
			private set { this.parentAccount = value; }
		}

        #region Overrides

        /// <summary>
        /// This method adds an Withdrawal to the collection
        /// </summary>
        /// <param name="item">The Withdrawal being added</param>
        public override void Add(IWithdrawalRule item)
        {
            if (Contains(item))
                throw new ApplicationException("This rule already exists");
            
            base.Add(item);
            item.Account = ParentAccount;

            if (item.Account.CounterAccount == null && item.CounterAccount == null)
                throw new ApplicationException("A counter account is mandatory.");
        }

        #endregion

        #region Private Variables

        private ICustomerAccount parentAccount;

        #endregion
    }
}
