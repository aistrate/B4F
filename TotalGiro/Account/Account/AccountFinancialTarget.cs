using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Accounts
{
    public class AccountFinancialTarget : IAccountFinancialTarget
    {
        public AccountFinancialTarget() { }

        public int Key { get; set; }
        public ICustomerAccount ParentAccount { get; set; }
        public Money TargetAmount { get; set; }
        public Money DepositPerYear { get; set; }
        public DateTime TargetEndDate
        {
            get { return this.targetEndDate.HasValue ? this.targetEndDate.Value : DateTime.MinValue; }
            set { this.targetEndDate = value; }
        }
        public ILogin CreatedBy { get; set; }

        public DateTime CreationDate
        {
            get { return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; }
            set { this.creationDate = value; }
        }

        #region Privates

        private DateTime? creationDate;
        private DateTime? targetEndDate;

        #endregion
    }
}
