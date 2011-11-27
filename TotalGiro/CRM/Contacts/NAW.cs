using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.CRM
{
    public class NAW : INAW
    {
        public NAW()
        {

        }        

        public int Key
        {
            get { return key; }
            set { key = value; }
        }	

        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }        

        public string ContactTitle
        {
            get { return contactTitle; }
            set { contactTitle = value; }
        }	

        /// <summary>
        /// Get/set residential address
        /// </summary>
        public Address ResidentialAddress
        {
            get { return residentialAddress; }
            set { residentialAddress = value; }
        }

        /// <summary>
        /// Get/set postal address
        /// </summary>
        public Address PostalAddress
        {
            get { return postalAddress; }
            set { postalAddress = value; }
        }

        #region Private Variables

        private Address postalAddress;
        private Address residentialAddress;
        private DateTime creationDate;
        private string contactTitle;
        private int key;
        
        #endregion

    }
}
