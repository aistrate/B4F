using System;
using System.Collections.Generic;
namespace B4F.TotalGiro.Valuations.ReportedData.Reports
{
    public interface IEndTermDividWepComparisonCollection : IList<IEndTermDividWepComparison>
    {
        void AddEndTermDividWepComparison(IEndTermDividWepComparison entry);
        IReportEndTermDividWep Parent { get; set; }
    }
}
