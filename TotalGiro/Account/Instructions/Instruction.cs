using System;
using System.Collections;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This is a instruction used for any kind of process involving orders & cash.
    /// </summary>
    public abstract class Instruction : TotalGiroBase<IInstruction>, IInstruction, IStateMachineClient
	{
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">Instruction</see> class.
        /// </summary>
        protected Instruction() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">Instruction</see> class.
        /// </summary>
        /// <param name="account">The account the withdrawal will belong to</param>
        /// <param name="executionDate">The date the instruction should execute</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        internal Instruction(IAccountTypeCustomer account, DateTime executionDate, bool doNotChargeCommission)
		{
            if (account == null)
                throw new ApplicationException("You can not enter a instruction without an account.");

            if (executionDate.Date < DateTime.Now.Date)
                throw new ApplicationException(string.Format("You can not enter {0} instructions in the past ({1}).", InstructionType.ToString(), ExecutionDate.ToShortDateString()));

            this.account = account;
            this.ExecutionDate = executionDate;
            this.DoNotChargeCommission = doNotChargeCommission;
            this.creationDate = DateTime.Now;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The account the instruction belongs to.
        /// </summary>
        public virtual IAccountTypeCustomer Account
		{
			get { return this.account; }
			internal set { this.account = value; }
		}

        /// <summary>
        /// Is this instruction still active.
        /// </summary>
        public virtual bool IsActive
		{
            get { return this.isActive; }
            set { this.isActive = value; }
		}

        /// <summary>
        /// Was this instruction cancelled.
        /// </summary>
        public virtual bool Cancelled
        {
            get { return this.cancelled; }
            set { this.cancelled = value; }
        }

        /// <summary>
        /// The type of instruction.
        /// </summary>
        public abstract InstructionTypes InstructionType { get; }

        /// <summary>
        /// Is this instruction a type of rebalance.
        /// </summary>
        public abstract bool IsTypeRebalance { get; }

        /// <exclude/>
        public virtual string DisplayInstructionType
        {
            get { return InstructionType.ToString(); }
        }

        /// <summary>
        /// Should the instruction be without any commission charges
        /// </summary>
        public virtual bool DoNotChargeCommission
        {
            get { return doNotChargeCommission; }
            internal set { doNotChargeCommission = value; }
        }

        /// <summary>
        /// The status of the instruction
        /// </summary>
        public abstract int Status { get; set; }

        /// <exclude/>
        public abstract string DisplayStatus  { get; }

        /// <exclude/>
        public virtual bool HasOrders
        {
            get { return (OrdersGenerated > 0); }
        }

        public virtual bool Warning { get; set; }

        /// <exclude/>
        public abstract bool IsEditable { get; }

        /// <summary>
        /// The date that the instruction should execute.
        /// </summary>
		public virtual DateTime ExecutionDate
		{
			get { return this.executionDate; }
			set { this.executionDate = value.Date; }
		}

        /// <summary>
        /// The date that the rebalance was done (can be historical)
        /// </summary>
        public virtual DateTime ActualExecutedDate
        {
            get { return actualExecutedDate; }
            set { actualExecutedDate = value; }
        }

        /// <summary>
        /// The date that the instruction is closed
        /// </summary>
        public virtual DateTime CloseDate
        {
            get { return closeDate; }
            set { closeDate = value; }
        }

        /// <summary>
        /// The date that the instruction was created.
        /// </summary>
		public virtual DateTime CreationDate
		{
			get { return this.creationDate; }
			internal set { this.creationDate = value; }
		}

        /// <summary>
        /// The date that the instruction was last modified.
        /// </summary>
        public virtual DateTime LastUpdated 
		{
			get { return this.lastUpdated; }
		}

        /// <summary>
        /// A descriptive message on the instruction
        /// </summary>
		public virtual string Message
		{
			get { return this.message; }
			set { this.message = value; }
		}

        /// <summary>
        /// The orders that originated from this instruction
        /// </summary>
        public virtual IInstructionOrderCollection Orders
        {
            get
            {
                IInstructionOrderCollection col = (IInstructionOrderCollection)orders.AsList();
                if (col.ParentInstruction == null) col.ParentInstruction = this;
                return col;
            }
        }

        /// <summary>
        /// The active orders that originated from this instruction
        /// </summary>
        public virtual IInstructionOrderCollection ActiveOrders
        {
            get
            {
                return Orders.NewCollection(x => !x.IsClosed);
                //IList activeOrders = new ArrayList();
                //foreach (IOrder order in Orders)
                //{
                //    if (!order.IsClosed)
                //        activeOrders.Add(order);
                //}
                //return new AccountTypeInternal.OrderCollection(this.Account, activeOrders);
            }
        }

        /// <summary>
        /// Calculated database field how many orders wwere generated for this instruction.
        /// </summary>
        public virtual int OrdersGenerated
        {
            get { return this.ordersGenerated; }
            private set { this.ordersGenerated = value; }
        }

        /// <summary>
        /// The engine that performs the actions
        /// </summary>
        public virtual IInstructionEngine Engine
        {
            get { return engine; }
            set { engine = value; }
        }

        /// <summary>
        /// The Orders that should be stored
        /// </summary>
        public virtual IList UpdateableOrders
        {
            get { return updateableOrders; }
            set { updateableOrders = value; }
        }

        #endregion

        #region Methods

        public abstract void SetInstructionMessage(int result);

        #endregion

        #region Actions

        protected virtual void terminateInstruction()
        {
            IsActive = false;
            CloseDate = DateTime.Now;
            Message = string.Format("The instruction was finished at {0}", CloseDate.ToString());
        }

        protected void cancelInstruction()
        {
            IsActive = false;
            CloseDate = DateTime.Now;
            Cancelled = true;
            Message = string.Format("The instruction was cancelled at {0}", CloseDate.ToString());
        }

        protected bool cancelAttachedOrders()
        {
            foreach (IOrder order in Orders)
            {
                if (!order.IsClosed)
                {
                    // No attached money orders
                    if (!(order.IsMonetary && ((IMonetaryOrder)this).MoneyParent != null))
                    {
                        // when one the order is not unapproveable -> it can no longer be cancelled
                        if (!order.Approved || order.IsCancellable)
                        {
                            // cancel the BMF
                            order.Cancel();
                            if (UpdateableOrders == null)
                                UpdateableOrders = new ArrayList();
                            UpdateableOrders.Add(order);
                        }
                    }
                }
            }

            if (this.InstructionType == InstructionTypes.BuyModel || this.InstructionType == InstructionTypes.Rebalance)
            {
                if (UpdateableOrders != null && Orders.Count == UpdateableOrders.Count)
                    ((IInstructionTypeRebalance)this).CashTransfers.DisConnect();
            }
            return true;
        }

        #endregion
	
		#region Private Variables

		private IAccountTypeCustomer account;
        private bool isActive = true;
        private bool cancelled = false;
        private DateTime executionDate;
        private DateTime actualExecutedDate;
        private DateTime closeDate;
        private DateTime creationDate;
		private DateTime lastUpdated;
		private string message;
        private bool doNotChargeCommission;
        //private IList bagOfOrders = new ArrayList();
        private int ordersGenerated;
        private IDomainCollection<IOrder> orders;
        private IInstructionEngine engine;
        private IList updateableOrders;

        protected enum CheckForCurPositionsReturnValue
        {
            NoCash,
            CashFound,
            ForeignCurrencyPositionsFound,
        }

		#endregion

        #region IStateMachineClient Members

        public abstract string CheckCondition(int conditionID);
        public abstract bool RunAction(int actionID);

        #endregion
    }
}
