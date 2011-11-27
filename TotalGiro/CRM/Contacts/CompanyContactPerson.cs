using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.CRM.Contacts;

namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// Class represents person responsible for a company
    /// </summary>
    public class CompanyContactPerson : ICompanyContactPerson
	{

		public CompanyContactPerson()
		{

		}

        /// <summary>
        /// Get/set identifier
        /// </summary>
        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.CRM.CompanyContact">CompanyContact</see> class.
        /// </summary>
        public CompanyContactPerson(IContactPerson Person, IContactCompany Company)
		{
            this.ContactPerson = Person;
            this.Company = Company;
		}

        /// <summary>
        /// Get/set primary contact flag
        /// </summary>
        public AuthorizedSignatureEnum AuthorizedSignature
		{
            get { 
                    if (authorizedSignature == 1)
                        return AuthorizedSignatureEnum.Independent;
                    else if (authorizedSignature == 2)
                        return AuthorizedSignatureEnum.Common;
                    else
                        return AuthorizedSignatureEnum.None;
                }
            set { 
                    if (value == AuthorizedSignatureEnum.None)
                    {
                        authorizedSignature = Convert.ToInt16(AuthorizedSignatureEnum.None);
                    }
                    else if (value == AuthorizedSignatureEnum.Independent)
                    {
                        authorizedSignature = Convert.ToInt16(AuthorizedSignatureEnum.Independent);
                    }
                    else if (value == AuthorizedSignatureEnum.Common)
                    {
                        authorizedSignature = Convert.ToInt16(AuthorizedSignatureEnum.Common);
                    }
                }
		}

        /// <summary>
        /// Get/set company 
        /// </summary>
        public IContactCompany Company
        {
            get { return company; }
            set { company = value; }
        }

        /// <summary>
        /// Get/set person responsible
        /// </summary>
        public IContactPerson ContactPerson
		{
			get { return person; }
			set { person = value; }
		}

        private int key;
        private IContactPerson person;
        private IContactCompany company;
        private Int16 authorizedSignature;

        
	}
}
