using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
	[Serializable]
    [XmlRoot("TGRequest")]
    public class WSRequest
    {
        private string version = "";
        private string username = "";
        private string password = "";
        private WSAccountRequestData updaterequestdata = null;

        #region Constructors

        public WSRequest()
        {
        }

        #endregion


        #region Properties

        [XmlAttribute("version")]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        [XmlAttribute("username")]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [XmlAttribute("password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        #endregion

        #region Public Method

        [XmlElement("UpdateRequestData")]
        public WSAccountRequestData UpdateRequestData
        {
            get { return updaterequestdata; }
            set { updaterequestdata = value; }
        }

        #endregion


    }
}
