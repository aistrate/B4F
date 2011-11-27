using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts.ManagementPeriods;

namespace B4F.TotalGiro.Fees.FeeRules
{
    public interface IFeeRuleCollection : IGenericCollection<IFeeRule>
    {
        IList<IFeeRule> Filter(ManagementTypes managementType, DateTime date);
    }
}
