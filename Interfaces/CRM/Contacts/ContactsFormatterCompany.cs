using System;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.CRM.Contacts
{
    public class ContactsFormatterCompany : ContactsFormatter
    {
        public ContactsFormatterCompany(ICustomerAccount account)
            : base(account)
        {
        }

        public ContactsFormatterCompany(ICustomerAccount account, IContactsNAW contactsNAW, IContactsNAW secondContactsNAW)
            : base(account, contactsNAW, secondContactsNAW)
        {
        }

        public ContactsFormatterCompany(IContactsNAW contactsNAW)
            : base(contactsNAW)
        {
            IContactPerson secondContact = RetrieveSecondContact(contactsNAW.Contact);
            if (secondContact != null)
                SecondContactsNAW = secondContact.CurrentNAW;
        }
        
        protected override IContactPerson RetrieveSecondContact(IContact firstContact)
        {
            try
            {
                ICompanyContactPersonCollection contactPersons = ((IContactCompany)firstContact).CompanyContacts;
                if (contactPersons.Count > 0)
                    return contactPersons[0].ContactPerson;

                return null;
            }
            catch (NullReferenceException)
            {
                throw new ApplicationException(
                    string.Format("Could not retrieve company contact person for contact {0} - {1}.", 
                                  firstContact.Key, firstContact.CurrentNAW.Name));
            }
        }

        public override string AddressFirstLine
        {
            get { return ContactsNAW.Name; }
        }

        public override string AddressSecondLine
        {
            get { return GetAttentionOfForm(SecondContactsNAW); }
        }

        public override string DearSirForm
        {
            get { return GetDearSirForm(SecondContactsNAW); }
        }
    }
}
