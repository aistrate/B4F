using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Fees.FeeRules
{
    public interface IFeeRule
    {
        int Key { get; set; }
        IFeeCalc FeeCalculation { get; set; }
        IAssetManager AssetManager { get; set; }
        bool IsDefault { get; set; }
        IPortfolioModel ModelPortfolio { get; set; }
        ICustomerAccount Account { get; set; }
        bool ExecutionOnly { get; set; }
        bool HasEmployerRelation { get; set; }
        bool SendByPost { get; set; }
        int StartPeriod { get; set; }
        int EndPeriod { get; set; }
        int Weight { get; set; }
        string DisplayRule { get; }

        bool CalculateWeight(IManagementPeriodUnit client);
        bool EnvelopsPeriod(int period);
        bool EnvelopsDate(DateTime date);
        bool ChargeFeeForDate(DateTime date);
    }
}
