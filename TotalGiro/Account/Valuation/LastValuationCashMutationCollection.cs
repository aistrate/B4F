using System;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Valuations
{
    /// <summary>
    /// This class is used to hold the collection of LastValuationCashMutations
    /// </summary>
    /// <moduleiscollection/>
    public class LastValuationCashMutationCollection : GenericDictionary<ValuationCashMutationKey, ILastValuationCashMutationHolder>, ILastValuationCashMutationCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Valuations.LastValuationCashMutationCollection">LastValuationCashMutationCollection</see> class.
        /// </summary>
        /// <param name="account">The account that owns these mutations</param>
        /// <param name="bagOfCashMutations">The relevant cash mutations</param>
        internal LastValuationCashMutationCollection(IAccountTypeCustomer account, IList bagOfCashMutations)
            : base(bagOfCashMutations, "CashMutKey")
		{
            this.account = account;
		}

        /// <summary>
        /// The account that holds these Last ValuationCashMutations
        /// </summary>
        public IAccountTypeCustomer Account
        {
            get { return account; }
        }

		#region Methods

        /// <summary>
        /// Add a new CashMutationHolder to the collection.
        /// </summary>
        /// <param name="key">The unique key of the CashMutation</param>
        /// <param name="lastCashMutation">The relevant cash mutation</param>
        public void Add(ValuationCashMutationKey key, IValuationCashMutation lastCashMutation)
		{
            LastValuationCashMutationHolder holder = new LastValuationCashMutationHolder(key, lastCashMutation);
            Add(holder.CashMutKey, holder);
        }

		#endregion

		#region Privates

        private IAccountTypeCustomer account;

		#endregion
    }
}
