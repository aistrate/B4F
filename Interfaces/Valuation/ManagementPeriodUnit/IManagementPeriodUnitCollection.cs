using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts.ManagementPeriods;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public interface IManagementPeriodUnitCollection : IList<IManagementPeriodUnit>
    {
        IManagementPeriod Parent { get; set;  }
        IManagementPeriodUnit GetItemByDate(DateTime date);
        IManagementPeriodUnit GetItemByType(ManagementTypes managementType);
    }
}
