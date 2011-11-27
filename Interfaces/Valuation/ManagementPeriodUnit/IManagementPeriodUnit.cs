using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ManagementPeriodUnits.ReportData;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public enum FeesCalculatedStates
    {
        No = 0,
        Yes = 1,
        Irrelevant = 2
    }
    
    public interface IManagementPeriodUnit
    {
        int Key { get; set; }
        IManagementPeriod ManagementPeriod { get; }
        IManagementPeriodUnitParent UnitParent { get; }
        ICustomerAccount Account { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        int Period { get; }
        int Days { get; }
        bool IsPeriodEnd { get; }
        int Month { get; }
        IPortfolioModel ModelPortfolio { get; set;  }
        bool IsExecOnlyCustomer { get; set; }
        Money TotalValue { get; }
        IAverageHoldingCollection AverageHoldings { get; }

        IManagementFee ManagementFee { get; set; }
        IKickBackExport KickBackExport { get; set; }
        bool IsStornoed { get; set; }
        FeesCalculatedStates FeesCalculated { get; set; }
        int RulesFound { get; set; }
        bool Success { get; set; }
        string Message { get; set; }
        bool IsRelevantForFees { get; }
        int DocumentsSentByPost { get; set; }

        IManagementPeriodUnit NewManagementPeriodUnit { get; }
        IManagementPeriodUnitFeeCollection FeeItems { get; }
        IList<IAverageHoldingFee> AverageHoldingFeeItems { get; }
        Money TotalFeeAmount { get; }
        bool IsEditable { get; }

        bool IsBetween(DateTime date);
    }
}
