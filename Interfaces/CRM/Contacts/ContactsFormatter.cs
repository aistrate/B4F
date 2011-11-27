using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.CRM.Contacts
{
    /// <summary>
    /// Class that (with its descendands) takes care of correctly assigning the first and the second contact, and formatting their names
    /// and addresses, whether the primary account holder is a person or a company.
    /// </summary>
    public abstract class ContactsFormatter
    {
        /// <summary>
        /// Static method used by Notas and Financial Reports when created, and by Customer Accounts 
        /// (e.g. when printing Financial Reports; when showing account details on page Client Portfolio).
        /// If argument is an 'en/of' account, the method will use the account's current primary and secondary ContactNAWs.
        /// </summary>
        public static ContactsFormatter CreateContactsFormatter(ICustomerAccount account)
        {
            switch (getContactType(account))
            {
                case ContactTypeEnum.Company:
                    return new ContactsFormatterCompany(account);
                case ContactTypeEnum.Delegate:
                    return new ContactsFormatterDelegate(account);
                default:
                    return new ContactsFormatterPerson(account);
            }
        }

        /// <summary>
        /// Static method used by Notas when printed. It uses the (one or two) ContactsNAWs stored on the Nota at creation time.
        /// </summary>
        public static ContactsFormatter CreateContactsFormatter(ICustomerAccount account, IContactsNAW contactsNAW, IContactsNAW secondContactsNAW)
        {
            switch (getContactType(contactsNAW))
            {
                case ContactTypeEnum.Company:
                    return new ContactsFormatterCompany(account, contactsNAW, secondContactsNAW);
                case ContactTypeEnum.Delegate:
                    return new ContactsFormatterDelegate(account, contactsNAW, secondContactsNAW);
                default:
                    return new ContactsFormatterPerson(account, contactsNAW, secondContactsNAW);
            }
        }

        /// <summary>
        /// Static method used by Contacts (e.g. when sending 'New user name' letters,
        /// 'New/Changed password' emails, or 'New notas/financial reports created' emails).
        /// </summary>
        public static ContactsFormatter CreateContactsFormatter(IContactsNAW contactsNAW)
        {
            switch (getContactType(contactsNAW))
            {
                case ContactTypeEnum.Company:
                    return new ContactsFormatterCompany(contactsNAW);
                case ContactTypeEnum.Delegate:
                    return new ContactsFormatterDelegate(contactsNAW);
                default:
                    return new ContactsFormatterPerson(contactsNAW);
            }
        }

        private static ContactTypeEnum getContactType(ICustomerAccount account)
        {
            try
            {
                return account.AccountHolders.PrimaryAccountHolder.Contact.ContactType;
            }
            catch (NullReferenceException)
            {
                throw new ApplicationException(
                    string.Format("Could not retrieve primary contact for account {0} - {1}.", account.Key, account.DisplayNumberWithName));
            }
        }

        private static ContactTypeEnum getContactType(IContactsNAW contactsNAW)
        {
            try
            {
                return contactsNAW.Contact.ContactType;
            }
            catch (NullReferenceException)
            {
                throw new ApplicationException(
                    string.Format("Could not retrieve contact for contact NAW {0} - {1}.", contactsNAW.Key, contactsNAW.Name));
            }
        }

        // Constructor
        protected ContactsFormatter(ICustomerAccount account)
        {
            this.account = account;

            try
            {
                IContact firstContact = Account.AccountHolders.PrimaryAccountHolder.Contact;
                IContactPerson secondContact = RetrieveSecondContact(firstContact);

                this.contactsNAW = firstContact.CurrentNAW;
                if (secondContact != null)
                    this.secondContactsNAW = secondContact.CurrentNAW;
            }
            catch (NullReferenceException)
            {
                throw new ApplicationException(
                    string.Format("Could not retrieve account holder information for account {0} - {1}.",
                                  account.Key, account.DisplayNumberWithName));
            }
        }

        // Constructor
        protected ContactsFormatter(ICustomerAccount account, IContactsNAW contactsNAW, IContactsNAW secondContactsNAW)
        {
            this.account = account;
            this.contactsNAW = contactsNAW;
            this.secondContactsNAW = secondContactsNAW;
        }

        // Constructor
        protected ContactsFormatter(IContactsNAW contactsNAW)
        {
            this.contactsNAW = contactsNAW;
        }

        public ICustomerAccount Account
        {
            get { return account; }
        }

        public IContactsNAW ContactsNAW
        {
            get { return contactsNAW; }
            protected set { contactsNAW = value; }
        }

        public IContactsNAW SecondContactsNAW
        {
            get { return secondContactsNAW; }
            protected set { secondContactsNAW = value; }
        }

        /// <summary>
        /// First line of the postal address on Notas, formatted in the correct Dutch style.
        /// It contains the name of the primary account holder, who can be either a person or a company.
        /// </summary>
        public abstract string AddressFirstLine { get; }

        /// <summary>
        /// Second line of the postal address on Notas, formatted in the correct Dutch style.
        /// It contains the name of the secondary account holder (if primary holder is a person) preceded by "en/of", 
        /// or the name of the contact person (if primary holder is a company) preceded by "T.a.v.".
        /// This always represents a person (or else is empty).
        /// </summary>
        public abstract string AddressSecondLine { get; }

        /// <summary>
        /// Used on Deposit Notas.
        /// </summary>
        public abstract string DearSirForm { get; }

        public Address Address
        {
            get { return ContactsNAW.PostalAddress != null ?  ContactsNAW.PostalAddress : ContactsNAW.ResidentialAddress; }
        }

        public void AssertAddressIsComplete()
        {
            if (Address == null)
                throw new ApplicationException(
                                string.Format("Neither postal nor residential address could be found for contact NAW {0}.", ContactsNAW.Key));
            
            Address.AssertIsComplete();
        }

        public string FullName { get { return AddressFirstLine; } }

        public string FullAddress
        {
            get { return GetFullAddress(AddressFirstLine, AddressSecondLine, Address); }
        }

        protected abstract IContactPerson RetrieveSecondContact(IContact firstContact);

        protected static string GetFullPoliteForm(IContactsNAW contactsNAW)
        {
            if (contactsNAW != null)
            {
                if (contactsNAW.Contact != null && contactsNAW.Contact.ContactType == ContactTypeEnum.Person)
                {
                    IContactPerson contact = (IContactPerson)contactsNAW.Contact;
                    return GetFullPoliteForm(contact.Gender, contact.FirstName, contact.MiddleName, contactsNAW.Name);
                }
                else
                    return contactsNAW.Name;
            }
            else
                return string.Empty;
        }

        internal protected static string GetFullPoliteForm(Gender gender, string firstName, string middleName, string lastName)
        {
            string genderDutch = (gender == Gender.Male   ? "De heer" :
                                 (gender == Gender.Female ? "Mevrouw" : 
                                                            "De heer/mevrouw"));

            return new string[] { genderDutch, firstName, middleName, lastName }.WhereNonEmpty()
                                                                                .JoinStrings(" ");
        }

        protected static string GetShortPoliteForm(IContactsNAW contactsNAW)
        {
            if (contactsNAW != null)
            {
                IContactPerson contact = (IContactPerson)contactsNAW.Contact;
                return GetShortPoliteForm(contact.Gender, contact.MiddleName, contactsNAW.Name);
            }
            else
                return "heer/mevrouw";
        }

        protected static string GetShortPoliteForm(Gender gender, string middleName, string lastName)
        {
            string genderDutch = (gender == Gender.Male   ? "heer" :
                                 (gender == Gender.Female ? "mevrouw" :
                                                            "heer/mevrouw"));

            return new string[] { genderDutch, middleName, lastName }.WhereNonEmpty()
                                                                     .JoinStrings(" ");
        }

        protected static string GetDearSirForm(IContactsNAW contactsNAW)
        {
            return "Geachte " + GetShortPoliteForm(contactsNAW);
        }

        internal protected static string GetDearSirForm(Gender gender, string middleName, string lastName)
        {
            return "Geachte " + GetShortPoliteForm(gender, middleName, lastName);
        }

        protected static string GetAttentionOfForm(IContactsNAW contactsNAW)
        {
            return GetAttentionOfForm(contactsNAW, null);
        }

        protected static string GetAttentionOfForm(IContactsNAW contactsNAW, Func<string, string> addCustomPrefix)
        {
            if (contactsNAW != null)
            {
                IContactPerson contact = (IContactPerson)contactsNAW.Contact;
                return GetAttentionOfForm(contact.Gender, contact.FirstName, contact.MiddleName, contactsNAW.Name, addCustomPrefix);
            }
            else
                return "";
        }

        internal protected static string GetAttentionOfForm(Gender gender, string firstName, string middleName, string lastName)
        {
            return GetAttentionOfForm(gender, firstName, middleName, lastName, null);
        }

        protected static string GetAttentionOfForm(Gender gender, string firstName, string middleName, string lastName,
                                                   Func<string, string> addCustomPrefix)
        {
            string fullPoliteForm;
            if (gender == Gender.Unknown && firstName == string.Empty)
                fullPoliteForm = lastName;        // not a person, but a department etc.
            else
                fullPoliteForm = GetFullPoliteForm(gender, firstName, middleName, lastName);

            return addCustomPrefix != null ? addCustomPrefix(fullPoliteForm) : string.Format("T.a.v. {0}", fullPoliteForm);
        }

        internal protected static string GetFullAddress(string addressFirstLine, string addressSecondLine, Address address)
        {
            try
            {
                List<string> addressLines = new List<string>()
                {
                    addressFirstLine,
                    addressSecondLine
                };

                if (address != null)
                    addressLines.AddRange(new string[]
                    {
                        address.StreetAddressLine,
                        address.CityAddressLine,
                        address.CountryAddressLine
                    });

                if (address == null || !address.IsComplete)
                    addressLines.Add("(address incomplete)");
                
                return addressLines.WhereNonEmpty()
                                   .JoinStrings(" ", "    ", "\n");
            }
            catch
            {
                return "???";
            }
        }

        private ICustomerAccount account;
        private IContactsNAW contactsNAW;
        private IContactsNAW secondContactsNAW;
    }
}
