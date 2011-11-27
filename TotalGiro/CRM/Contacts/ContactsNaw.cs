using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.CRM
{
    public class ContactsNAW : IContactsNAW
    {
        public ContactsNAW() { }

        public ContactsNAW(string name, Address postalAddress, Address residentialAddress )
        {
            this.Name = name;
            this.PostalAddress = postalAddress;
            this.ResidentialAddress = residentialAddress;

        }

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// The date that the ContactsNAW was created.
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        public override bool Equals(object obj)
        {
            IContactsNAW otherNAW = obj as IContactsNAW;
            if (otherNAW != null)
            {
                if (this.ResidentialAddress == null && otherNAW.ResidentialAddress == null &&
                    this.PostalAddress == null && otherNAW.PostalAddress == null)
                    return this.Name == otherNAW.Name;
                
                if ((this.ResidentialAddress == null && otherNAW.ResidentialAddress != null) ||
                    (this.ResidentialAddress != null && otherNAW.ResidentialAddress == null) ||
                    (this.PostalAddress == null && otherNAW.PostalAddress != null) ||
                    (this.PostalAddress != null && otherNAW.PostalAddress == null))
                    return false;
                
                return (this.Name == otherNAW.Name &&
                    this.ResidentialAddress.City == otherNAW.ResidentialAddress.City &&
                    this.ResidentialAddress.Country == otherNAW.ResidentialAddress.Country &&
                    this.ResidentialAddress.HouseNumber == otherNAW.ResidentialAddress.HouseNumber &&
                    this.ResidentialAddress.HouseNumberSuffix == otherNAW.ResidentialAddress.HouseNumberSuffix &&
                    this.ResidentialAddress.PostalCode == otherNAW.ResidentialAddress.PostalCode &&
                    this.ResidentialAddress.Street == otherNAW.ResidentialAddress.Street &&
                    this.PostalAddress.City == otherNAW.PostalAddress.City &&
                    this.PostalAddress.Country == otherNAW.PostalAddress.Country &&
                    this.PostalAddress.HouseNumber == otherNAW.PostalAddress.HouseNumber &&
                    this.PostalAddress.HouseNumberSuffix == otherNAW.PostalAddress.HouseNumberSuffix &&
                    this.PostalAddress.PostalCode == otherNAW.PostalAddress.PostalCode &&
                    this.PostalAddress.Street == otherNAW.PostalAddress.Street);
            }
            else
                return false;
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

	    public virtual IContact Contact
	    {
		    get { return contact;}
		    set { contact = value;}
	    }


        public virtual Address ResidentialAddress
        {
            get { return residentialAddress; }
            set { residentialAddress = value; }
        }


        public virtual Address PostalAddress
        {
            get { return postalAddress; }
            set { postalAddress = value; }
        }

        public ContactsFormatter Formatter
        {
            get
            {
                if (formatter == null)
                    formatter = ContactsFormatter.CreateContactsFormatter(this);
                return formatter;
            }
        }

        #region Private Variables

        private int key;
        private IContact contact;
        private Address residentialAddress;
        private Address postalAddress;
        private string name;
        private DateTime creationDate;
        private DateTime lastUpdated;
        private ContactsFormatter formatter;

        #endregion

    }
}
