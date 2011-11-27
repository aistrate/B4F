using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Valuations.ReportedData.Reports
{
    public class EndTermDividWepComparisonCollection : TransientDomainCollection<IEndTermDividWepComparison>, B4F.TotalGiro.Valuations.ReportedData.Reports.IEndTermDividWepComparisonCollection
    {
        public EndTermDividWepComparisonCollection()
            : base() { }

        public EndTermDividWepComparisonCollection(IReportEndTermDividWep parent)
            : base()
        {
            Parent = parent;
        }

        public void AddEndTermDividWepComparison(IEndTermDividWepComparison entry)
        {
            if (IsInitialized)
                entry.Parent = Parent;
            base.Add(entry);
        }

        public IReportEndTermDividWep Parent { get; set; }
    }
}
