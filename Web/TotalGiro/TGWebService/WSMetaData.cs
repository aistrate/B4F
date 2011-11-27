using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("MetaData")]
    public class WSMetaData
    {
        private string sessionid;
        private ArrayList listMessage;

        public WSMetaData()
        {
            listMessage = new ArrayList();
        }

        [XmlElement("Messages")]
        public WSMessage[] Messages {
            get {
                WSMessage[] messages = new WSMessage[listMessage.Count];
                listMessage.CopyTo(messages);
                return messages;
            }
            set {
              if( value == null ) return;
              WSMessage[] messages = (WSMessage[])value;
              listMessage.Clear();
              foreach (WSMessage message in messages)
                  listMessage.Add(message);
            }
        }

        public int AddMessage( WSMessage message ) {
            return listMessage.Add( message );
        }

       
        [XmlElement("SessionId")]
        public string SessionId
        {
            get { return sessionid; }
            set { sessionid = value; }
        }

    }
}
