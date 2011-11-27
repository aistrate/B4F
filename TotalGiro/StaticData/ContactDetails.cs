using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// Class holding communication means
    /// </summary>
	public class ContactDetails : IContactDetails
	{

		public ContactDetails() 	{		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.CRM.ContactDetails">ContactDetails</see> class.
        /// </summary>
        public ContactDetails(TelephoneNumber telephone, TelephoneNumber fax, TelephoneNumber mobile, 
								TelephoneNumber telephoneAH, string email, bool sendnewsitem)
		{
			this.telephone = telephone;
			this.fax = fax;
			this.mobile = mobile;
			this.telephoneAH = telephoneAH;
			this.email = email;
            this.sendnewsitem = sendnewsitem;
		}

		private string email;
        private bool sendnewsitem;

        /// <summary>
        /// Get/set email address
        /// </summary>
        public virtual string Email
		{
			get { return email; }
			set { email = value; }
		}

        public virtual bool SendNewsItem
        {
            get { return sendnewsitem; }
            set { sendnewsitem = value; }
        }

        /// <summary>
        /// Get/set fax number
        /// </summary>
        public virtual TelephoneNumber Fax
		{
			get { return fax; }
			set { fax = value; }
		}

        /// <summary>
        /// Get/set telephone
        /// </summary>
        public virtual TelephoneNumber Telephone
		{
            get { return telephone; }
			set { telephone = value; }
		}

        /// <summary>
        /// Get/set mobile number
        /// </summary>
        public virtual TelephoneNumber Mobile
		{
            get { return mobile; }
			set { mobile = value; }
		}

        /// <summary>
        /// Get/set telephone after hours
        /// </summary>
        public virtual TelephoneNumber TelephoneAH
		{
            get { return telephoneAH; }
			set { telephoneAH = value; }
		}
	
        /// <summary>
        /// Overriden string composition
        /// </summary>
        /// <returns>Telephone number</returns>
		public override string ToString()
		{
            if (Telephone != null)
                return Telephone.ToString();
            else
                return String.Empty;
		}

		private TelephoneNumber telephone;
		private TelephoneNumber fax;
		private TelephoneNumber mobile;
		private TelephoneNumber telephoneAH;

	}
}
