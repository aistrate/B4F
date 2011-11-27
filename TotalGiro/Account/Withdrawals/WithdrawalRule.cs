using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts.Instructions;

namespace B4F.TotalGiro.Accounts.Withdrawals
{
    public class WithdrawalRule: IWithdrawalRule
    {
        #region Constructor

        protected WithdrawalRule() { }

        public WithdrawalRule(Money amount, WithdrawalRuleRegularity regularity, DateTime firstDateWithdrawal, ICounterAccount counterAccount)
        {
            if (amount == null || amount.IsZero)
                throw new ApplicationException("Amount is mandatory.");

            if (Util.IsNullDate(firstDateWithdrawal))
                throw new ApplicationException("First Withdrawal Date is mandatory.");

            if (regularity == null)
                throw new ApplicationException("Regularity is mandatory.");

            if (firstDateWithdrawal < DateTime.Today)
                throw new ApplicationException("First Withdrawal Date can not be in the past.");

            this.Amount = amount;
            this.Regularity = regularity;
            this.FirstDateWithdrawal = firstDateWithdrawal;
            this.CounterAccount = counterAccount;
            this.IsActive = true;
            this.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
        }

        #endregion

        #region Properties

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public ICustomerAccount Account
        {
            get { return account; }
            set { account = value; }
        }

        public Money Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public WithdrawalRuleRegularity Regularity
        {
            get { return regularity; }
            set { regularity = value; }
        }

        public PandhouderPermissions PandhouderPermission
        {
            get { return pandhouderPermission; }
            set { pandhouderPermission = value; }
        }

        public virtual ICounterAccount CounterAccount
        {
            get { return counterAccount; }
            set { counterAccount = value; }
        }

        public virtual string TransferDescription { get; set; }
        public virtual bool DoNotChargeCommission { get; set; }

        public DateTime FirstDateWithdrawal
        {
            get { return firstDateWithdrawal; }
            set { firstDateWithdrawal = value; }
        }

        public DateTime EndDateWithdrawal
        {
            get { return endDateWithdrawal; }
            set 
            {
                if (Util.IsNotNullDate(value))
                {
                    if (value < FirstDateWithdrawal)
                        throw new ApplicationException("End Date should be after the start date.");
                }
                
                endDateWithdrawal = value;
                if (Util.IsNotNullDate(endDateWithdrawal) && endDateWithdrawal < DateTime.Today)
                    IsActive = false;
            }
        }

        public bool IsActive { get; set; }

        public DateTime LastWithdrawalDate
        {
            get { return GetSpecificDate(0); }
        }

        public DateTime NextWithdrawalDate1
        {
            get { return GetSpecificDate(1); }
        }

        public DateTime NextWithdrawalDate2
        {
            get { return GetSpecificDate(2); }
        }

        public DateTime NextWithdrawalDate3
        {
            get { return GetSpecificDate(3); }
        }

        public DateTime GetSpecificDate(int number)
        {
            return Util.DateAddByRegularity(Regularity.Key, number, FirstDateWithdrawal, EndDateWithdrawal, getHolidays());
        }

        public DateTime MaxWithdrawalDate
        {
            get
            {
                DateTime retVal = DateTime.MinValue;
                try
                {
                    retVal =  GetMaxWithdrawalDate();
                }
                catch (Exception)
                {
                    IsInValid = true;
                }
                return retVal;
            }
        }

        public DateTime GetMaxWithdrawalDate()
        {
            DateTime retDate = DateTime.MinValue;
            if (Account == null)
                throw new ApplicationException("Account is manadatory to calculate the max date for a withdrawal");

            IPortfolioModel model = Account.ModelPortfolio;
            if (model == null)
                throw new ApplicationException("Model is manadatory to calculate the max date for a withdrawal");

            if (model.Details == null)
                throw new ApplicationException("Model details is manadatory to calculate the max date for a withdrawal");

            switch (model.Details.CashManagementFundOption)
            {
                case CashManagementFundOptions.Excluded:
                    retDate = Regularity.DateUnitExclCashFund.AddUnitToDate(DateTime.Today, DateIntervalOptions.ExcludeWeekendsAndHolidays, getHolidays());
                    break;
                case CashManagementFundOptions.Included:
                    retDate = Regularity.DateUnitInclCashFund.AddUnitToDate(DateTime.Today, DateIntervalOptions.ExcludeWeekendsAndHolidays, getHolidays());
                    break;
                case CashManagementFundOptions.CashFundOnly:
                    retDate = Regularity.DateUnitCashFundOnly.AddUnitToDate(DateTime.Today, DateIntervalOptions.ExcludeWeekendsAndHolidays, getHolidays());
                    break;
            }

            if (Util.IsNotNullDate(EndDateWithdrawal) && retDate > EndDateWithdrawal)
                retDate = EndDateWithdrawal;
            return retDate;
        }

