using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ManagementPeriodUnits;
using System.Reflection;

namespace B4F.TotalGiro.Fees.FeeRules
{
    /// <summary>
    /// Represents a rule for matching orders to a fee calculation.
    /// </summary>
    public class FeeRule : IFeeRule
    {
        public FeeRule() { }

        public FeeRule(IFeeCalc feeCalculation,
            IPortfolioModel modelPortfolio, ICustomerAccount account, bool isDefault,
            bool executionOnly, bool hasEmployerRelation, bool sendByPost, int startPeriod)
        {
            if (startPeriod == 0 || feeCalculation == null ||
                (account == null && modelPortfolio == null && !isDefault))
                throw new ApplicationException("Some parameters for the fee rule are mandatory");

            this.FeeCalculation = feeCalculation;
            this.StartPeriod = startPeriod;
            this.ModelPortfolio = modelPortfolio;
            this.Account = account;
            this.IsDefault = isDefault;
            this.ExecutionOnly = executionOnly;
            this.HasEmployerRelation = hasEmployerRelation;
            this.SendByPost = sendByPost;
        }

        /// <summary>
        /// Gets or sets the unique ID of the fee rule.
        /// </summary>
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Gets or sets the fee calculation that will be used for orders matched by this fee rule.
        /// </summary>
        public virtual IFeeCalc FeeCalculation
        {
            get { return feeCalculation; }
            set { feeCalculation = value; }
        }

        /// <summary>
        /// Gets or sets the fee calculation that will be used for orders matched by this fee rule.
        /// </summary>
        public bool ChargeFeeForDate(DateTime date)
        {
            IFeeCalcVersion calc = FeeCalculation.Versions.GetItemByPeriod(Util.GetPeriodFromDate(date));
            if (calc != null)
                return calc.IsFeeRelevant;
            else
                return false;
        }

        /// <summary>
        /// The Asset manager this rule belongs to.
        /// </summary>
        public virtual IAssetManager AssetManager
        {
            get { return this.assetManager; }
            set { this.assetManager = value; }
        }

        /// <summary>
        /// Is this a default rule for this assetmanager
        /// </summary>
        public virtual bool IsDefault { get; set; }


        /// <summary>
        /// Gets or sets the <b>ModelPortfolio</b> attached to this fee rule; 
        /// if set, the rule will only be applied to orders placed for accounts that use this model portfolio.
        /// </summary>
        public virtual IPortfolioModel ModelPortfolio
        {
            get { return this.modelPortfolio; }
            set 
            { 
                this.modelPortfolio = value;
                if (value != null)
                    AssetManager = value.AssetManager;
            }
        }

        /// <summary>
        /// Gets or sets the <b>Account</b> attached to this fee rule; 
        /// if set, the rule will only be applied to orders placed for this account.
        /// </summary>
        public virtual ICustomerAccount Account
        {
            get { return account; }
            set 
            { 
                account = value;
                if (value != null && value.AccountOwner != null && !value.AccountOwner.IsStichting)
                    AssetManager = (IAssetManager)value.AccountOwner;
            }
        }

        /// <summary>
        /// Is the account ExecutionOnly
        /// </summary>
        public virtual bool ExecutionOnly { get; set; }

        /// <summary>
        /// Does the account have a relationship to the employer
        /// </summary>
        public virtual bool HasEmployerRelation { get; set; }

        /// <summary>
        /// This rule accounts for notas and documents that are send by post in the period
        /// </summary>
        public virtual bool SendByPost { get; set; }

        /// <summary>
        /// The period this rule becomes active
        /// </summary>
        public virtual int StartPeriod
        {
            get { return startPeriod; }
            set { startPeriod = value; }
        }

        /// <summary>
        /// The period this rule is deactivated
        /// </summary>
        public virtual int EndPeriod
        {
            get { return endPeriod; }
            set { endPeriod = value; }
        }

        /// <summary>
        /// The display characteristics of this rule.
        /// </summary>
        public string DisplayRule
        {
            get
            {
                string display = "";

                if (this.Account != null)
                    display += ", Account: " + this.Account.DisplayNumberWithName;
                if (this.ModelPortfolio != null)
                    display += ", Model: " + this.ModelPortfolio.ModelName;
                if (ExecutionOnly)
                    display += ", ExecutionOnly: true";
                if (HasEmployerRelation)
                    display += ", HasEmployerRelation: true";
                if (SendByPost)
                    display += ", SendByPost: true";

                if (!string.IsNullOrEmpty(display) && display.Length > 2)
                    display = display.Substring(2);

                if (this.IsDefault)
                    display = "Default rule " + display;

                return display.Trim();
            }
        }

        /// <summary>
        /// Gets or sets a number representing the priority of this rule among other rules that match the same order; 
        /// the larger the <b>Weight</b>, the higher the priority.
        /// </summary>
        public virtual int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public bool EnvelopsPeriod(int period)
        {
            bool retVal = false;
            if (StartPeriod <= period && (EndPeriod == 0 || EndPeriod >= period))
                retVal = true;
            return retVal;
        }

        public bool EnvelopsDate(DateTime date)
        {
            return EnvelopsPeriod(Util.GetPeriodFromDate(date));
        }

        /// <summary>
        /// Date/time this fee rule was created
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
            internal set { this.creationDate = value; }
        }

