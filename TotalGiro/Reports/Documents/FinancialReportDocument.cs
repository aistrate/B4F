using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Reports.Financial;

namespace B4F.TotalGiro.Reports.Documents
{
    public class FinancialReportDocument : Document, IFinancialReportDocument
    {
        public FinancialReportDocument() : base() { }

        public FinancialReportDocument(string fileName, string filePath, bool sentByPost)
            : base(fileName, filePath, sentByPost) { }

        public override ICustomerAccount Account
        {
            get
            {
                if (Report != null)
                    return Report.Account;
                else
                    throw new ApplicationException(
                        string.Format("Could not determine account for document '{0}' because no report is associated with it.", Key));
            }
        }

        public virtual IReport Report { get; set; }
    }
}
