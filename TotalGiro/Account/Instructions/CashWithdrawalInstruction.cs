using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.BackOffice.Orders;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This is a instruction used for cash withdrawal
    /// </summary>
    public class CashWithdrawalInstruction : Instruction, ICashWithdrawalInstruction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">Instruction</see> class.
        /// </summary>
        protected CashWithdrawalInstruction() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.CashWithdrawalInstruction">CashWithdrawalInstruction</see> class.
        /// </summary>
        /// <param name="account">The account the withdrawal will belong to</param>
        /// <param name="executionDate">The day from which the instruction should be taken into account</param>
        /// <param name="withdrawalDate">The day that the money should be withdrawn</param>
        /// <param name="amount">The relevant amount</param>
        /// <param name="counterAccount">The account the money should go to</param>
        /// <param name="rule">The rule that is responsible for this periodic instruction</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        internal CashWithdrawalInstruction(IAccountTypeCustomer account, DateTime executionDate, DateTime withdrawalDate, Money amount,
                ICounterAccount counterAccount, IWithdrawalRule rule, string transferDescription, bool doNotChargeCommission)
            : base(account, executionDate, doNotChargeCommission)
		{
            this.Account = account;
            this.WithdrawalDate = withdrawalDate;
            this.Amount = amount;
            this.CounterAccount = counterAccount;
            this.Rule = rule;
            this.TransferDescription = transferDescription;
            this.status = CashWithdrawalInstructionStati.New;
            if (rule != null)
                this.IsPeriodic = true;
        }

        #endregion

        #region Properties

        public virtual DateTime WithdrawalDate
        {
            get { return withdrawalDate; }
            set { withdrawalDate = value; }
        }

        public DateTime LatestPossibleRebalanceStartDate 
        {
            get
            {
                DateTime retVal = DateTime.MinValue;
                if (modelDetail() != null)
                    retVal = Util.DateAdd(DateInterval.BusinessDay, modelDetail().DaysDurationRebalance * -1, WithdrawalDate, DateIntervalOptions.None, null);
                return retVal;
            }
        }

        public DateTime LatestPossibleFreeUpCashDate
        {
            get
            {
                DateTime retVal = DateTime.MinValue;
                if (modelDetail() != null)
                {
                    if (Account.ModelPortfolio.Details.IncludeCashManagementFund)
                        retVal = Util.DateAdd(DateInterval.BusinessDay, modelDetail().DaysFreeUpCash * -1, WithdrawalDate, DateIntervalOptions.None, null);
                }
                return retVal;
            }
        }

        private IModelDetail modelDetail()
        {
            IModelDetail detail = null;
            if (Account != null && Account.ModelPortfolio != null)
                detail = Account.ModelPortfolio.Details;
            return detail;
        }

        public virtual Money Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public virtual bool IsPeriodic
        {
            get { return isPeriodic; }
            set { isPeriodic = value; }
        }

        public virtual string DisplayRegularity
        {
            get 
            {
                if (Rule != null)
                {
                    if (Rule.Regularity != null)
                        return Rule.Regularity.Description;
                    else
                        return "Unknown";
                }
                else
                    return "Ad hoc";
            }
        }

        public virtual ICounterAccount CounterAccount
        {
            get 
            {
                if (counterAccount == null)
                {
                    if (Rule != null && Rule.CounterAccount != null)
                        return Rule.CounterAccount;
                    else
                        return Account.CounterAccount;
                }
                else
                    return counterAccount;
            }
            set 
            { 
                if (value == null && (Account == null || Account.CounterAccount == null))
                    throw new ApplicationException(string.Format("The account {0} does not have a default counter account.", Account.DisplayNumberWithName));
                
                counterAccount = value; 
            }
        }

        public virtual IMoneyTransferOrder MoneyTransferOrder 
        {
            get { return moneyTransferOrder; }
            set { moneyTransferOrder = value; }
        }

        public virtual string Reference
        {
            get 
            {
                if ((reference == null || reference == "") && MoneyTransferOrder != null)
                    reference = MoneyTransferOrder.Reference;
                return reference; 
            }
            set { reference = value; }
        }

        public virtual string TransferDescription 
        {
            get 
            {
                if (string.IsNullOrEmpty(this.transferDescription) && rule != null)
                    return rule.TransferDescription;
                else
                    return this.transferDescription; 
            }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!(rule != null && rule.TransferDescription.Equals(value)))
                        this.transferDescription = value;
                }
            }
        }

        public virtual IWithdrawalRule Rule
        {
            get { return rule; }
            set { rule = value; }
        }

        public virtual bool IsCancellable
        {
            get { return IsActive; }
        }

        #endregion

        #region Methods

        public bool Edit(DateTime withdrawalDate, DateTime executionDate, Money amount, ICounterAccount counterAccount, string transferDescription, bool doNotChargeCommission)
        {
            if (!IsEditable)
                throw new ApplicationException("This instruction is not editable");

            this.WithdrawalDate = withdrawalDate;
            this.ExecutionDate = executionDate;
            if (this.Rule == null)
                this.Amount = amount;
            this.CounterAccount = counterAccount;
            this.TransferDescription = transferDescription;
            this.DoNotChargeCommission = doNotChargeCommission;

            Money portValue = Account.TotalAll + Account.ActiveWithdrawalInstructions.TotalAmount;
            if (portValue == null || portValue.IsLessThanZero)
                throw new ApplicationException("The total amount of withdrawals exceeds the totalportfolio value.");

            return true;
        }

        public bool Validate()
        {
            // The maximum amount that can be withdrawn
            Money maxWithdrawalAmount = Account.Portfolio.MaxWithdrawalAmount();
            if (maxWithdrawalAmount == null)
                maxWithdrawalAmount = new Money(0M, Account.AccountOwner.BaseCurrency);
            // also look at other withdrawals
            Money withdrwls = Account.ActiveWithdrawalInstructions.TotalAmount;
            if (Money.Add((maxWithdrawalAmount + withdrwls), Amount).IsLessThanZero)
            {
                string withdrwlsExcp = "";
                if (withdrwls != null && withdrwls.IsNotZero)
                    withdrwlsExcp = string.Format(" and future withdrawals ({0})", withdrwls.DisplayString);
                throw new ApplicationException(string.Format("The withdrawal of {0} for account {1} exceeds the maximum allowed portfolio withdrawal amount ({2}){3}.", 
                    Amount.DisplayString, 
                    Account.DisplayNumberWithName,
                    maxWithdrawalAmount.DisplayString, 
                    withdrwlsExcp));
            }
            else
                return true;
        }


        #endregion

        #region overrides

        public override int Status
        {
            get { return (int)status; }
            set { status = (CashWithdrawalInstructionStati)value; }
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
            get { return (status == CashWithdrawalInstructionStati.New); }
        }

        /// <summary>
        /// The type of instruction.
        /// </summary>
        public override InstructionTypes InstructionType
        {
            get { return InstructionTypes.CashWithdrawal; }
        }

        public override bool IsTypeRebalance
        {
            get { return false; }
        }

        public override void SetInstructionMessage(int result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        public override string ToString()
        {
            return "CashWithdrawalInstruction for " + Account.ToString() + " -> status: " + Status.ToString() + ": ID " + " status: " + Key.ToString();
        }

        #endregion

        #region Private Variables

        private CashWithdrawalInstructionStati status;
        private DateTime withdrawalDate;
        private Money amount;
        private bool isPeriodic;
        private ICounterAccount counterAccount;
        private IMoneyTransferOrder moneyTransferOrder;
        private string reference;
        private IWithdrawalRule rule;
        private string transferDescription;

        #endregion

        #region IStateMachineClient Members

        public override string CheckCondition(int conditionID)
        {
            string retVal = "None";
            CashWithdrawalInstructionConditions condition = (CashWithdrawalInstructionConditions)conditionID;

            switch (condition)
            {
                case CashWithdrawalInstructionConditions.CheckPendingCashOrder:
                    switch (checkPendingMoneyTransferOrder())
                    {
                        case MoneyTransferOrderStati.Terminated:
                            retVal = "OK";
                            break;
                        case MoneyTransferOrderStati.Cancelled:
                            retVal = "CANCEL";
                            break;
                    }
                    break;
                case CashWithdrawalInstructionConditions.CheckCancel:
                    if (checkCancel())
                        retVal = "OK";
                    break;
                default: //CashWithdrawalInstructionConditions.CheckAction
                    Message = "";
                    Warning = false;
                    switch (checkAction())
                    {
                        case CashWithdrawalInstructionCheckCashReturnValues.PlaceCashOrder:
                            retVal = "WD";
                            break;
                        case CashWithdrawalInstructionCheckCashReturnValues.PlaceCashFundOrder:
                            retVal = "CF";
                            break;
                        case CashWithdrawalInstructionCheckCashReturnValues.CreateRebalanceInstruction:
                            retVal = "RB";
                            break;
                        case CashWithdrawalInstructionCheckCashReturnValues.NotEnoughBuyingPower:
                            Message = string.Format("The withdrawal amount ({0}) exceeds the account's total portfolio amount ({1})", Amount.DisplayString, Account.TotalAll.DisplayString);
                            Warning = true;
                            break;
                        default: // DoNothing
                            if (Message == "")
                                Message = string.Format("No action needed {0}", DateTime.Now.ToString());
                            break;
                    }
                    break;
            }
            return retVal;
        }

        public override bool RunAction(int actionID)
        {
            switch ((CashWithdrawalInstructionActions)actionID)
            {
                case CashWithdrawalInstructionActions.PlaceFreeUpCashFundOrder:
                    Engine.PerformAction(InstructionEngineActions.PlaceFreeUpCashFundOrder, this);
                    break;
                case CashWithdrawalInstructionActions.CreateFreeUpCashRebalanceInstruction:
                    Engine.PerformAction(InstructionEngineActions.CreateFreeUpCashRebalanceInstruction, this);
                    break;
                case CashWithdrawalInstructionActions.CreateCashOrder:
                    Engine.PerformAction(InstructionEngineActions.CreateMoneyTransferOrder, this);
                    break;
                case CashWithdrawalInstructionActions.Terminate:
                    terminateInstruction();
                    break;
                case CashWithdrawalInstructionActions.Cancel:
                    cancelInstruction();
                    break;
                default:
                    break;
            }
            return true;
        }

        #endregion

        #region Conditions

        private CashWithdrawalInstructionCheckCashReturnValues checkAction()
        {
            CashWithdrawalInstructionCheckCashReturnValues retVal = CashWithdrawalInstructionCheckCashReturnValues.DoNothing;

            // Check the date
            if (DateTime.Today >= WithdrawalDate)
            {
                if (Money.Add(Account.TotalCash, Amount).IsGreaterThanOrEqualToZero)
                    return CashWithdrawalInstructionCheckCashReturnValues.PlaceCashOrder;
                else if (Money.Add(Account.TotalBothCash, Amount).IsGreaterThanOrEqualToZero)
                {
                    if (Account.OpenOrdersForAccount.Count > 0 || (
                        Account.AccountType == AccountTypes.Customer && ((ICustomerAccount)Account).ActiveRebalanceInstructions != null && ((ICustomerAccount)Account).ActiveRebalanceInstructions.Count > 0))
                        return CashWithdrawalInstructionCheckCashReturnValues.DoNothing;
                    else
                        return CashWithdrawalInstructionCheckCashReturnValues.PlaceCashFundOrder;
                }
                else if (!Money.Add(Account.TotalAll, Amount).IsGreaterThanOrEqualToZero)
                {
                    Message = string.Format("The withdrawal amount ({0}) exceeds the account's total portfolio amount ({1})", Amount.DisplayString, Account.TotalAll.DisplayString);
                    Warning = true;
                }
            }
            if (LatestPossibleFreeUpCashDate != DateTime.MinValue && DateTime.Today >= LatestPossibleFreeUpCashDate)
                return CheckAccountCash(CashWithdrawalInstructionActions.PlaceFreeUpCashFundOrder);
            else if (DateTime.Today >= LatestPossibleRebalanceStartDate)
                return CheckAccountCash(CashWithdrawalInstructionActions.CreateFreeUpCashRebalanceInstruction);
            return retVal;
        }

        public CashWithdrawalInstructionCheckCashReturnValues CheckAccountCash(CashWithdrawalInstructionActions action)
        {
            CashWithdrawalInstructionCheckCashReturnValues retVal = CashWithdrawalInstructionCheckCashReturnValues.DoNothing;

            if (!(Money.Add(Account.TotalCash, Amount).IsGreaterThanOrEqualToZero && Money.Add(Account.TotalBothCash, Amount).IsGreaterThanOrEqualToZero))
            {
                if (Money.Add(Account.TotalBothCash, Amount).IsGreaterThanOrEqualToZero || Money.Add(Account.TotalCashFund, Amount).IsGreaterThanOrEqualToZero)
                {
                    // No Rebalance needed
                    if (action == CashWithdrawalInstructionActions.PlaceFreeUpCashFundOrder)
                    {
                        // check if cash fund order already created
                        bool check = true;
                        if (Account.OpenOrdersForAccount != null && Account.OpenOrdersForAccount.Count > 0)
                        {
                            foreach (IOrder order in Account.OpenOrdersForAccount)
                            {
                                if (order.Status == OrderStati.New)
                                    check = false;
                            }

                        }
                        if (check)
                            retVal = CashWithdrawalInstructionCheckCashReturnValues.PlaceCashFundOrder;
                    }
                }
                else if (Money.Add(Account.TotalAll, Amount).IsGreaterThanOrEqualToZero)
                    switch (action)
                    {
                        case CashWithdrawalInstructionActions.CreateFreeUpCashRebalanceInstruction:
                        case CashWithdrawalInstructionActions.PlaceFreeUpCashFundOrder:

                            if (Account.ActiveRebalanceInstructions != null && Account.ActiveRebalanceInstructions.Count > 0)
                            {
                                foreach (IInstructionTypeRebalance ib in Account.ActiveRebalanceInstructions)
                                {
                                    if (ib.IsActive)
                                    {
                                        if (ib.InstructionType == InstructionTypes.Rebalance && ((IRebalanceInstruction)ib).CashWithdrawalInstruction != null && ((IRebalanceInstruction)ib).CashWithdrawalInstruction.Key.Equals(Key))
                                            Message = string.Format("This withdrawal instruction already created a rebalance instruction {0} with status {1}", ib.Key.ToString(), ib.DisplayStatus);
                                        else
                                        {
                                            Message = string.Format("The account needs to rebalanced, but a instruction {0} with status {1} already exists. Cancel this rebalance instruction and process this withdrawal instruction again.", ib.Key.ToString(), ib.DisplayStatus);
                                            Warning = true;
                                        }
                                    }
                                }
                            }
                            else
                                retVal = CashWithdrawalInstructionCheckCashReturnValues.CreateRebalanceInstruction;
                            break;
                    }
                else
                    retVal = CashWithdrawalInstructionCheckCashReturnValues.NotEnoughBuyingPower;
            }
            return retVal;
        }

        private MoneyTransferOrderStati checkPendingMoneyTransferOrder()
        {
            MoneyTransferOrderStati retVal = MoneyTransferOrderStati.New;

            if (MoneyTransferOrder != null)
            {
                return MoneyTransferOrder.Status;
            }
            return retVal;
        }

        private bool checkCancel()
        {
            cancelAttachedOrders();
            return true;
        }


        #endregion

    }
}
