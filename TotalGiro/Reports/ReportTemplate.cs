using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Reports
{
    public class ReportTemplate : IReportTemplate
    {
        private ReportTemplate() { }

        public ReportTemplate(IManagementCompany managementCompany, string reportName, string reportTemplateName)
        {
            this.managementCompany = managementCompany;
            this.reportName = reportName;
            this.reportTemplateName = reportTemplateName;
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual IManagementCompany ManagementCompany
        {
            get { return managementCompany; }
            private set { managementCompany = value; }
        }

        public virtual string ReportName
        {
            get { return reportName; }
            private set { reportName = value; }
        }

        public virtual string ReportTemplateName
        {
            get { return reportTemplateName; }
            set { reportTemplateName = value; }
        }

        public override int GetHashCode()
        {
            return Key;
        }

        public override string ToString()
        {
            return ReportTemplateName;
        }

        #region Private Variables

        private int key;
        IManagementCompany managementCompany;
        private string reportName;
        private string reportTemplateName;

        #endregion
    }
}
