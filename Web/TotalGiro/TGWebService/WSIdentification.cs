using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("Identification")]
    public class WSIdentification
    {

        private string type;
        private DateTime validityperiod;
        private string number;

        #region Constructors

        public WSIdentification()
        {
        }

        #endregion

        #region Properties

        [XmlElement("Number")]
        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        [XmlElement("ValidityPeriod")]
        public DateTime ValidityPeriod
        {
            get { return validityperiod; }
            set { validityperiod = value; }
        }
	
        [XmlElement("Type")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        #endregion

        #region Publuic Methods
        /// <summary>
        /// Returns a string with all attributes seperated by a |. This can be used to import into an Excel file
        /// </summary>
        /// <returns>A string containing structured request information</returns>
        public string WriteToString()
        {
            return string.Format("{0}|{1}|{2}",
                    type, validityperiod.ToString("yyyyMMdd"), number);
        }

        #endregion
    }


}
