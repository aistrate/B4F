using System;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.ManagementPeriodUnits.Corrections
{
    public interface IManagementPeriodUnitCorrection
    {
        int Key { get; set; }
        IManagementPeriodUnit Unit { get; }
        IAverageHolding AverageHolding { get; }
        bool Skip { get; set; }
        bool IsOpen { get; }
    }
}