        private IDateTimeCollection getHolidays()
        {
            IDateTimeCollection holidays = null;
            try
            {
                holidays = Account.AccountOwner.StichtingDetails.Country.CountryHolidays;
            }
            catch
            {
                holidays = null;
            }
            return holidays;
        }

        /// <summary>
        /// This is the collection of Periodic Withdrawal Instructions that belong to this rule.
        /// </summary>
        public virtual ICashWithdrawalInstructionCollection WithdrawalInstructions
        {
            get
            {
                if (this.cashWithdrawals == null)
                    this.cashWithdrawals = new CashWithdrawalInstructionCollection(this.Account, bagOfWithdrawalInstructions);
                return cashWithdrawals;
            }
        }

        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
        }

        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        public virtual bool IsInValid { get; private set; }
        public virtual string CreatedBy { get; set; }

        #endregion

        #region Methods

        public bool Validate()
        {
            Money portValue = Account.TotalAll;
            Money allRulesAmount = null;

            if (Account.ActiveWithdrawalInstructions != null)
                portValue += Account.ActiveWithdrawalInstructions.TotalAmount;

            if (Account.WithdrawalRules != null && Account.WithdrawalRules.Count > 0)
            {
                foreach (IWithdrawalRule rule in Account.WithdrawalRules)
                {
                    if (rule.IsActive && (Util.IsNullDate(rule.EndDateWithdrawal) || rule.EndDateWithdrawal > DateTime.Today) &&
                        !rule.Equals(this))
                        allRulesAmount += rule.Amount;
                }
            }

            if (portValue == null || portValue.IsZero || portValue.IsLessThanZero)
                throw new ApplicationException(string.Format("The withdrawal rule of {0} for account {1} is not allowed since the totalportfolio value (incl. future withdrawals) ({2}) is not enough.", Amount.DisplayString, Account.DisplayNumberWithName, portValue.DisplayString));
            
            if (Money.Subtract(portValue, Amount).IsLessThanZero)
                throw new ApplicationException(string.Format("The withdrawal rule of {0} for account {1} exceeds the totalportfolio value (incl. future withdrawals) ({2}).", Amount.DisplayString, Account.DisplayNumberWithName, portValue.DisplayString));

            if (allRulesAmount != null && Money.Subtract(portValue, Amount + allRulesAmount).IsLessThanZero)
                throw new ApplicationException(string.Format("The withdrawal rules for account {0} exceed the totalportfolio value (incl. future withdrawals) ({1}).", Account.DisplayNumberWithName, portValue.DisplayString));

            if ((Amount.Abs().Quantity / portValue.Quantity) > 0.5M)
                throw new ApplicationException("The withdrawal rule can not be more than 50% of the total portfolio.");

            decimal tpv = Amount.Abs().Quantity;
            if (allRulesAmount != null)
                tpv += allRulesAmount.Abs().Quantity;
            if ((tpv / portValue.Quantity) > 0.5M)
                throw new ApplicationException("The total withdrawal rules can not be more than 50% of the total portfolio.");
            
            return true;
        }

        #endregion

        #region Private Variables

        private ICustomerAccount account;
        private Money amount;
        private WithdrawalRuleRegularity regularity;
        private PandhouderPermissions pandhouderPermission;
        private ICounterAccount counterAccount;
        private DateTime firstDateWithdrawal;
        private DateTime endDateWithdrawal;
        private DateTime creationDate = DateTime.Now;
        private DateTime lastUpdated;
        private int key;
        private IList bagOfWithdrawalInstructions;
        private ICashWithdrawalInstructionCollection cashWithdrawals;

        #endregion
    }
}
