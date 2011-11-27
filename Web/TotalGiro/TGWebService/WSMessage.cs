using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    public enum WSMessageSeverity { MESSAGE=0, WARN = 1, ERR = 2 }

    [Serializable]
    [XmlRoot("Message")]
    public class WSMessage
    {
        private string description;
        private int messageid;
        private WSMessageSeverity severity;

        #region Constructors

        public WSMessage(WSMessageSeverity severity, int messageid, string description)
        {
            this.severity = severity;
            this.messageid = messageid;
            this.description = description;
        }

        public WSMessage()
        {

        }

        #endregion

        #region Properties

        [XmlAttribute("severity")]
        public WSMessageSeverity Severity
        {
            get { return severity; }
            set { severity = value; }
        }
	
        [XmlAttribute("id")]
        public int MessageId
        {
            get { return messageid; }
            set { messageid = value; }
        }

        [XmlElement("Description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}({2})",severity.ToString(), description,messageid.ToString());
        }

    }
}
