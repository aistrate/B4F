using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Utils;
using System.Collections;

namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// Class representing person specific information about a contact object
    /// </summary>
    public class ContactPerson : Contact, IContactPerson
	{
		public ContactPerson() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.CRM.Person">Person</see> class.
        /// </summary>
        public ContactPerson(string FirstName, string MiddleName,Gender ContactGender, INationality nationality,
            string LastName, Address postalAddress, Address residentialAddress, IContactDetails contactDetails )
            : base(LastName, postalAddress, residentialAddress, contactDetails)
		{
			this.FirstName = FirstName;
			this.MiddleName = MiddleName;
			this.Gender = ContactGender;
            this.Nationality = nationality;
		}

        public override ContactTypeEnum ContactType
        {
            get { return ContactTypeEnum.Person; }
        }

        public override string GetBSN
        {
            get { return this.BurgerServiceNummer; }
        }

        public override DateTime GetBirthFounding
        {
            get { return this.DateOfBirth; }
        }

        /// <summary>
        /// Only a few fields are stored
        /// </summary>
        public bool HasMinimumData { get; set; }

        /// <summary>
        /// Get/set title
        /// </summary>
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

        /// <summary>
        /// Get/set first name
        /// </summary>
		public virtual string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

        /// <summary>
        /// Get/set middle name
        /// </summary>
        public virtual string MiddleName
		{
			get { return middleName; }
			set { middleName = value; }
		}

        /// <summary>
        /// Get/set gender
        /// </summary>
        public virtual Gender Gender
		{
			get { return gender; }
			set { gender = value; }
		}

        /// <summary>
        /// Get/set nationality
        /// </summary>
        public virtual INationality Nationality
		{
			get { return nationality; }
			set { nationality = value; }
		}

        /// <summary>
        /// Get/set sofinumber
        /// </summary>
        public virtual string BurgerServiceNummer
		{
            get { return burgerServiceNummer; }
            set { burgerServiceNummer = value; }
		}

        /// <summary>
        /// Get/set date of birth
        /// </summary>
        public virtual DateTime DateOfBirth
		{
			get 
			{
                if (dateOfBirth.Year <= 1800)
                {
                    dateOfBirth = DateTime.MinValue;
                }
				return dateOfBirth; 
			}
			set
            { 
              dateOfBirth = value; 
            }
		}

        /// <summary>
        /// Get/set identification object
        /// </summary>
        public virtual IIdentification Identification
		{
			get { return identification; }
			set { identification = value; }
		}

        public virtual ICompanyContactPersonCollection ContactCompanies
        {
            get
            {
                if (this.contactCompanies == null)
                    this.contactCompanies = new CompanyContactPersonCollection(this, bagOfCompanies);
                return contactCompanies;
            }
            set { contactCompanies = value; }
        }
	
		private string title;
		private string firstName;
		private string middleName;
		private Gender gender;
		private INationality nationality;
        private string burgerServiceNummer;
        private DateTime dateOfBirth = DateTime.MinValue;
		private IIdentification identification;
        private IList bagOfCompanies = new ArrayList();
        private ICompanyContactPersonCollection contactCompanies;

	}	
}
