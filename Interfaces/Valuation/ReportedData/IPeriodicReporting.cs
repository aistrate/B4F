using System;
namespace B4F.TotalGiro.Valuations.ReportedData
{

    public interface IPeriodicReporting
    {
        string CreatedBy { get; set; }
        DateTime CreationDate { get; set; }
        DateTime EndTermDate { get; set; }
        ReportingPeriodDetail ReportingPeriod { get; set; }
        EndTermType TermType { get;  }
        int EndTermYear { get; }
        int Key { get; set; }
    }
}
