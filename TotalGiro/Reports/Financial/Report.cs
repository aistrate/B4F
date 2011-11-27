using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Reports.Documents;

namespace B4F.TotalGiro.Reports.Financial
{
    public abstract class Report : IReport
    {
        public static readonly ReportStatuses[] ReportStatusList = new ReportStatuses[]
        {
            ReportStatuses.Error,
            ReportStatuses.PrintSuccess,
            ReportStatuses.NoValuations
        };

        public static IReport CreateReport(ICustomerAccount account, IReportLetter reportLetter,
                                           ReportStatuses reportStatus, string reportErrorMessage)
        {
            if (reportLetter.ReportLetterTypeId == (int)ReportLetterTypes.EOY)
                return new ReportEOY(account, reportLetter, reportStatus, reportErrorMessage);
            else
                return new ReportQuarter(account, reportLetter, reportStatus, reportErrorMessage);
        }

        protected Report() { }

        protected Report(ICustomerAccount account, IReportLetter reportLetter, ReportStatuses reportStatus, string reportErrorMessage)
        {
            Account = account;
            ModelPortfolio = account.ModelPortfolio;
            SetContactsNAW(account);

            ReportLetter = reportLetter;
            ReportStatusId = (int)reportStatus;
            ErrorMessage = reportErrorMessage;

            CreationDate = DateTime.Now;
        }

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public void SetContactsNAW(ICustomerAccount account)
        {
            ContactsFormatter formatter = ContactsFormatter.CreateContactsFormatter(account);
            ContactsNAW = formatter.ContactsNAW;
            SecondContactsNAW = formatter.SecondContactsNAW;
        }

        public virtual ICustomerAccount Account { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual IFinancialReportDocument Document { get; set; }
        public virtual IContactsNAW ContactsNAW { get; set; }
        public virtual IContactsNAW SecondContactsNAW { get; set; }
        public virtual IReportLetter ReportLetter { get; set; }
        public virtual int ReportStatusId { get; set; }
        public virtual IModelBase ModelPortfolio { get; set; }
        public virtual string ErrorMessage { get; set; }

        private int key;
    }
}