using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Financial
{
    public class ReportEOY : Report, IReportEOY
    {
        public ReportEOY() { }

        public ReportEOY(ICustomerAccount account, IReportLetter reportLetter, ReportStatuses reportStatus, string reportErrorMessage)
            : base(account, reportLetter, reportStatus, reportErrorMessage)
        {
        }
    }
}
