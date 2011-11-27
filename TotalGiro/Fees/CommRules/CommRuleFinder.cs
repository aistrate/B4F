using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Fees.CommRules
{
	public class CommRuleFinder
	{
		public CommRuleFinder(IList<ICommRule> dependencies)
		{
			this.dependencies = dependencies;
		}

        public ICommRule FindRule(ICommClient client)
		{
			List<ICommRule> results = new List<ICommRule>();
			ICommRule result = null;

			// First reset the weights back to 0
			foreach (ICommRule rd in dependencies) {	rd.Weight = 0; }

			// Calculate the weight
			foreach (ICommRule rd in dependencies.Where(x => 
                x.AssetManager.Key == client.Account.AccountOwner.Key && 
                (client.TransactionDate >= x.StartDate) && (Util.IsNullDate(x.EndDate) || (Util.IsNotNullDate(x.EndDate) && client.TransactionDate <= x.EndDate))))
			{
                if (rd.CalculateWeight(client))
				{
					results.Add(rd);
				}
			}
			if (results.Count > 0)
				result = (ICommRule)results.OrderByDescending(x => x.Weight).FirstOrDefault();
			return result;
		}


		#region Private Variables

        private IList<ICommRule> dependencies = null;

		#endregion

		
	}
}
