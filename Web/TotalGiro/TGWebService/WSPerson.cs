using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("Person")]
    public class WSPerson
    {
        private DateTime birthdate;
        private string sex;
        private string initials;
        private string middlename;
        private string title;
        private string lastname;
        private string sofinumber;
        private string nationality;
        private WSIdentification identification;
        private WSContactDetails contactdetails;

        #region Constructors

        public WSPerson()
        {
        }

        #endregion

        #region Properties

        [XmlElement("Identification")]
        public WSIdentification Identification
        {
            get { return identification; }
            set { identification = value; }
        }

        [XmlElement("ContactDetails")]
        public WSContactDetails ContactDetails
        {
            get { return contactdetails; }
            set { contactdetails = value; }
        }

        [XmlElement("SOFINumber")]
        public string SOFINumber
        {
            get { return sofinumber; }
            set { sofinumber = value; }
        }

        [XmlElement("BirthDate")]
        public DateTime BirthDate
        {
            get { return birthdate; }
            set { birthdate = value; }
        }

        [XmlElement("Sex")]
        public string Sex
        {
            get { return sex; }
            set { sex = value; }
        }

        [XmlElement("Initials")]
        public string Initials
        {
            get { return initials; }
            set { initials = value; }
        }

        [XmlElement("MiddleName")]
        public string MiddleName
        {
            get { return middlename; }
            set { middlename = value; }
        }

        [XmlElement("Title")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [XmlElement("LastName")]
        public string LastName
        {
            get { return lastname; }
            set { lastname = value; }
        }

        [XmlElement("Nationality")]
        public string Nationality
        {
            get { return nationality; }
            set { nationality = value; }
        }

        #endregion


        /// <summary>
        /// Returns a string with all attributes seperated by a |. This can be used to import into an Excel file
        /// </summary>
        /// <returns>A string containing structured request information</returns>
        public string WriteToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}",
                    birthdate.ToString("yyyyMMdd"),sex, initials, middlename, title, lastname, sofinumber,
                    identification != null ? identification.WriteToString() : "", 
                    contactdetails != null ? contactdetails.WriteToString() : "");
        }	
    }
}
