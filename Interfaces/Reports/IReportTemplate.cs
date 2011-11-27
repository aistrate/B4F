using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Reports
{
    public interface IReportTemplate
    {
        int Key { get; set; }
        IManagementCompany ManagementCompany { get; }
        string ReportName { get; }
        string ReportTemplateName { get; set; }
        int GetHashCode();
        string ToString();
    }
}
