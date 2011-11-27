using System;
namespace B4F.TotalGiro.Valuations.ReportedData.Reports
{
    public interface IReportEndTermDividWep
    {
        ReportingPeriodDetail ReportPeriod { get; set; }
        IEndTermDividWepComparisonCollection Records { get; }
    }
}
