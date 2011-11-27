using System;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.CRM.Contacts
{
    public class ContactsFormatterDelegate : ContactsFormatter
    {
        public ContactsFormatterDelegate(ICustomerAccount account)
            : base(account)
        {
        }

        public ContactsFormatterDelegate(ICustomerAccount account, IContactsNAW contactsNAW, IContactsNAW secondContactsNAW)
            : base(account, contactsNAW, secondContactsNAW)
        {
        }

        public ContactsFormatterDelegate(IContactsNAW contactsNAW)
            : base(contactsNAW)
        {
        }

        protected override IContactPerson RetrieveSecondContact(IContact firstContact)
        {
            try
            {
                if (Account.AccountHolders.Count >= 2)
                    foreach (IAccountHolder accountHolder in Account.AccountHolders)
                        if (!accountHolder.IsPrimaryAccountHolder)
                            return (IContactPerson)accountHolder.Contact;

                return null;
            }
            catch (NullReferenceException)
            {
                throw new ApplicationException(
                    string.Format("Could not retrieve secondary account holder for account {0} - {1}.",
                                  Account.Key, Account.DisplayNumberWithName));
            }
        }

        public override string AddressFirstLine
        {
            get { return ContactsNAW.Name; }
        }

        public override string AddressSecondLine
        {
            get { return GetAttentionOfForm(SecondContactsNAW, f => string.Format("inzake ({0})", f)); }
        }

        public override string DearSirForm
        {
            get { return GetDearSirForm(SecondContactsNAW); }
        }
    }
}
