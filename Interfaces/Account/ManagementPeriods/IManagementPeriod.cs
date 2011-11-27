using System;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Accounts.ManagementPeriods
{
    [Flags()]
    public enum ManagementTypes
    {
        ManagementFee = 1,
        KickBack = 2
    }

    public interface IManagementPeriod : IAuditable
    {
        int Key { get; set; }
        ICustomerAccount Account { get; set; }
        ManagementTypes ManagementType { get; set; }
        DateTime StartDate { get; set; }
        DateTime? EndDate { get; set; }
        string Employee { get; }
        IManagementPeriodUnitCollection ManagementPeriodUnits { get; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }
        bool IsEditable { get; }

        bool Edit(DateTime startDate, DateTime endDate);
        bool IsBetween(DateTime date);
        bool IsBetween(ManagementTypes managementType, DateTime date);
    }
}
