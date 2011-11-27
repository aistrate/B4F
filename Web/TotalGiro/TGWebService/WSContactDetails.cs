using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("ContactDetails")]
    public class WSContactDetails
    {
        private string phonework;
        private string phoneprivate;
        private string phonefax;
        private string phonemobile;
        private string email;
        private WSAddress residentialaddress;
        private WSAddress postaddress;

        #region Constructors

        public WSContactDetails()
        {
        }

        #endregion

        #region Properties

        [XmlElement("Address")]
        public WSAddress ResidentialAddress
        {
            get { return residentialaddress; }
            set { residentialaddress = value; }
        }

        [XmlElement("PostAddress")]
        public WSAddress PostAddress
        {
            get { return postaddress; }
            set { postaddress = value; }
        }

        [XmlElement("EMail")]
        public string EMail
        {
            get { return email; }
            set { email = value; }
        }

        [XmlElement("PhoneMobile")]
        public string PhoneMobile
        {
            get { return phonemobile; }
            set { phonemobile = value; }
        }

        [XmlElement("PhoneFax")]
        public string PhoneFax
        {
            get { return phonefax; }
            set { phonefax = value; }
        }

        [XmlElement("PhonePrivate")]
        public string PhonePrivate
        {
            get { return phoneprivate; }
            set { phoneprivate = value; }
        }

        [XmlElement("PhoneWork")]
        public string PhoneWork
        {
            get { return phonework; }
            set { phonework = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string with all attributes seperated by a |. This can be used to import into an Excel file
        /// </summary>
        /// <returns>A string containing structured request information</returns>
        public string WriteToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                    phonework, phoneprivate, phonefax, phonemobile, email,
                    residentialaddress != null ? residentialaddress.WriteToString() : "",
                    postaddress != null ? postaddress.WriteToString() : "");
        }

        #endregion
    }
}
