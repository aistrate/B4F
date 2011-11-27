using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.GeneralLedger.Journal.Maintenance
{
    public class ClientBookYearClosureCollection : TransientDomainCollection<IClientBookYearClosure>, IClientBookYearClosureCollection
    {
        public ClientBookYearClosureCollection()
            : base() { }

        public ClientBookYearClosureCollection(IBookYearClosure parent)
            : base()
        {
            Parent = parent;
        }

        public IBookYearClosure Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        public void AddClientBookYearClosure(IClientBookYearClosure closure)
        {
            closure.ParentClosure = this.Parent;
            base.Add(closure);
        }

        #region Privates        

        private IBookYearClosure parent;

        #endregion
    }
}
