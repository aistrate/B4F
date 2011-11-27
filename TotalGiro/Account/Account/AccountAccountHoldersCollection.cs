using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Accounts
{
    public class AccountAccountHoldersCollection : GenericCollection<IAccountHolder>, IAccountAccountHoldersCollection
    {
        public AccountAccountHoldersCollection() { }
        public AccountAccountHoldersCollection(IAccountTypeCustomer parent, IList bagOfAccountHolders)
            : base(bagOfAccountHolders)
        {
            this.Parent = parent;
        }

        public IAccountTypeCustomer Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public void SetPrimaryAccountHolder(IContact contact)
        {
            foreach (IAccountHolder ich in this)
            {
                if (ich.Contact.Equals(contact))
                    ich.IsPrimaryAccountHolder = true;
                else
                    ich.IsPrimaryAccountHolder = false;
            }
        }


        public IAccountHolder PrimaryAccountHolder
        {
            get
            {
                if (Count == 1)
                    return this[0];
                foreach (IAccountHolder ich in this)
                {
                    if (ich.IsPrimaryAccountHolder)
                        return ich;
                }
                return null;
            }
        }

        public IAccountHolder EnOfAccountHolder
        {
            get
            {
                if (Count > 1)
                {
                    foreach (IAccountHolder ich in this)
                    {
                        if (!ich.IsPrimaryAccountHolder)
                            return ich;
                    }
                }
                return null;
            }
        }

        #region Overrides

        public override void Add(IAccountHolder item)
        {
            if (!item.GiroAccount.Equals(Parent))
                throw new ApplicationException("The account should match the parent account.");

            base.Add(item);
        }


        public override bool Contains(IAccountHolder item)
        {
            bool ret = false;

            foreach (IAccountHolder accHolder in this)
            {
                if (accHolder.Contact.Equals(item.Contact))
                {
                    return ret = true;
                }
            }
            return ret;
        }

        public override bool Remove(IAccountHolder item)
        {
            bool success = base.Remove(item);
            item.GiroAccount = null;
            return success;
        }

        #endregion

        public IAccountHolder GetItemByContact(IContact contact)
        {
            foreach (IAccountHolder accHolder in this)
            {
                if (accHolder.Contact.Equals(contact))
                {
                    return accHolder;
                }
            }
            return null;
        }


        #region Private Variables

        private IAccountTypeCustomer parent;

        #endregion
    }

}
