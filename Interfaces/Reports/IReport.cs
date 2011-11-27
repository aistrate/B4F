using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.Reports.Documents;


namespace B4F.TotalGiro.Reports.Financial
{
    public enum ReportStatuses
    {
        NoValuations = 0,
        PrintSuccess = 1,
        Error = 2
    }

    public interface IReport
    {
        int Key { get; set; }
        IReportLetter ReportLetter { get; set; }
        int ReportStatusId { get; set; }
        IModelBase ModelPortfolio { get; set; }
        ICustomerAccount Account { get; set; }
        DateTime CreationDate { get; set; }
        IFinancialReportDocument Document { get; set; }
        IContactsNAW ContactsNAW { get; set; }
        IContactsNAW SecondContactsNAW { get; set; }
        string ErrorMessage { get; set;}
        void SetContactsNAW(ICustomerAccount account);
    }
}
