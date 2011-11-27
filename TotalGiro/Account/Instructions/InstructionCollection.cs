using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This class is used to hold the collection of account instructions
    /// </summary>
    /// <moduleiscollection/>
    public class InstructionCollection : GenericCollection<IInstruction>, IInstructionCollection
	{
        private InstructionCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.InstructionCollection">InstructionCollection</see> class.
        /// </summary>
        /// <param name="parentAccount">The account that owns the collection of instructions</param>
        /// <param name="bagOfInstructions">The hibernate collection of instructions</param>
        internal InstructionCollection(IAccountTypeCustomer parentAccount, IList bagOfInstructions)
            : base(bagOfInstructions)
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
        /// Returns the active instruction
        /// </summary>
        public IInstruction GetActiveInstruction
        {
            get 
            {
                // search for the first active instruction
                if (this.Count > 0)
                {
                    foreach (IInstruction instruct in this.SortedByDefault())
                    {
                        if (instruct.IsActive && instruct.ExecutionDate.Date <= DateTime.Now.Date)
                            return instruct;
                    }
                }
                return null;
            }
        }

		#region Overrides

        protected override object GetDefaultSortValue(IInstruction instruction)
        {
            return instruction.ExecutionDate;
        }

        /// <summary>
		/// This method adds an instruction to the collection, but only when no active instructions exist for the account.
		/// </summary>
		/// <param name="item">The instruction being added</param>
        public override void Add(IInstruction item)
		{
            Instruction instruction = (Instruction)item;
            
            if (instruction.IsTypeRebalance)
            {
                // check if not another active rebalance type instruction exists
                foreach (IInstruction instruct in this)
                {
                    if (instruct.IsActive && instruct.IsTypeRebalance)
                        throw new ApplicationException(string.Format("A {0} instruction already exists for account {1}.", instruct.InstructionType.ToString(), ParentAccount.ShortName));
                }
            }
            else if (instruction.InstructionType == InstructionTypes.CashWithdrawal)
            {
                ICashWithdrawalInstruction withdrawal = (ICashWithdrawalInstruction)item;
                if (withdrawal.WithdrawalDate.AddDays(-7) < DateTime.Today)
                {
                    // check if an active rebalance type instruction exists within the 
                    foreach (IInstruction instruct in this)
                    {
                        if (instruct.IsActive && instruct.IsTypeRebalance && instruct.Status > 1)
                            throw new ApplicationException(string.Format("An active {0} instruction (ID: {1}) already exists for account {2}. Cancel this instruction first and then add the withdrawal instruction.", instruct.InstructionType.ToString(), instruct.Key, ParentAccount.ShortName));
                    }
                }
            }
            
            base.Add(item);
            instruction.Account = ParentAccount;
		}

		#endregion

        #region Private Variables

        private IAccountTypeCustomer parentAccount;

        #endregion
	}
}
