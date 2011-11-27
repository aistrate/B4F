using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Accounts.ManagementPeriods;

namespace B4F.TotalGiro.Accounts
{
    public class FeeRuleCollection : GenericCollection<IFeeRule>, IFeeRuleCollection
    {
        internal FeeRuleCollection(ICustomerAccount parent, IList bagOfModelFeeRules)
            : base(bagOfModelFeeRules)
        {
            this.parent = parent;
        }

        /// <summary>
        /// The model the rules belong to.
        /// </summary>
        public ICustomerAccount Parent
        {
            get { return parent; }
        }

        public override void Add(IFeeRule item)
        {
            int count = 0;
            if (Count > 0)
            {
                foreach (IFeeRule rule in this)
                {
                    if (rule.FeeCalculation.FeeType.Equals(item.FeeCalculation.FeeType))
                    {
                        // new rule is before existing rule -> set endDate
                        if (rule.StartPeriod == item.StartPeriod)
                            throw new Exception(string.Format("A {0} fee rule already exists in the same period for account {1}.", item.FeeCalculation.FeeType.ToString(), Parent.Number));
                        // new rule is before existing rule -> set endDate on new rule
                        else if (rule.StartPeriod > item.StartPeriod)
                            item.EndPeriod = rule.StartPeriod;
                        else if (rule.StartPeriod < item.StartPeriod && rule.EndPeriod == 0)
                        {
                            rule.EndPeriod = item.StartPeriod;
                            count++;
                        }
                    }
                }
            }
            if (count > 1)
                throw new Exception(string.Format("Too many active {0} fee rules on account {1} exist.", item.FeeCalculation.FeeType.ToString(), Parent.Number));

            item.Account = Parent;
            base.Add(item);
        }

        public IList<IFeeRule> Filter(ManagementTypes managementType, DateTime date)
        {
            return (from a in this.Cast<IFeeRule>()
                    where a.FeeCalculation.FeeType.ManagementType.Equals(managementType) && a.EnvelopsDate(date)
                    select a).ToList();
        }

        #region Privates

        private ICustomerAccount parent;

        #endregion
    }
}
