using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("OPALInput")]
    public class WSOPALInput
    {

        Results results = null;

        [XmlElement("Results")]
        public Results outputresults
        {
            get {return results;}
            set { results = value; }
        }
        
    }

    [Serializable]
    [XmlRoot("OPALOutput")]
    public class WSOPALOutput
    {
        Project project = null;

        [XmlElement("Project")]
        public Project OutputProject
        {
            get {return project;}
            set {project = value;}
        }
    }

    [Serializable]
    [XmlRoot("OPALData")]
    public class WSOPALData
    {
        WSOPALInput opalinput = null;
        WSOPALOutput opaloutput = null;


        [XmlElement("OPALInput")]
        public WSOPALInput OpalInput
        {
            get {return opalinput;}
            set { opalinput = value; }
        }

        [XmlElement("OPALOutput")]
        public WSOPALOutput OpalOutput
        {
            get { return opaloutput; }
            set { opaloutput = value; }
        }
    }
}