        /// <summary>
        /// Date/time when this fee rule has last been updated
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }


        /// <summary>
        /// Decides whether this commission rule has the highest priority among all matching rules for the given order.
        /// </summary>
        /// <param name="client">The order/transaction to verify.</param>
        /// <returns><b>true</b> if this rule has the highest priority among all matching rules for the given order, <b>false</b> if not.</returns>
        public bool CalculateWeight(IManagementPeriodUnit client)
        {
            bool potentialHit = true;
            int theWeight = 0;

            if (this.FeeCalculation == null || !this.FeeCalculation.IsActive)
                return false;

            if (!(client.Account.AccountOwner.Key.Equals(this.AssetManager.Key)))
                return false;

            if (!(client.ManagementPeriod.ManagementType.Equals(this.FeeCalculation.FeeType.ManagementType) && EnvelopsPeriod(client.Period)))
                return false;

            if (client.Account != null)
                potentialHit = RuleComparer.CalculateWeight<IAccountTypeCustomer>(client.Account, this.Account, "Key", (int)RuleWeighting.Account, ref theWeight);

            if (potentialHit && this.ExecutionOnly)
                potentialHit = RuleComparer.CalculateWeight<bool>(client.IsExecOnlyCustomer, this.ExecutionOnly, (int)RuleWeighting.ExecutionOnly, ref theWeight);

            if (potentialHit && this.HasEmployerRelation && client.Account.AccountType == AccountTypes.Customer)
                potentialHit = RuleComparer.CalculateWeight<bool>(((ICustomerAccount)client.Account).EmployerRelationship != AccountEmployerRelationship.None, this.HasEmployerRelation, (int)RuleWeighting.ExecutionOnly, ref theWeight);

            if (potentialHit && client.ModelPortfolio != null)
                potentialHit = RuleComparer.CalculateWeight<IPortfolioModel>(client.ModelPortfolio, this.ModelPortfolio, "Key", (int)RuleWeighting.ModelPortfolio, ref theWeight);

            if (potentialHit && this.SendByPost)
                potentialHit = RuleComparer.CalculateWeight<bool>(client.DocumentsSentByPost > 0, this.SendByPost, (int)RuleWeighting.SendByPost, ref theWeight);

            if (potentialHit && this.IsDefault)
                potentialHit = RuleComparer.CalculateWeight<bool>(true, this.IsDefault, (int)RuleWeighting.Default, ref theWeight);

            if (potentialHit)
                this.Weight = theWeight;

            if (theWeight == 0)
                potentialHit = false;

            return potentialHit;
        }

        private bool calculateWeight<T>(object obj1, object obj2, RuleWeighting ruleWeighting, ref int theWeight)
        {
            bool theResult = false;
            if (obj2 != null)
            {
                if (((T)obj1).GetHashCode().Equals(((T)obj2).GetHashCode()))
                {
                    theWeight += (int)ruleWeighting;
                    theResult = true;
                }
            }
            else
            {
                theResult = true;
            }
            return theResult;
        }

        private bool calculateWeight<T>(object obj1, object obj2, string keyField, RuleWeighting ruleWeighting, ref int theWeight)
        {
            bool theResult = false;
            if (obj2 != null)
            {
                if (getPropertyValue(obj1, typeof(T), keyField) == getPropertyValue(obj2, typeof(T), keyField))
                {
                    theWeight += (int)ruleWeighting;
                    theResult = true;
                }
            }
            else
            {
                theResult = true;
            }
            return theResult;
        }

        private int getPropertyValue(object obj, Type type, string keyField)
        {
            PropertyInfo pi = obj.GetType().GetProperty(keyField);
            if (pi != null)
                return (int)pi.GetValue(obj, null);
            else
                return int.MinValue;
        }

        /// <summary>
        /// A string representation of the fee rule.
        /// </summary>
        /// <returns>A string representation of the fee rule.</returns>
        public override string ToString()
        {
            return string.Format("rule: {0} {1} for {2}", this.FeeCalculation.FeeType.ToString(), this.FeeCalculation.Name);
        }

        internal class MySorter : IComparer<IFeeRule>
        {
            public MySorter(SortOrder SortOrder)
            {
                this.sortOrder = SortOrder;
            }

            public enum SortOrder
            {
                Ascending = 0,
                Descending
            }

            #region IComparer<FeeRule> Members

            public int Compare(IFeeRule x, IFeeRule y)
            {
                int result = 0;
                if (x == null)
                    result = -1;
                else if (y == null)
                    result = 1;
                else
                {
                    int xNumber = x.Weight;
                    int yNumber = y.Weight;
                    switch (sortOrder)
                    {
                        case SortOrder.Ascending:
                            result = xNumber.CompareTo(yNumber);
                            break;
                        case SortOrder.Descending:
                            result = yNumber.CompareTo(xNumber);
                            break;
                    }
                }
                return result;
            }

            #endregion

            private SortOrder sortOrder = SortOrder.Descending;
        }

        #region Private Variables

        private int key;
        private IFeeCalc feeCalculation;
        private IAssetManager assetManager;
        private IPortfolioModel modelPortfolio;
        private ICustomerAccount account;
        private int startPeriod;
        private int endPeriod;
        private DateTime creationDate = DateTime.Now;
        private DateTime lastUpdated;
        private int weight;

        private enum RuleWeighting
        {
            Default = 1,
            SendByPost = 4,
            ModelPortfolio = 8,
        	ExecutionOnly = 16,
	        HasEmployerRelation = 32,
            Account = 64
        }

        #endregion
    }
}
