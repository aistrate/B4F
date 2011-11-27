using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections.Persistence;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Valuations;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Notifications;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This is an abstract class and a subclass of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeInternal">AccountTypeInternal</see> class.
    /// It serves as a base class for accounts that are customers in the TotalGiro system, so people who invest their money.
    /// </summary>
    public abstract class AccountTypeCustomer : AccountTypeInternal, IAccountTypeCustomer
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeCustomer">AccountTypeCustomer</see> class.
        /// </summary>
        public AccountTypeCustomer() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeCustomer">AccountTypeCustomer</see> class.
        /// </summary>
        /// <param name="number">The Account's number</param>
        /// <param name="shortName">Shortname of the account</param>
        /// <param name="accountOwner">The owner of the account</param>
        /// <param name="modelPortfolio">The model portfolio the account is tied to</param>
        public AccountTypeCustomer(string number, string shortName, IManagementCompany accountOwner, IPortfolioModel modelPortfolio)
            : base(number, shortName, accountOwner)
		{
			this.ModelPortfolio = modelPortfolio;
		}

		/// <summary>
		/// This is the lifecycle that is tied to the account.
        /// The account's model is based upon this cycle and the age of the primary accountholder.
		/// </summary>
        public virtual ILifecycle Lifecycle{ get; set; }

		/// <summary>
		/// This is the model portfolio that is tied to the account.
        /// The account's portfolio is based upon this model.
		/// </summary>
        public virtual IPortfolioModel ModelPortfolio
		{
			get { return modelPortfolio; }
			set { modelPortfolio = value; }
		}

        /// <summary>
        /// Is this customer an execution only customer
        /// </summary>
        public virtual bool IsExecOnlyCustomer
        {
            get { return isExecOnlyCustomer; }
            set { isExecOnlyCustomer = value; }
        }

        /// <summary>
        /// The name of the model portfolio
        /// </summary>
        public virtual string ModelPortfolioName
        {
            get { return (modelPortfolio != null ? modelPortfolio.ModelName : ""); }
        }

        /// <summary>
        /// ManagementPeriods attached to the Account
        /// </summary>
        public virtual IManagementPeriodCollection ManagementPeriods
        {
            get
            {
                IManagementPeriodCollection periods = null;
                if (this.AccountType == AccountTypes.Customer)
                {
                    periods = (IManagementPeriodCollection)managementPeriods.AsList();
                    if (periods.Parent == null)
                        periods.Parent = (ICustomerAccount)this;
                }
                return periods;
            }
        }

        public DateTime GetLatestManagementEndDate(ManagementTypes managementType)
        {
            DateTime endDate = DateTime.MinValue;
            foreach (IManagementPeriod period in ManagementPeriods)
            {
                if (period.ManagementType == managementType && Util.IsNotNullDate(period.EndDate))
                {
                    if (endDate < period.EndDate.Value)
                        endDate = period.EndDate.Value;
                }
            }
            return endDate;
        }

        public bool ReportToTax { get; set; }

        public IManagementPeriod CurrentManagementFeePeriod { get; set; }

        public IManagementPeriod CurrentKickBackFeePeriod 
        {
            get
            {
                IManagementPeriod period = null;
                if (ManagementPeriods != null && ManagementPeriods.Count > 0)
                {
                    var periods = (from a in ManagementPeriods.Filter(ManagementTypes.KickBack)
                                  where !a.EndDate.HasValue
                                  select a);
                    if (periods != null && periods.Count() > 0)
                        period = periods.First();
                }
                return period;
            }
        }

        public DateTime ManagementStartDate
        {
            get
            {
                DateTime retDate = DateTime.MinValue;
                if (CurrentManagementFeePeriod != null)
                    retDate = CurrentManagementFeePeriod.StartDate;
                return retDate;
            }
        }

        public DateTime ManagementEndDate
        {
            get
            {
                DateTime retDate = DateTime.MinValue;
                if (CurrentManagementFeePeriod != null && CurrentManagementFeePeriod.EndDate.HasValue)
                    retDate = CurrentManagementFeePeriod.EndDate.Value;
                return retDate;
            }
            set
            {
                if (CurrentManagementFeePeriod == null)
                    throw new ApplicationException("It is not possible to set the management end date when no management period exists.");
                CurrentManagementFeePeriod.EndDate = value;
            }
        }

        /// <summary>
        /// The date that the customer goes under management (first transaction)
        /// </summary>
        public virtual DateTime FirstManagementStartDate
        {
            get
            {
                if (firstManagementStartDate.HasValue)
                    return firstManagementStartDate.Value;
                else
                    return DateTime.MinValue;
            }
            set
            {
                if (CurrentManagementFeePeriod == null && Util.IsNotNullDate(value))
                    createManagementFeePeriod(value);
                else
                {
                    if (CurrentManagementFeePeriod.StartDate > value && ManagementPeriods.IsManagementPeriodEditable(ManagementTypes.ManagementFee))
                        CurrentManagementFeePeriod.StartDate = value;
                }
                checkKickBackManagementPeriod(value, null);
                this.firstManagementStartDate = value;
            }
        }

        #region Helper FirstManagementStartDate

        protected void createManagementFeePeriod(DateTime txDate)
        {
            if (CurrentManagementFeePeriod == null && UseManagementFee && TradeableStatus == Tradeability.Tradeable)
            {
                bool create = false;
                IModelHistory model = ModelPortfolioChanges.GetItemByDate(txDate);
                if (model != null && model.ModelPortfolio != null)
                {
                    IList<IFeeRule> feeRules = model.ModelPortfolio.FeeRules.Filter(ManagementTypes.ManagementFee, txDate);
                    create = checkFeeRulesRelevant(feeRules, txDate);
                }
                if (!create && AccountType == AccountTypes.Customer)
                {
                    IList<IFeeRule> feeRules = ((ICustomerAccount)this).FeeRules.Filter(ManagementTypes.ManagementFee, txDate);
                    create = checkFeeRulesRelevant(feeRules, txDate);
                }
                if (create)
                    ManagementPeriods.AddManagementPeriod(new ManagementPeriod(ManagementTypes.ManagementFee, txDate));
            }
        }

        protected bool checkFeeRulesRelevant(IList<IFeeRule> feeRules, DateTime txDate)
        {
            if (feeRules != null && feeRules.Count > 0)
            {
                foreach (IFeeRule rule in feeRules)
                {
                    if (rule.ChargeFeeForDate(txDate))
                        return true;
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// The date that the customer is no longer under management
        /// </summary>
        public virtual DateTime FinalManagementEndDate
        {
            get { 
                return 
                    this.finalManagementEndDate.HasValue ? this.finalManagementEndDate.Value : DateTime.MinValue; 
            }
            set 
            {
                checkKickBackManagementPeriod(null, value);
                this.finalManagementEndDate = value;
                if (Util.IsNotNullDate(value) && CurrentManagementFeePeriod != null)
                    CurrentManagementFeePeriod.EndDate = value;
            }
        }

        public virtual bool UseManagementFee
        {
            get { return this.useManagementFee; }
            set
            {
                if (this.useManagementFee != value)
                {
                    this.useManagementFee = value;
                    if (value)
                        createManagementFeePeriod(FirstManagementStartDate);
                    else
                    {
                        if (ManagementPeriods != null && ManagementPeriods.Count > 0)
                        {
                            for (int i = ManagementPeriods.Count; i > 0; i--)
                            {
                                IManagementPeriod period = ManagementPeriods[i - 1];
                                if (period.ManagementType == ManagementTypes.ManagementFee)
                                {
                                    if (period.ManagementPeriodUnits != null && period.ManagementPeriodUnits.Count > 0)
                                        throw new ApplicationException("The account can no longer be set to no management fee. Contact your system administrator.");
                                    else
                                    {
                                        if (CurrentManagementFeePeriod != null && CurrentManagementFeePeriod.Key == period.Key)
                                            CurrentManagementFeePeriod = null;
                                        ManagementPeriods.Remove(period);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual bool UseKickback
        {
            get { return this.useKickback; }
            set
            {
                if (this.useKickback != value)
                {
                    this.useKickback = value;
                    if (value)
                        checkKickBackManagementPeriod(FirstManagementStartDate, null);
                    else
                    {
                        if (ManagementPeriods != null && ManagementPeriods.Count > 0)
                        {
                            for (int i = ManagementPeriods.Count; i > 0; i--)
                            {
                                IManagementPeriod period = ManagementPeriods[i - 1];
                                if (period.ManagementType == ManagementTypes.KickBack)
                                {
                                    if (period.ManagementPeriodUnits != null && period.ManagementPeriodUnits.Count > 0)
                                        throw new ApplicationException("The account can no longer be set to no kickback. Contact your system administrator.");
                                    else
                                        ManagementPeriods.Remove(period);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void checkKickBackManagementPeriod(DateTime? startDate, DateTime? endDate)
        {
            if (this.UseKickback)
            {
                if (startDate.HasValue && Util.IsNotNullDate(startDate.Value))
                {
                    IList<IManagementPeriod> periods = ManagementPeriods.Filter(ManagementTypes.KickBack);
                    if (periods != null && periods.Count > 0)
                    {
                        if (periods.Count == 1 && periods[0].StartDate > startDate.Value && ManagementPeriods.IsManagementPeriodEditable(ManagementTypes.KickBack))
                            periods[0].StartDate = startDate.Value;
                    }
                    else
                        ManagementPeriods.AddManagementPeriod(new ManagementPeriod(ManagementTypes.KickBack, startDate.Value));
                }
                else if (endDate.HasValue)
                {
                    foreach (IManagementPeriod p in ManagementPeriods)
                    {
                        if (p.ManagementType == ManagementTypes.KickBack && Util.IsNullDate(p.EndDate))
                            p.EndDate = endDate.Value;
                    }
                }
            }
        }

        /// <summary>
        ///  The account who will pay the Exit Fee (Eindafrekening managementfee)
        /// </summary>
        public virtual IAccountTypeCustomer ExitFeePayingAccount
        {
            get 
            {
                if (this.exitFeePayingAccount != null)
                    return this.exitFeePayingAccount;
                else
                    return this;
            }
            set { this.exitFeePayingAccount = value; }
        }

        /// <summary>
        /// The Date that the last ValuationMutation job did run.
        /// It is not wise / possible to print reports after this date
        /// It is reset (to the earliest date) in sp TG_ResetValuations when storno's did happen in the past
        /// </summary>
        public virtual DateTime ValuationMutationValidityDate
        {
            get
            {
                if (valuationMutationValidityDate.HasValue)
                    return valuationMutationValidityDate.Value;
                else
                    return DateTime.MinValue;
            }
            set { this.valuationMutationValidityDate = value; }
        }

        /// <summary>
        /// Either the last date that the daily valuations were created for this account, or
        /// the date that the valuations are still valid (due to storno's)
        /// </summary>
        public virtual DateTime LastValuationDate
        {
            get
            {
                if (lastValuationDate.HasValue)
                    return lastValuationDate.Value;
                else
                    return DateTime.MinValue;
            }
            set { this.lastValuationDate = value; }
        }

        /// <summary>
        /// The date that the valuations are trustworthy and available
        /// </summary>
        public virtual DateTime ValuationValidityDate
        {
            get 
            { 
                if (LastValuationDate < ValuationMutationValidityDate)
                    return LastValuationDate; 
                else
                    return ValuationMutationValidityDate;
            }
        }

        /// <summary>
        /// The method to alter the Model portfolio
        /// </summary>
        /// <param name="newModelPortfolio">The new model portfolio</param>
        /// <param name="isExecOnlyCustomer">Is this an execution only customer</param>
        /// <param name="employerRelationship">The relationship that the account has to the company</param>
        /// <param name="employee">The employee who performs the change</param>
        /// <returns>true when successfull</returns>
        public bool SetModelPortfolio(IPortfolioModel newModelPortfolio, IInternalEmployeeLogin employee, DateTime changeDate)
        {
            AccountEmployerRelationship employerRelationship = AccountEmployerRelationship.None;
            if (this.AccountType == AccountTypes.Customer)
                employerRelationship = ((ICustomerAccount)this).EmployerRelationship;
            if (Util.IsNullDate(changeDate))
                changeDate = DateTime.Now;

            return SetModelPortfolio(Lifecycle, newModelPortfolio, this.IsExecOnlyCustomer, employerRelationship, employee, changeDate);
        }

        /// <summary>
        /// The method to alter the Model portfolio
        /// </summary>
        /// <param name="lifecycle">The current lifecycle</param>
        /// <param name="newModelPortfolio">The new model portfolio</param>
        /// <param name="isExecOnlyCustomer">Is this an execution only customer</param>
        /// <param name="employerRelationship">The relationship that the account has to the company</param>
        /// <param name="employee">The employee who performs the change</param>
        /// <param name="changeDate">The date of the change</param>
        /// <returns></returns>
        public bool SetModelPortfolio(ILifecycle lifecycle, IPortfolioModel newModelPortfolio,
            bool isExecOnlyCustomer, AccountEmployerRelationship employerRelationship,
            IInternalEmployeeLogin employee, DateTime changeDate)
        {
            ModelPortfolioChanges.Add(new ModelHistory.ModelHistory(this, lifecycle, newModelPortfolio, isExecOnlyCustomer, employerRelationship, employee, changeDate));
            IsExecOnlyCustomer = isExecOnlyCustomer;
            if (this.AccountType == AccountTypes.Customer)
                ((ICustomerAccount)this).EmployerRelationship = employerRelationship;
            ModelPortfolio = newModelPortfolio;

            if (CurrentManagementFeePeriod == null && Util.IsNotNullDate(FirstManagementStartDate) && Util.IsNullDate(FinalManagementEndDate))
                createManagementFeePeriod(changeDate);
            return true;
        }

        /// <summary>
        /// This is the collection of modelportfolio changes that belong to the account.
        /// The <see cref="T:B4F.TotalGiro.Accounts.ModelHistory.ModelHistoryCollection">modelportfolio changes</see> that belong to the current account.
        /// </summary>
        public virtual IModelHistoryCollection ModelPortfolioChanges
        {
            get
            {
                if (this.modelPortfolioChanges == null)
                    this.modelPortfolioChanges = new ModelHistoryCollection(this, bagOfModelHistoryItems);
                return modelPortfolioChanges;
            }
        }

        /// <summary>
        /// The account where the contact belongs to
        /// </summary>
        public virtual IAccountAccountHoldersCollection AccountHolders
        {
            get
            {
                if (this.accountHolders == null)
                    this.accountHolders = new AccountAccountHoldersCollection(this, bagOfAccountHolders);
                return accountHolders;
            }
            set { accountHolders = value; }
        }

        /// <summary>
        /// The account's PrimaryAccountHolder
        /// </summary>
        public virtual IAccountHolder PrimaryAccountHolder
        {
            get
            {
                if (AccountHolders != null && AccountHolders.Count > 0)
                    return AccountHolders.PrimaryAccountHolder;
                else
                    return null;
            }
        }

        public virtual IAccountHolder EnOfAccountHolder
        {
            get
            {
                if (AccountHolders != null && AccountHolders.Count > 1)
                    return AccountHolders.EnOfAccountHolder;
                else
                    return null;
            }
        }

        public virtual bool HasPrimaryAH
        {
            get { return PrimaryAccountHolder != null; }
        }

        public IContactSendingOptionCollection ContactSendingOptions
        {
            get
            {
                if (HasPrimaryAH)
                    return PrimaryAccountHolder.Contact.ContactSendingOptions;
                else
                    throw new ApplicationException(
                                    "Contact sending options not available because account doesn't have a primary account holder.");
            }
        }

        public bool NeedsSendByPost(SendableDocumentCategories documentCategory)
        {
            return ContactSendingOptions.GetValueOrDefault(documentCategory, SendingOptions.ByPost);
        }

        public bool SendByPost
        {
            get { return ContactSendingOptions.GetValueOrDefault(SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByPost); }
        }

        public bool SendByEmail
        {
            get { return ContactSendingOptions.GetValueOrDefault(SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByEmail); }
        }

        /// <summary>
        /// This is the collection of instructions that belong to the account.
        /// A <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">instruction</see> is involved in several processes like for instance a rebalance.
        /// </summary>
		public virtual IInstructionCollection AccountInstructions
		{
			get
			{
				if (this.accountInstructions == null)
					this.accountInstructions = new InstructionCollection(this, bagOfInstructions);
				return accountInstructions;
			}
			set { accountInstructions = value; }
		}

        /// <summary>
        /// This is the collection of active instructions that belong to the account.
        /// A <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">rebalance instruction</see> defines the workflow that is involved in a rebalance.
        /// </summary>
        public virtual IInstructionCollection ActiveAccountInstructions
        {
            get
            {
                if (this.activeAccountInstructions == null)
                    this.activeAccountInstructions = new InstructionCollection(this, bagOfActiveInstructions);
                return activeAccountInstructions;
            }
        }

        public virtual IList<IMoneyTransferOrder> ActiveMoneyTransferOrders
        {
            get { return activeMoneyTransferOrders; }
        }

        /// <summary>
        /// This is the collection of active rebalance instructions that belong to the account.
        /// </summary>
        public virtual IInstructionCollection ActiveRebalanceInstructions
        {
            get
            {
                IList rebins = new ArrayList();
                if (ActiveAccountInstructions != null && ActiveAccountInstructions.Count > 0)
                {
                    foreach (IInstruction instruction in ActiveAccountInstructions)
                    {
                        if (instruction.IsTypeRebalance)
                            rebins.Add(instruction);
                    }
                }
                return new InstructionCollection(this, rebins);
            }
        }

        /// <summary>
        /// This is the collection of active withdrawal instructions that belong to the account.
        /// </summary>
        public virtual ICashWithdrawalInstructionCollection ActiveWithdrawalInstructions
        {
            get
            {
                IList withdrawals = new ArrayList();
                if (ActiveAccountInstructions != null && ActiveAccountInstructions.Count > 0)
                {
                    foreach (IInstruction instruction in ActiveAccountInstructions)
                    {
                        if (instruction.InstructionType == InstructionTypes.CashWithdrawal)
                        {
                            // Ignore the withdrawals where the relevance (exec) date is in the future
                            if (instruction.ExecutionDate <= DateTime.Now)
                                withdrawals.Add(instruction);
                        }
                    }
                }
                return new CashWithdrawalInstructionCollection(this, withdrawals);
            }
        }

        /// <summary>
        /// Is the customer departing
        /// </summary>
        public virtual bool IsDeparting
        {
            get { return ActiveRebalanceInstructions.Any(x => x.InstructionType == InstructionTypes.ClientDeparture); }
        }

        /// <summary>
        /// Is the customer under rebalance
        /// </summary>
        public virtual bool IsUnderRebalance
        {
            get { return ActiveRebalanceInstructions.Any(x => x.InstructionType == InstructionTypes.Rebalance || x.InstructionType == InstructionTypes.BuyModel); }
        }

        /// <summary>
        /// The current rebalance date of the active instruction
        /// </summary>
        public virtual DateTime CurrentRebalanceDate
        {
            get
            {
                DateTime retVal = DateTime.MinValue;

                if (ActiveAccountInstructions != null && ActiveAccountInstructions.Count > 0)
                {
                    foreach (IInstruction instruction in ActiveAccountInstructions)
                    {
                        if (instruction.IsTypeRebalance && instruction.IsActive && !instruction.Cancelled)
                        {
                            retVal = instruction.ExecutionDate;
                            break;
                        }
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        /// The last rebalance date of an instruction that was NOT cancelled but finished completely
        /// </summary>
        public virtual DateTime LastRebalanceDate
        {
            get
            {
                DateTime retVal = DateTime.MinValue;

                if (AccountInstructions != null && AccountInstructions.Count > 0)
                {
                    foreach (IInstruction instruction in AccountInstructions.SortedByDefault().Reversed())
                    {
                        if (instruction.IsTypeRebalance && !instruction.IsActive && (!instruction.Cancelled || (instruction.Cancelled && (instruction.OrdersGenerated > 0))))
                        {
                            retVal = instruction.ActualExecutedDate;
                            break;
                        }
                    }
                }
                return retVal;
            }
        }


        /// <summary>
        /// This property informs whether a new rebalance instruction can be inserted.
        /// </summary>
        public virtual bool AllowNewRebalanceInstruction
        {
            get 
            {
                if (ActiveAccountInstructions != null && ActiveAccountInstructions.Count > 0)
                {
                    foreach (IInstruction instruction in ActiveAccountInstructions)
                    {
                        if (instruction.IsTypeRebalance)
                            return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// This is the method where a new rebalance instruction is created for the account.
        /// </summary>
        /// <param name="instructionType">The type of instruction, currently we only support rebalance instructions</param>
        /// <param name="orderActionType">The type of instruction that is placed on the orders</param>
        /// <param name="executionDate">The date that the rebalance should be executed</param>
        /// <returns>A new <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">instruction</see></returns>
        public IInstructionTypeRebalance CreateInstruction(InstructionTypes instructionType, OrderActionTypes orderActionType, DateTime executionDate, bool doNotChargeCommission)
        {
            return CreateInstruction(instructionType, orderActionType, executionDate, doNotChargeCommission, null);
        }

        /// <summary>
        /// This is the method where a new rebalance instruction is created for the account.
        /// </summary>
        /// <param name="instructionType">The type of instruction, currently we only support rebalance instructions</param>
        /// <param name="orderActionType">The type of instruction that is placed on the orders</param>
        /// <param name="executionDate">The date that the rebalance should be executed</param>
        /// <param name="doNotChargeCommission">Determines whether commission should be charged</param>
        /// <param name="cashTransfers">The cash transfers involved</param>
        /// <returns>A new <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">instruction</see></returns>
        public IInstructionTypeRebalance CreateInstruction(InstructionTypes instructionType, OrderActionTypes orderActionType, DateTime executionDate, bool doNotChargeCommission, IList<IJournalEntryLine> cashTransfers)
        {
            IInstructionTypeRebalance instruct = null;
            switch (instructionType)
            {
                case InstructionTypes.Rebalance:
                    instruct = new RebalanceInstruction(this, executionDate, orderActionType, doNotChargeCommission, cashTransfers);
                    break;
                case InstructionTypes.BuyModel:
                    instruct = new BuyModelInstruction(this, executionDate, orderActionType, doNotChargeCommission, cashTransfers);
                    break;
                default:
                    throw new ApplicationException("Wrong instruction type for this method");
            }

            if (instruct != null)
            {
                AccountInstructions.Add(instruct);
                ActiveAccountInstructions.Add(instruct);
            }
            return instruct;
        }

        /// <summary>
        /// This is the method where a new withdrawal instruction is created for the account.
        /// </summary>
        /// <param name="executionDate">The day from which the instruction should be taken into account</param>
        /// <param name="withdrawalDate">The day that the money should be there</param>
        /// <param name="withdrawalAmount">The relevant amount</param>
        /// <param name="counterAccount">The account the money should go to</param>
        /// <param name="rule">The rule that is responsible for this periodic instruction</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        /// <returns>A new <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">instruction</see></returns>
        public ICashWithdrawalInstruction CreateWithdrawalInstruction(DateTime executionDate, DateTime withdrawalDate,
            Money withdrawalAmount, ICounterAccount counterAccount, IWithdrawalRule rule, string transferDescription, bool doNotChargeCommission)
        {
            if (withdrawalAmount == null || withdrawalAmount.IsGreaterThanZero)
                throw new ApplicationException("The withdrawal amount is mandatory and can not be positive");

            if (counterAccount == null && this.CounterAccount == null)
                throw new ApplicationException("There should be at least a counter account on the rule or on the account.");

            ICashWithdrawalInstruction instruct = new CashWithdrawalInstruction(this, executionDate, withdrawalDate, withdrawalAmount, counterAccount, rule, transferDescription, doNotChargeCommission);
            if (instruct != null && instruct.Validate())
            {
                AccountInstructions.Add(instruct);
                ActiveAccountInstructions.Add(instruct);
            }
            return instruct;
        }

        /// <summary>
        /// This is the method where a new withdrawal instruction is created for the account.
        /// </summary>
        /// <param name="executionDate">The day from which the instruction should be taken into account</param>
        /// <param name="counterAccount">The account the money should go to</param>
        /// <param name="transferDescription">The transfer Description on the final Money transfer order</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        /// <returns>A new <see cref="T:B4F.TotalGiro.Accounts.Instructions.ClientDepartureInstruction">instruction</see></returns>
        public IClientDepartureInstruction CreateDepartureInstruction(DateTime executionDate, ICounterAccount counterAccount, string transferDescription, bool doNotChargeCommission)
        {
            IClientDepartureInstruction instruct = new ClientDepartureInstruction(this, executionDate, doNotChargeCommission, counterAccount, transferDescription);
            if (instruct != null)
            {
                AccountInstructions.Add(instruct);
                ActiveAccountInstructions.Add(instruct);
            }
            return instruct;
        }

        /// <summary>
        /// The last stored valuation cash mutations
        /// </summary>
        public virtual ILastValuationCashMutationCollection LastValuationCashMutations
        {
            get
            {
                if (this.lastValuationCashMutations == null)
                    this.lastValuationCashMutations = new LastValuationCashMutationCollection(this, this.bagOfValuationCashMutations);
                return lastValuationCashMutations;
            }
        }

        /// <summary>
        /// This is the collection of active withdrawal instructions that belong to the account.
        /// </summary>
        public virtual IAccountNotificationsCollection Notifications
        {
            get
            {
                AccountNotificationsCollection col = (AccountNotificationsCollection)notifications.AsList();
                if (col.Parent == null) col.Parent = this;
                return col;
            }
        }


		#region Private Variables

        private IPortfolioModel modelPortfolio;
        private bool isExecOnlyCustomer;
        private IAccountAccountHoldersCollection accountHolders;
        private IList bagOfAccountHolders = new ArrayList();
        private DateTime? firstManagementStartDate;
        private DateTime? finalManagementEndDate;
        private IAccountTypeCustomer exitFeePayingAccount;
        private DateTime? valuationMutationValidityDate;
        private DateTime? lastValuationDate;
        private IModelHistoryCollection modelPortfolioChanges;
        private IList bagOfModelHistoryItems = new ArrayList();
        private IInstructionCollection accountInstructions;
        private IInstructionCollection activeAccountInstructions;
        private IList bagOfInstructions = new ArrayList();
        private IList bagOfActiveInstructions = new ArrayList();
        private IList<IMoneyTransferOrder> activeMoneyTransferOrders;
        private IList bagOfValuationCashMutations = new ArrayList();
        private ILastValuationCashMutationCollection lastValuationCashMutations;
        private IDomainCollection<Notification> notifications;
        private IDomainCollection<IManagementPeriod> managementPeriods;
        private bool useKickback;
        private bool useManagementFee;

		#endregion
    }
}
