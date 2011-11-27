using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.CRM
{
    public class CounterAccountCollection : GenericCollection<ICounterAccount>, ICounterAccountCollection
    {
        internal CounterAccountCollection() { }

        public CounterAccountCollection(IContact parent, IList bagOfCounterAccounts)
            : base(bagOfCounterAccounts)
        {
            this.Parent = parent;
        }

        public IContact Parent
        {
            get { return parent; }
            private set { parent = value; }
        }

        #region Overrides

        public override void Add(ICounterAccount item)
        {
            if (Contains(item))
            {
                throw new ApplicationException("The counter account is already attached to this contact.");
            }
            base.Add(item);
        }
        
        #endregion

        #region Privates

        private IContact parent;
        
        #endregion
    }
}
