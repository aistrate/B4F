using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Reports
{
    public class ReportSetting : IReportSetting
    {
        public ReportSetting() { }

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual ICustomerAccount Account
        {
            get { return account; }
            set { account = value; }
        }

        public virtual bool IsEoy
        {
            get { return isEoy; }
            set { isEoy = value; }
        }

        public virtual bool IsQuarter
        {
            get { return isQuarter; }
            set { isQuarter = value; }
        }

        public virtual bool IsDeposit
        {
            get { return isDeposit; }
            set { isDeposit = value; }
        }

        public virtual Int32 EmployeeID
        {
            get { return employeeId; }
            set { employeeId = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        #region Private Variables
        private int key;
        private ICustomerAccount account;
        private bool isEoy;
        private bool isQuarter;
        private bool isDeposit;
        private Int32 employeeId;
        private DateTime creationDate = Util.NullDate;

        #endregion
    }
}
