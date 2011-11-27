using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Fees.RuleDependencies
{
	public class CommRuleGeneric : CommRule
	{
		protected CommRuleGeneric() {}

		public CommRuleGeneric(IModel ModelPortfolio, IAccountType AccountType, 
			string CommRuleName, FeeTypes FeeType, Fees.Calculations.CommCalc CommCalculation)
			: base(CommRuleName, FeeType, CommCalculation)
		{
			this.ModelPortfolio = ModelPortfolio;
			this.AccountType = AccountType;
		}

		public virtual IModel ModelPortfolio
		{
			get { return modelPortfolio; }
			set { modelPortfolio = value; }
		}

		public virtual IAccountType AccountType
		{
			get { return accountType; }
			set { accountType = value; }
		}
	

		#region Private Variables

		private IModel modelPortfolio;
		private IAccountType accountType;

		#endregion

		public override CommRuleTypes CommRuleType
		{
			get
			{
				return CommRuleTypes.Generic;
			}
		}

		public override bool CalculateWeight(IOrder theOrder)
		{
			bool potentialHit = true;
			int theWeight = 0;

			potentialHit = calculateWeight(((IAccountTypeCustomer)theOrder.Account).ModelPortfolio, this.ModelPortfolio, RuleWeighting.ModelPortfolio, ref theWeight);

			if (potentialHit)
				potentialHit = calculateWeight(theOrder.Account.Type, this.AccountType, RuleWeighting.AccountType, ref theWeight);

			if (potentialHit)
				this.Weight = theWeight;

			return potentialHit;
		}

}
}
