using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Fees.RuleDependencies
{
	public class CommRuleSpecific : CommRule
	{
		protected CommRuleSpecific() { }

		public CommRuleSpecific(OrderActionTypes actionType, bool applyToAllAccounts, 
			IAccountType accountType, IModel modelPortfolio, 
			IAccount account, IInstrument instrument, ISecCategory ruleSecCategory,
			IExchange exchange, string commRuleName, FeeTypes feeType,
			CommRuleOpenClose openClose, CommRuleBuySell buySell,
			Fees.Calculations.CommCalc commCalculation) 
			: base(commRuleName, feeType, commCalculation)
		{
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
		}

		public virtual OrderActionTypes ActionType
		{
			get { return this.actionType; }
			set { this.actionType = value; }
		}

		public virtual bool ApplyToAllAccounts
		{
			get { return this.applyToAllAccounts; }
			set { this.applyToAllAccounts = value; }
		}

		public virtual IAccountType AccountType
		{
			get { return this.accountType; }
			set { this.accountType = value; }
		}

		public virtual IModel ModelPortfolio
		{
			get { return this.modelPortfolio; }
			set { this.modelPortfolio = value; }
		}

		public virtual IAccount Account
		{
			get { return account; }
			set { account = value; }
		}

		public virtual IExchange Exchange
		{
			get { return exchange; }
			set { exchange = value; }
		}

		public virtual ISecCategory RuleSecCategory
		{
			get { return ruleSecCategory; }
			set { ruleSecCategory = value; }
		}

		public virtual IInstrument Instrument
		{
			get { return instrument; }
			set { instrument = value; }
		}

		public virtual CommRuleBuySell BuySell
		{
			get { return buySell; }
			set { buySell = value; }
		}

		public virtual CommRuleOpenClose OpenClose
		{
			get { return openClose; }
			set { openClose = value; }
		}

		public override bool CalculateWeight(IOrder theOrder)
		{
			IInstrument instrument = null;
			bool potentialHit = true;
			int theWeight = 0;

			potentialHit = calculateWeight(theOrder.Account, this.Account, RuleWeighting.Account, ref theWeight);

			if (potentialHit)
			{
				if (theOrder.Account is IAccountTypeCustomer)
					potentialHit = calculateWeight(((IAccountTypeCustomer)theOrder.Account).ModelPortfolio, this.ModelPortfolio, RuleWeighting.ModelPortfolio, ref theWeight);
			}

			if (potentialHit)
				potentialHit = calculateWeight(theOrder.Account.Type, this.AccountType, RuleWeighting.AccountType, ref theWeight);

			if (potentialHit)
			{
				if (theOrder is ISecurityOrder)
					instrument = ((ISecurityOrder)theOrder).TradedInstrument;
				else
					instrument = theOrder.Value.Underlying;
			}

			if(potentialHit)
				potentialHit = calculateWeight(instrument, this.Instrument, RuleWeighting.Instrument, ref theWeight);

			if (potentialHit)
			{
				if (instrument is ITradeableInstrument)
					potentialHit = calculateWeight(((ITradeableInstrument)instrument).DefaultExchange, this.Exchange, RuleWeighting.Exchange, ref theWeight);
			}
			
			if (potentialHit)
				potentialHit = calculateWeight(instrument.SecCategory, this.RuleSecCategory, RuleWeighting.SecCategory, ref theWeight);

			if (potentialHit)
				potentialHit = calculateWeight(true, this.ApplyToAllAccounts, RuleWeighting.AllAccounts, ref theWeight);

			if(potentialHit)
				this.Weight = theWeight;
			
			return potentialHit;
		}

		public override CommRuleTypes CommRuleType
		{
			get
			{
				return (Account == null && Exchange == null && Instrument == null && RuleSecCategory != null && BuySell == CommRuleBuySell.Both ? 
                    CommRuleTypes.Default : CommRuleTypes.Specific);
			}
		}

		#region Private Variables

		private OrderActionTypes actionType;
		private bool applyToAllAccounts;
		private IAccountType accountType;
		private IModel modelPortfolio;
		private IAccount account;
		private IExchange exchange;
		private ISecCategory ruleSecCategory;
		private IInstrument instrument;
		private CommRuleBuySell buySell = CommRuleBuySell.Both;
		private CommRuleOpenClose openClose = CommRuleOpenClose.Both;

		#endregion

	}
}
