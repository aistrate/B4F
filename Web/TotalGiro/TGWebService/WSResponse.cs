using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("TGResponse")]
    public class WSResponse
    {
        private WSMetaData metadata;

        public WSMetaData MetaData
        {
            get { return metadata; }
            set { metadata = value; }
        }
	
    }
}
