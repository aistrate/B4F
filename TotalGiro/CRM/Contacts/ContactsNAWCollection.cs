using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.CRM
{
    public class ContactsNAWCollection : GenericCollection<IContactsNAW>, IContactsNAWCollection
    {
        public ContactsNAWCollection()
        { }

        public ContactsNAWCollection(IContact Parent, IList bagOfContactsNAWs)
            : base(bagOfContactsNAWs)
        {
            this.Parent = Parent;
        }

        public IContact Parent
        {
            get { return parent; }
            private set { parent = value; }
        }

        public IContactsNAW Current
        {
            get 
            {
                IContactsNAWCollection al = (IContactsNAWCollection)this.SortedByDefault().Reversed();
                if (al != null)
                    return (IContactsNAW)al[0];
                else
                    return null;
            }
        }

        protected override object GetDefaultSortValue(IContactsNAW item)
        {
            return item.CreationDate;
        }

        /// <summary>
        /// Add item to collection of historical prices
        /// </summary>
        /// <param name="item">HistoricalPrice object</param>
        public override void Add(IContactsNAW item)
        {
            base.Add(item);
            item.Contact = Parent;
        }


        #region Private Variables

        private IContact parent;

        #endregion

    }
}
