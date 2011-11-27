using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;
using System.Collections;

namespace B4F.TotalGiro.CRM
{
    public class ContactsIntroducersCollection : GenericCollection<IContactsIntroducer>, IContactsIntroducersCollection
    {
        public ContactsIntroducersCollection() { }

        public ContactsIntroducersCollection(IContact Parent, IList bagOfContactsIntroducers)
            : base(bagOfContactsIntroducers)
        {
            this.Parent = Parent;
        }

        public IContact Parent
        {
            get { return parent; }
            private set { parent = value; }
        }

        

        #region Private Variables
        private IContact parent;
        #endregion
    }
}
