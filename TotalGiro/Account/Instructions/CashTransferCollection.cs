using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions
{
    public class CashTransferCollection : TransientDomainCollection<IJournalEntryLine>, ICashTransferCollection
    {
        public CashTransferCollection()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">Instruction</see> class.
        /// </summary>
        /// <param name="parent">The instruction that owns the collection of transfers</param>
        public CashTransferCollection(IInstruction parent)
            : base()
        {
            Parent = parent;
        }

        /// <summary>
        /// The instruction that owns the collection of transfers
        /// </summary>
		public IInstruction Parent
		{
			get { return this.parent; }
			internal set 
            { 
                this.parent = value;
                IsInitialized = true;
            }
		}

        /// <summary>
        /// Returns the active instruction
        /// </summary>
        public Money TotalTransferAmount
        {
            get 
            {
                Money total = null;
                if (this.Count > 0)
                {
                    foreach (IJournalEntryLine transfer in this)
                    {
                        Money transferAmount = transfer.Credit;
                        if (transfer.GLAccount.CashTransferType == B4F.TotalGiro.GeneralLedger.Static.CashTransferTypes.TransferFee)
                            transferAmount = transfer.Debit.Negate();

                        if (transferAmount != null && transferAmount.IsNotZero)
                        {
                            if (transferAmount.Underlying.ToCurrency.IsBase)
                                total += transferAmount;
                            else
                                throw new ApplicationException("All transfers should be in base currency");
                        }
                    }
                }
                if (Parent.InstructionType == InstructionTypes.BuyModel)
                    total += ((IBuyModelInstruction)Parent).DepositCashPositionDifference;

                return total;
            }
        }

        public void AddTransfers(IList<IJournalEntryLine> transfers)
        {
            if (transfers != null &&  transfers.Count > 0)
            {
                foreach (IJournalEntryLine transfer in transfers)
                    Add(transfer);
            }
        }

        public void DisConnect()
        {
            // check if all orders have been cancelled
            if (this.SelectMany(x => x.Instruction.Orders).Any(x => x.CancelStatus != B4F.TotalGiro.Orders.OrderCancelStati.Cancelled))
                return;
            
            for (int i = this.Count; i > 0; i--)
            {
                IJournalEntryLine transfer = this[i - 1];
                transfer.Instruction = null;
                base.Remove(transfer);
            }
            base.Clear();
        }

		#region Overrides

        /// <summary>
		/// This method adds an transfer to the collection
		/// </summary>
		/// <param name="item">The transfer being added</param>
        public new void Add(IJournalEntryLine item)
		{
            if (item.Instruction != null)
                throw new ApplicationException(string.Format("This cash transfer {0} already has been rebalanced.", item.Key.ToString()));

            base.Add(item);
            if (IsInitialized)
                item.Instruction = Parent;
		}

		#endregion

        #region Private Variables

        private IInstruction parent;

        #endregion
    }
}
