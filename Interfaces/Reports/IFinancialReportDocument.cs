using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Reports.Financial;

namespace B4F.TotalGiro.Reports.Documents
{
    public interface IFinancialReportDocument : IDocument
    {
        IReport Report { get; set; }
    }
}
