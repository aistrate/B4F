using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This class is used to hold the collection of account CashWithdrawal instructions
    /// </summary>
    /// <moduleiscollection/>
    public class CashWithdrawalInstructionCollection : GenericCollection<ICashWithdrawalInstruction>, ICashWithdrawalInstructionCollection
    {
        private CashWithdrawalInstructionCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.CashWithdrawalInstructionCollection">CashWithdrawalInstructionCollection</see> class.
        /// </summary>
        /// <param name="parentAccount">The account that owns the collection of instructions</param>
        /// <param name="bagOfWithdrawalInstructions">The hibernate collection of cash withdrawal instructions</param>
        internal CashWithdrawalInstructionCollection(IAccountTypeCustomer parentAccount, IList bagOfWithdrawalInstructions)
            : base(bagOfWithdrawalInstructions)
		{
			this.parentAccount = parentAccount;
		}

        /// <summary>
        /// The account that owns the collection of instructions
        /// </summary>
		public IAccountTypeCustomer ParentAccount
		{
			get { return this.parentAccount; }
			private set { this.parentAccount = value; }
		}

        /// <summary>
        /// The total amount that needs to be withdrawed.
        /// </summary>
        public Money TotalAmount
        {
            get
            {
                Money amount = null;
                foreach (ICashWithdrawalInstruction instruction in this)
                    amount += instruction.Amount;
                return amount;
            }
        }

        /// <summary>
        /// The First WithdrawalDate in the collection.
        /// </summary>
        public DateTime FirstWithdrawalDate
        {
            get
            {
                DateTime retVal = DateTime.MaxValue;
                foreach (ICashWithdrawalInstruction instruction in this)
                {
                    if (instruction.WithdrawalDate < retVal)
                        retVal = instruction.WithdrawalDate;
                }
                return (retVal < DateTime.MaxValue ? retVal : DateTime.MinValue);
            }
        }

        /// <summary>
        /// The total amount that needs to be kept in Cash
        /// </summary>
        /// <param name="withdrawalInstruction">The withdrawal instruction that caused the rebalance. This withdrawal amount should be kept in cash anyway</param>
        /// <returns></returns>
        public Money TotalKeepCashAmount(ICashWithdrawalInstruction withdrawalInstruction)
        {
            if (ParentAccount.ModelPortfolio != null && ParentAccount.ModelPortfolio.Details != null &&
                ParentAccount.ModelPortfolio.Details.IncludeCashManagementFund)
            {
                Money amount = null;
                int keepFreeCashDays = ParentAccount.ModelPortfolio.Details.DaysKeepFreeCash;

                foreach (ICashWithdrawalInstruction instruction in this)
                {
                    if ((withdrawalInstruction != null && instruction.Key == withdrawalInstruction.Key) ||
                        instruction.WithdrawalDate.AddDays(keepFreeCashDays * -1) <= DateTime.Today)
                        amount += instruction.Amount;
                }
                return amount;
            }
            else
                return TotalAmount;
        }

        public bool Contains(DateTime date)
        {
            foreach (ICashWithdrawalInstruction withDrawal in this)
            {
                if (withDrawal.WithdrawalDate.Equals(date))
                    return true;
            }
            return false;
        }

        public int[] GetKeys()
        {
            int[] keys = null;
            if (Count > 0)
            {
                keys = new int[Count];
                int i = 0;

                foreach (ICashWithdrawalInstruction instruction in this)
                {
                    keys[i] = instruction.Key;
                    i++;
                }
            }
            return keys;
        }

		#region Overrides

        protected override object GetDefaultSortValue(ICashWithdrawalInstruction instruction)
        {
            return instruction.WithdrawalDate;
        }

		#endregion

        #region Private Variables

        private IAccountTypeCustomer parentAccount;

        #endregion
    }
}
