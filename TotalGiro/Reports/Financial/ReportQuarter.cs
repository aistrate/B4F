using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Financial
{
    public class ReportQuarter : Report, IReportQuarter
    {
        public ReportQuarter() { }

        public ReportQuarter(ICustomerAccount account, IReportLetter reportLetter, ReportStatuses reportStatus, string reportErrorMessage)
            : base(account, reportLetter, reportStatus, reportErrorMessage)
        {
        }
    }
}

