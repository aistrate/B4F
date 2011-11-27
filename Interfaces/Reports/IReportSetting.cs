using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Reports
{
    public interface IReportSetting
    {
        int Key { get; set; }
        ICustomerAccount Account { get; set; }
        bool IsEoy { get; set; }
        bool IsQuarter { get; set; }
        bool IsDeposit { get; set; }
        Int32 EmployeeID { get; set; }
        DateTime CreationDate { get; set;}
    }
}
