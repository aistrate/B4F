using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.CRM.Contacts;
using System.Collections;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// Class representing company specific information about a contact object
    /// </summary>
	public class ContactCompany : Contact, IContactCompany 
	{
		public ContactCompany() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.CRM.ContactCompany">ContactCompany</see> class.
        /// </summary>
        public ContactCompany(string LastName, Address postalAddress, Address residentialAddress, IContactDetails contactDetails, DateTime DateOfFounding, string KvKNumber) 
            : base(LastName, postalAddress, residentialAddress, contactDetails) 
		{
			this.DateOfFounding = DateOfFounding;
			this.KvKNumber = KvKNumber;
		}

        public ContactCompany(string LastName, Address postalAddress, Address residentialAddress, IContactDetails contactDetails, string KvKNumber)
            : base(LastName, postalAddress, residentialAddress, contactDetails)
        {
            this.KvKNumber = KvKNumber;
        }

        public override ContactTypeEnum ContactType
        {
            get { return ContactTypeEnum.Company; }
        }

        public override string GetBSN
        {
            get { return this.KvKNumber; }
        }

        public override DateTime GetBirthFounding
        {
            get { return this.DateOfFounding; }
        }
		
        /// <summary>
        /// Get/set date of founding
        /// </summary>
        public virtual DateTime DateOfFounding
        {
            get
            {
                if (dateOfFounding.Year <= 1800)
                {
                    dateOfFounding = DateTime.MinValue;
                }
                return dateOfFounding;
            }
            set { dateOfFounding = value; }
        }

        /// <summary>
        /// Get/set Dutch chamber of commerce number
        /// </summary>
        public virtual string KvKNumber
		{
			get { return kvKNumber; }
			set { kvKNumber = value; }
		}

        /// <summary>
        /// The collection of Naam-Adres-Woonplaats belonging to contact
        /// </summary>
        public virtual ICompanyContactPersonCollection CompanyContacts
        {
            get
            {
                if (this.companyContacts == null)
                    this.companyContacts = new CompanyContactPersonCollection(this, bagOfContactPerson);
                return companyContacts;
            }
            set { companyContacts = value; }
        }

		private DateTime dateOfFounding = DateTime.MinValue;
		private string kvKNumber;
        private IList bagOfContactPerson = new ArrayList();
        private ICompanyContactPersonCollection companyContacts;

        

}
}
