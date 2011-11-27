using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections.Persistence;
using System.Text;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This is a instruction used for rebalancing or buying the model
    /// </summary>
    public abstract class InstructionTypeRebalance : Instruction, IInstructionTypeRebalance
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.InstructionTypeRebalance">InstructionTypeRebalance</see> class.
        /// </summary>
        protected InstructionTypeRebalance() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.InstructionTypeRebalance">InstructionTypeRebalance</see> class.
        /// </summary>
        /// <param name="account">The account the withdrawal will belong to</param>
        /// <param name="executionDate">The date the instruction should execute</param>
        /// <param name="orderActionType">The type of instruction that is placed on the orders</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        /// <param name="cashTransfers">The transfers involved</param>
        internal InstructionTypeRebalance(IAccountTypeCustomer account, DateTime executionDate, OrderActionTypes orderActionType, bool doNotChargeCommission, IList<IJournalEntryLine> cashTransfers)
            : base(account, executionDate, doNotChargeCommission)
        {
            this.OrderActionType = orderActionType;
            this.status = RebalanceInstructionStati.New;
            CashTransfers.AddTransfers(cashTransfers);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type that is put on the orders.
        /// Is important for the commission calculation.
        /// </summary>
        public virtual OrderActionTypes OrderActionType
        {
            get { return orderActionType; }
            set { orderActionType = value; }
        }

        /// <summary>
        /// The cash transfers that belong to this instruction
        /// </summary>
        public virtual ICashTransferCollection CashTransfers
        {
            get
            {
                CashTransferCollection cashTransfers = (CashTransferCollection)transfers.AsList();
                if (cashTransfers.Parent == null)
                    cashTransfers.Parent = this;
                return cashTransfers;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Is this instruction a type of rebalance.
        /// </summary>
        public override bool IsTypeRebalance
        {
            get { return true; }
        }

        public override int Status
        {
            get { return (int)status; }
            set { status = (RebalanceInstructionStati)value; }
        }

        /// <exclude/>
        public override string DisplayStatus
        {
            get
            {
                return Status.ToString() + "-" + status.ToString();
            }
        }

        /// <exclude/>
        public override bool IsEditable
        {
            get { return (status == RebalanceInstructionStati.New); }
        }

        public override void SetInstructionMessage(int result)
        {
            switch ((RebalanceResults)result)
            {
                case RebalanceResults.NoRebalanceNegativePortfolioAmount:
                    Message = "No rebalance was done since the total portfolio value is negative.";
                    break;
                case RebalanceResults.NoCashFundOrdersNegativePortfolioAmount:
                    Message = "No cash fund order was placed since the total portfolio value is negative.";
                    break;
                case RebalanceResults.RebalanceWasNotNeeded:
                    terminateInstruction();
                    Message = "The rebalance was not needed.";
                    break;
                default:
                    // nothing
                    break;
            }
        }

        #endregion

        #region Methods

        public bool Edit(OrderActionTypes orderActionType, DateTime executionDate, bool doNotChargeCommission)
        {
            if (!IsEditable)
                throw new ApplicationException("This instruction is not editable");

            this.OrderActionType = orderActionType;
            this.ExecutionDate = executionDate;
            this.DoNotChargeCommission = doNotChargeCommission;
            return true;
        }

        #endregion

        #region Private Variables

        private OrderActionTypes orderActionType;
        protected RebalanceInstructionStati status;
        private IDomainCollection<IJournalEntryLine> transfers = new CashTransferCollection();

        #endregion
    
    }
}
