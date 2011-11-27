using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Fees.FeeRules
{
    public class FeeRuleFinder
    {
        public FeeRuleFinder(List<FeeRule> rules)
        {
            this.rules = rules;
        }
        
        public IFeeCalcVersion[] FindCalculations(IManagementPeriodUnit unit)
        {
            IFeeCalcVersion[] calculations = null;
            Dictionary<FeeTypes, List<IFeeRule>> rulesDictionary = new Dictionary<FeeTypes, List<IFeeRule>>();

            IModelHistory modelHistory = null;
            if (unit.IsPeriodEnd)
                modelHistory = unit.Account.ModelPortfolioChanges.GetItemByDate(unit.EndDate);
            else
                modelHistory = unit.Account.ModelPortfolioChanges.GetItemByDate(Util.GetLastDayOfMonth(unit.Period));

            if (modelHistory != null && modelHistory.ModelPortfolio != null)
            {
                unit.ModelPortfolio = modelHistory.ModelPortfolio;
                unit.IsExecOnlyCustomer = modelHistory.IsExecOnlyCustomer;
            }

            // First reset the weights back to 0
            foreach (IFeeRule rule in rules) { rule.Weight = 0; }

            // Calculate the weight
            foreach (IFeeRule rule in rules)
            {
                if (rule.CalculateWeight(unit))
                {
                    FeeTypes key = rule.FeeCalculation.FeeType.Key;
                    if (!rulesDictionary.ContainsKey(key))
                        rulesDictionary.Add(key, new List<IFeeRule>());
                    rulesDictionary[key].Add(rule);
                }
            }
            if (rulesDictionary.Count > 0)
            {
                calculations = new IFeeCalcVersion[rulesDictionary.Keys.Count];
                int i = 0;
                foreach (FeeTypes key in rulesDictionary.Keys)
                {
                    rulesDictionary[key].Sort(new FeeRule.MySorter(FeeRule.MySorter.SortOrder.Descending));
                    calculations[i] = rulesDictionary[key][0].FeeCalculation.Versions.GetItemByPeriod(unit.Period);
                    i++;
                }
            }
            return calculations;
        }

        private List<FeeRule> rules;
    }
}
