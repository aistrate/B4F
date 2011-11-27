using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Fees.CommRules
{
	/// <summary>
	/// Represents a rule for matching orders to a commission calculation.
	/// </summary>
    public class CommRule: ICommRule
	{
		public CommRule() { }

		public CommRule(string commRuleName,  
			OrderActionTypes actionType, bool applyToAllAccounts, 
			AccountTypes accountType, IPortfolioModel modelPortfolio, 
			IAccount account, IInstrument instrument, ISecCategory ruleSecCategory,
			IExchange exchange, CommRuleOpenClose openClose, CommRuleBuySell buySell,
			Fees.CommCalculations.CommCalc commCalculation,
            BaseOrderTypes originalOrderType,
            DateTime startdate, DateTime enddate) 
		{
			this.CommRuleName = commRuleName;
			this.actionType = actionType;
			this.ApplyToAllAccounts = applyToAllAccounts;
			this.AccountType = accountType;
			this.ModelPortfolio = modelPortfolio;
			this.Account = account;
			this.Instrument = instrument;
			this.RuleSecCategory = ruleSecCategory;
			this.Exchange = exchange;
            this.OpenClose = openClose;
			this.BuySell = buySell;
			this.CommCalculation = commCalculation;
            this.OriginalOrderType = originalOrderType;
            this.StartDate = startdate;
            this.EndDate = enddate;
		}

        /// <summary>
        /// Gets or sets the unique ID of the commission rule.
        /// </summary>
        public virtual int Key
		{
			get { return key; }
			set { key = value; }
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

		/// <summary>
		/// Gets or sets the name of the commission rule.
		/// </summary>
        public virtual string CommRuleName
		{
			get { return commRuleName; }
			set { commRuleName = value; }
		}

		/// <summary>
        /// Gets or sets the type of financial action this commission rule applies to 
        /// (see enum <see cref="B4F.TotalGiro.Orders.OrderActionTypes">OrderActionTypes</see>).
		/// </summary>
        public virtual OrderActionTypes ActionType
		{
			get { return this.actionType; }
			set { this.actionType = value; }
		}

		/// <summary>
		/// Gets or sets a flag indicating that this commission rule applies to all accounts, 
        /// thus allowing it to have the highest priority (largest <b>Weight</b>) among otherwise more specific rules 
        /// (e.g. a rule for a particular instrument with <b>ApplyToAllAccounts</b> set to <b>true</b> will have a higher priority 
        /// than a rule for a particular account and a particular exchange).
		/// </summary>
        public virtual bool ApplyToAllAccounts
		{
			get { return this.applyToAllAccounts; }
			set { this.applyToAllAccounts = value; }
		}

        /// <summary>
        /// Gets or sets the <b>AccountType</b> attached to this commission rule; 
        /// if set, the rule will only be applied to orders placed for accounts of this type.
        /// </summary>
        public virtual AccountTypes AccountType
		{
			get { return this.accountType; }
			set { this.accountType = value; }
		}

        /// <summary>
        /// Gets or sets the <b>ModelPortfolio</b> attached to this commission rule; 
        /// if set, the rule will only be applied to orders placed for accounts that use this model portfolio.
        /// </summary>
        public virtual IPortfolioModel ModelPortfolio
		{
			get { return this.modelPortfolio; }
			set { this.modelPortfolio = value; }
		}

		/// <summary>
        /// Gets or sets the <b>Account</b> attached to this commission rule; 
        /// if set, the rule will only be applied to orders placed for this account.
		/// </summary>
        public virtual IAccount Account
		{
			get { return account; }
			set { account = value; }
		}

        /// <summary>
        /// Does the account have a relationship to the employer
        /// </summary>
        public virtual bool HasEmployerRelation { get; set; }

        /// <summary>
        /// Gets or sets the <b>Exchange</b> attached to this commission rule; 
        /// if set, the rule will only be applied to orders placed on this exchange.
        /// </summary>
        public virtual IExchange Exchange
		{
			get { return exchange; }
			set { exchange = value; }
		}

        /// <summary>
        /// Gets or sets the <b>SecCategory</b> attached to this commission rule; 
        /// if set, the rule will only be applied to orders placed for this security category.
        /// </summary>
        public virtual ISecCategory RuleSecCategory
		{
			get { return ruleSecCategory; }
			set { ruleSecCategory = value; }
		}

        /// <summary>
        /// Gets or sets the <b>Instrument</b> attached to this commission rule; 
        /// if set, the rule will only be applied to orders placed for this instrument.
        /// </summary>
        public virtual IInstrument Instrument
		{
			get { return instrument; }
			set { instrument = value; }
		}

		/// <summary>
        /// Specifies whether this commission rule applies to <i>buy</i> or to <i>sell</i> orders.
		/// </summary>
        public virtual CommRuleBuySell BuySell
		{
			get { return buySell; }
			set { buySell = value; }
		}

        ///// <summary>
        ///// Specifies whether this commission rule applies to <i>open</i> or to <i>close</i> orders.
        ///// </summary>
        public virtual CommRuleOpenClose OpenClose
        {
            get { return openClose; }
            set { openClose = value; }
        }

		/// <summary>
		/// Gets or sets the commission calculation that will be used for orders matched by this commission rule.
		/// </summary>
        public virtual Fees.CommCalculations.ICommCalc CommCalculation
		{
			get { return commCalculation; }
			set { commCalculation = value; }
		}

        /// <summary>
        /// Gets or sets the additional charge that will be used for orders matched by this commission rule.
        /// </summary>
        public virtual Fees.CommCalculations.ICommCalc AdditionalCalculation
        {
            get { return additionalCalculation; }
            set { additionalCalculation = value; }
        }

        /// <summary>
        /// The date this rule becomes active
        /// </summary>
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        /// <summary>
        /// The date this rule is deactivated
        /// </summary>
        public virtual DateTime EndDate
        {
            get
            {
                if (endDate.HasValue)
                    return endDate.Value;
                else
                    return DateTime.MinValue;
            }
            set
            {
                endDate = value;
            }
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
        /// The Original order type of the order that initiated the commission.
        /// </summary>
        public BaseOrderTypes OriginalOrderType
        {
            get { return originalOrderType; }
            set { originalOrderType = value; }
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
                if (HasEmployerRelation)
                    display += ", HasEmployerRelation: true";
                if (this.ModelPortfolio != null)
                    display += ", Model: " + this.ModelPortfolio.ModelName;
                if (this.Instrument != null)
                    display += ", Instrument: " + this.Instrument.DisplayNameWithIsin;
                if (this.Exchange != null)
                    display += ", Exchange: " + this.Exchange.ExchangeName;
                if (this.RuleSecCategory != null)
                    display += ", SecCategory: " + this.RuleSecCategory.Description;
                if ((int)this.ActionType != 0)
                    display += ", ActionType: " + String.Join(", ", this.ActionType.ToEnumArray().Select(x => x.ToString()).ToArray());
                if (this.OriginalOrderType != BaseOrderTypes.Both)
                    display += ", OrderType: " + this.OriginalOrderType.ToString();
                if (this.BuySell != CommRuleBuySell.Both)
                    display += ", Side: " + this.BuySell.ToString();
                if (this.ApplyToAllAccounts)
                    display += ", ApplyToAllAccounts: true";

                if (!string.IsNullOrEmpty(display) && display.Length > 2)
                    display = display.Substring(2);

                if (this.CommRuleType == CommRuleTypes.Default)
                    display = "Default rule " + display;

                return display.Trim();
            }
        }


        /// <summary>
        /// Decides whether this commission rule has the highest priority among all matching rules for the given order.
		/// </summary>
        /// <param name="client">The order/transaction to verify.</param>
        /// <returns><b>true</b> if this rule has the highest priority among all matching rules for the given order, <b>false</b> if not.</returns>
        public bool CalculateWeight(ICommClient client)
		{
			IInstrument instrument = null;
			bool potentialHit = true;
			int theWeight = 0;

            if (client.TransactionDate < this.StartDate || (Util.IsNotNullDate(this.EndDate) && client.TransactionDate > this.EndDate))
                return false;

            if (client.Account.AccountOwner.Key != this.AssetManager.Key)
                return false;

            potentialHit = RuleComparer.CalculateWeight<IAccountTypeInternal>(client.Account, (IAccountTypeInternal)this.Account, "Key", (int)RuleWeighting.Account, ref theWeight);

			if (potentialHit)
			{
                if (client.Account.AccountType == AccountTypes.Customer && ((IAccountTypeCustomer)client.Account).ModelPortfolio != null)
                    potentialHit = RuleComparer.CalculateWeight<IPortfolioModel>(((IAccountTypeCustomer)client.Account).ModelPortfolio, this.ModelPortfolio, "Key", (int)RuleWeighting.ModelPortfolio, ref theWeight);
			}

            //if (potentialHit && (int)this.AccountType != 0)
            //    potentialHit = RuleComparer.CalculateWeight<AccountTypes>(client.Account.AccountType, this.AccountType, "Key", (int)RuleWeighting.AccountType, ref theWeight);

            if (potentialHit && this.HasEmployerRelation && client.Account.AccountType == AccountTypes.Customer)
                potentialHit = RuleComparer.CalculateWeight<bool>(((ICustomerAccount)client.Account).EmployerRelationship != AccountEmployerRelationship.None, this.HasEmployerRelation, (int)RuleWeighting.HasEmployerRelation, ref theWeight);

			if (potentialHit)
                instrument = client.TradedInstrument;

			if (potentialHit)
                potentialHit = RuleComparer.CalculateWeight<IInstrument>(instrument, this.Instrument, "Key", (int)RuleWeighting.Instrument, ref theWeight);

			if (potentialHit)
			{
                if (!instrument.IsCash && ((ITradeableInstrument)instrument).DefaultExchange != null)
                    potentialHit = RuleComparer.CalculateWeight<IExchange>(((ITradeableInstrument)instrument).DefaultExchange, this.Exchange, "Key", (int)RuleWeighting.Exchange, ref theWeight);
			}

			if (potentialHit)
                potentialHit = RuleComparer.CalculateWeight<ISecCategory>(instrument.SecCategory, this.RuleSecCategory, "Key", (int)RuleWeighting.SecCategory, ref theWeight);

			// ApplyToAllAccounts does not influence the hit result
			if (potentialHit)
                RuleComparer.CalculateWeight<bool>(true, this.ApplyToAllAccounts, (int)RuleWeighting.AllAccounts, ref theWeight);

            //// Check if commrule is made by the same asset manager
            //if (potentialHit)
            //    potentialHit = RuleComparer.CalculateWeight<IManagementCompany>(client.Account.AccountOwner, this.AssetManager, "Key", (int)RuleWeighting.AssetManager, ref theWeight);

            // Check the order action Type
            if (potentialHit && (int)this.ActionType != 0)
                potentialHit = RuleComparer.CalculateWeightForBitWiseEnum<OrderActionTypes>(client.ActionType, this.ActionType, (int)RuleWeighting.ActionType, ref theWeight);

            // Check the original Type of the order that initiated the commission
            if (potentialHit)
            {
                if (this.OriginalOrderType != BaseOrderTypes.Both)
                    potentialHit = RuleComparer.CalculateWeight<BaseOrderTypes>(client.OriginalOrderType, this.OriginalOrderType, (int)RuleWeighting.OrderType, ref theWeight);
            }

            // BuySell
			if (potentialHit)
			{
				if (BuySell != CommRuleBuySell.Both)
                    potentialHit = RuleComparer.CalculateWeight<CommRuleBuySell>((client.Side == Side.Buy ? CommRuleBuySell.Buy : CommRuleBuySell.Sell), BuySell, (int)RuleWeighting.SecCategory, ref theWeight);
			}

			//// OpenClose
			//if (potentialHit)
			//{
			//    if (OpenClose != CommRuleOpenClose.Both)
			//        potentialHit = calculateWeight((client.IsOpen == Side.Buy ? CommRuleBuySell.Buy : CommRuleBuySell.Sell), BuySell, 1, ref theWeight);
			//}

			if (potentialHit)
				this.Weight = theWeight;

			if (theWeight == 0)
				potentialHit = false;

			return potentialHit;
		}

		/// <summary>
        /// Gets a value indicating whether this is a <i>specific</i> or a <i>default</i> commission rule.
		/// </summary>
        public CommRuleTypes CommRuleType
		{
			get	{ return commRuleType; }
			set	{ commRuleType = value; }
		}

        /// <summary>
        /// A string representation of the commission rule.
        /// </summary>
        /// <returns>A string representation of the commission rule.</returns>
        public override string ToString()
		{
			return this.CommRuleType.ToString() + " rule: " + this.CommRuleName.Trim() + (this.CommRuleType == CommRuleTypes.Default ? " (default)" : "");
		}
        
		#region Private Variables

		private int key;
		private string commRuleName;
		private OrderActionTypes actionType;
		private bool applyToAllAccounts;
        // set default accounttype to Nostro -> No commission
        private AccountTypes accountType = AccountTypes.Nostro;
        private IPortfolioModel modelPortfolio;
        private DateTime startDate;
        private DateTime? endDate;
		private IAccount account;
		private IExchange exchange;
		private ISecCategory ruleSecCategory;
		private IInstrument instrument;
		private CommRuleBuySell buySell = CommRuleBuySell.Both;
        private CommRuleOpenClose openClose = CommRuleOpenClose.Both;
		private ICommCalc commCalculation;
        private ICommCalc additionalCalculation;
		private int weight;
        private IAssetManager assetManager;
        private CommRuleTypes commRuleType = CommRuleTypes.Specific;
        private BaseOrderTypes originalOrderType;

		private enum RuleWeighting
		{
			BuySell = 1,
            OrderType = 2,
            ActionType = 4,
            SecCategory = 8,
			ModelPortfolio = 16,
			Exchange = 32,
            Instrument = 64,
            HasEmployerRelation = 128,
            Account = 256,
            AllAccounts = 512
		}

		#endregion
	}
}
