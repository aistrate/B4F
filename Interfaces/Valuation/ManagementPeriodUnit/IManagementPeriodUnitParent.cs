using System;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public interface IManagementPeriodUnitParent
    {
        int Key { get; set; }
        ICustomerAccount Account { get; }
        int Period { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        int Month { get; }
        int Days { get; }
        IPortfolioModel ModelPortfolio { get; set; }
        bool IsExecOnlyCustomer { get; set; }
        Money TotalValue { get; }
        DateTime CreationDate { get; }
        IAverageHoldingCollection AverageHoldings { get; }
        IManagementPeriodUnitCollection ManagementPeriodUnits { get; }

        bool IsBetween(DateTime date);
    }
}
