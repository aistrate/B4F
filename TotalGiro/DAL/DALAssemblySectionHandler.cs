using System;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace B4F.TotalGiro.Dal
{
    public class DALAssemblySectionHandler : IConfigurationSectionHandler
    {

        public object Create(object parent, object configContext, XmlNode section)
        {

            XmlSerializer test = new XmlSerializer(typeof(AssembliesToHibernate));
            StringReader ss = new StringReader(section.OuterXml);
            AssembliesToHibernate mnh = (AssembliesToHibernate)test.Deserialize(ss);

            string[] nn = new string[mnh.Assemblies.Count];
            mnh.Assemblies.CopyTo(nn, 0);

            return nn;
        }

        [XmlRoot("AssembliesToHibernate")]
        public class AssembliesToHibernate
        {
            public System.Collections.Specialized.StringCollection Assemblies
            {
                get { return assemblies; }
                set { assemblies = value; }
            }

            private System.Collections.Specialized.StringCollection assemblies;

        }

    }
}

