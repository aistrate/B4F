using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Reports
{
    public class ReportLetter : IReportLetter
    {
        public ReportLetter() { }

        public ReportLetter(string concern, string letter)
        {
            this.concern = concern;
            this.letter = letter;
        }

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual string Concern
        {
            get { return concern; }
            set { concern = value; }
        }

        public virtual string Letter
        {
            get { return letter; }
            set { letter = value; }
        }

        public virtual int ReportLetterTypeId
        {
            get { return reportLetterTypeId;  }
            set { reportLetterTypeId = value; }
        }

        public virtual int ReportLetterYear
        {
            get { return reportLetterYear; }
            set { reportLetterYear = value; }
        }

        public string YearAndType
        {
            get { return string.Format("{0} {1}", ReportLetterYear, (ReportLetterTypes)ReportLetterTypeId); }
        }

        public virtual IManagementCompany ManagementCompany
        {
            get { return managementCompany; }
            set { managementCompany = value; }
        }

        public virtual int EmployeeID
        {
            get { return employeeId; }
            set { employeeId = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
            protected set { creationDate = value; }
        }

        public override string ToString()
        {
            return Concern.ToString();
        }

        #region Private Variables
        private int key;
        private string concern;
        private string letter;
        private int reportLetterTypeId;
        private int reportLetterYear;
        private IManagementCompany managementCompany;
        private int employeeId;
        private DateTime creationDate = DateTime.Now;

        #endregion
    }
}

