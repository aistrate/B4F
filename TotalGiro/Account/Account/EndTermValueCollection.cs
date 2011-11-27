using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Valuations.ReportedData;

namespace B4F.TotalGiro.Accounts
{
    public class EndTermValueCollection : TransientDomainCollection<IEndTermValue>, IEndTermValueCollection
    {
        public EndTermValueCollection()
            : base() { }

        public EndTermValueCollection(IAccountTypeInternal parentAccount)
            : base()
        {
            ParentAccount = parentAccount;
        }

        public IAccountTypeInternal ParentAccount
        {
            get { return parentAccount; }
            set
            {
                parentAccount = value;
                IsInitialized = true;
            }
        }

        public bool EndTermValueExists(ReportingPeriodDetail reportingPeriodDetail)
        {
            return this.Exists(e => ((e.TermType == reportingPeriodDetail.TermType) && (e.EndTermDate.Year == reportingPeriodDetail.EndTermYear)));

        }

        private IAccountTypeInternal parentAccount;
    }
}
