using System;

namespace B4F.TotalGiro.ManagementPeriodUnits.ReportData
{
    public interface IKickBackExport
    {
        B4F.TotalGiro.Accounts.ICustomerAccount Account { get; }
        string AccountNumber { get; }
        string AccountShortName { get; }
        string Adviseur { get; }
        decimal AvgValue { get; }
        DateTime CreationDate { get; }
        string InkoopOrginisatie { get; }
        string Kantoor { get; }
        int Key { get; }
        decimal Kickback { get; }
        decimal KickbackPercentage { get; }
        DateTime ManagementEndDate { get; }
        B4F.TotalGiro.Instruments.IPortfolioModel Model { get; }
        string ModelName { get; }
        B4F.TotalGiro.Stichting.Remisier.IRemisier ParentRemisier { get; }
        int Period { get; }
        B4F.TotalGiro.Stichting.Remisier.IRemisier Remisier { get; }
        B4F.TotalGiro.Stichting.Remisier.IRemisierEmployee RemisierEmployee { get; }
        //B4F.TotalGiro.ManagementPeriodUnits.IManagementPeriodUnit Unit { get; }
        IManagementPeriodUnitCollection ManagementPeriodUnits { get; }
    }
}
