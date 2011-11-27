using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace B4F.TotalGiro.Dal.Utils
{
    public class ExportToExcel
    { // you could have other overloads if you want to get creative... 
        private const string REFERENCEDBFILENAME = "B4F.TotalGiro.Dal.Resources.Excel.xsl";
        public static string CreateWorkbook(DataSet ds)
        {
            XmlDataDocument xmlDataDoc = new XmlDataDocument(ds);
            XslTransform xt = new XslTransform();
            StreamReader reader = new StreamReader(typeof(ExportToExcel).Assembly.GetManifestResourceStream(REFERENCEDBFILENAME));
            XmlTextReader xRdr = new XmlTextReader(reader); 
            xt.Load(xRdr, null, null); 
            StringWriter sw = new StringWriter(); 
            xt.Transform(xmlDataDoc, null, sw, null); 
            return sw.ToString();
        }
    }
}
