using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Accounts
{
    public class AccountFinancialTargetCollection : TransientDomainCollection<IAccountFinancialTarget>, IAccountFinancialTargetCollection
    {
        public AccountFinancialTargetCollection()
            : base() { }

        public AccountFinancialTargetCollection(ICustomerAccount parent)
            : base()
        {
            Parent = parent;
        }

        public ICustomerAccount Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        public void AddAccountFinancialTarget(IAccountFinancialTarget entry)
        {
            if (IsInitialized)
                entry.ParentAccount = Parent;
            base.Add(entry);
        }

        #region Privates

        private ICustomerAccount parent;

        #endregion
    }
}
