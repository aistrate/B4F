using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Reports
{
    public enum ReportLetterTypes
    {
        Q1 = 1,
        Q2 = 2,
        Q3 = 3,
        Q4 = 4,
        EOY = 12
    }

    public interface IReportLetter
    {
        int Key { get; set; }
        string Concern { get; set; }
        string Letter { get; set;}
        int ReportLetterTypeId { get; set; }
        int ReportLetterYear { get; set; }
        string YearAndType { get; }
        IManagementCompany ManagementCompany { get; set;}
        int EmployeeID { get; set; }
        DateTime CreationDate { get; }    
    }
}

