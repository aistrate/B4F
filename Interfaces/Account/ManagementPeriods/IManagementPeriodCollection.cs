using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Accounts.ManagementPeriods
{
    public interface IManagementPeriodCollection : IList<IManagementPeriod>
    {
        ICustomerAccount Parent { get; set; }
        IManagementPeriod GetItemByDate(DateTime date);
        void AddManagementPeriod(IManagementPeriod item);
        IList<IManagementPeriod> Filter(ManagementTypes managementType);
        bool IsManagementPeriodEditable(ManagementTypes managementType);
    }
}
