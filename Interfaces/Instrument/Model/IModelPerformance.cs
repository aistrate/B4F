using System;
using System.Collections;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Instruments
{
    public interface IModelPerformance
    {
        int Key { get; set; }
        int Quarter { get; set; }
        int PerformanceYear { get; set; }
        decimal IBoxxTarget {get; set;}
        decimal MSCIWorldTarget {get; set;}
        decimal CompositeTarget {get; set;}
        decimal BenchMarkValue {get; set;}
        int EmployeeID { get; set; }
        string DisplayBenchMarkValue { get; }

        IPortfolioModel ModelPortfolio { get; set; }
    }
}
