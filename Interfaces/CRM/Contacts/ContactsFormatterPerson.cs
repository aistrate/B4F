using System;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.CRM.Contacts
{
    public class ContactsFormatterPerson : ContactsFormatter
    {
        public ContactsFormatterPerson(ICustomerAccount account)
            : base(account)
        {
        }

        public ContactsFormatterPerson(ICustomerAccount account, IContactsNAW contactsNAW, IContactsNAW secondContactsNAW)
            : base(account, contactsNAW, secondContactsNAW)
        {
        }

        public ContactsFormatterPerson(IContactsNAW contactsNAW)
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
            get { return GetFullPoliteForm(ContactsNAW); }
        }

        public override string AddressSecondLine
        {
            get
            {
                return (SecondContactsNAW != null ?
                            "en/of " + GetFullPoliteForm(SecondContactsNAW) : "");
            }
        }

        public override string DearSirForm
        {
            get
            {
                string form = GetDearSirForm(ContactsNAW);
                if (SecondContactsNAW != null)
                    form += " en/of " + GetShortPoliteForm(SecondContactsNAW);

                return form;
            }
        }
    }
}
