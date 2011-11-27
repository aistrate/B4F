using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace B4F.TotalGiro.Communicator.KasBank
{
    public class GLDSTDFile : IGLDSTDFile
    {
        private const string FILENAME = @"GLDSTD.";
        private const int SIZEOFGLDSTD = 2000;
        private const int DATABLOCKSIZE = 80;

        public GLDSTDFile(string ExportPath, string KasMailID)
        {
            this.ExportPath = ExportPath;
            this.KasMailID = KasMailID;
            this.FullFileName = ExportPath + @"ZEND.DAT" + "." + DateTime.Now.ToString("yyyyMMddHHmmss");
            this.CreationDate = DateTime.Now;
        }

        public GLDSTDFile() { }
        public string ExportPath { get; set; }
        public string KasMailID { get; set; }
        public string CurrentTime { get; set; }
        public int Key { get; set; }
        public string FullFileName { get; set; }
        public DateTime CreationDate { get; set; }



        public virtual IGLDSTDCollection Records
        {
            get
            {
                if (this.records == null)
                    this.records = new GLDSTDCollection(this, bagOfRecords);
                return records;
            }
            internal set { records = value; }
        }

        public bool WriteOutFile()
        {
            if(!File.Exists(this.FullFileName))
            {
                this.CurrentTime = DateTime.Now.ToString("yyMMddHHmmss");
                using (StreamWriter sw = new StreamWriter(this.FullFileName,false))
                {
                    sw.Write(this.getHeader());
                    foreach (GLDSTD gldstd in Records)
                        sw.Write(gldstd.ToString());
                    sw.Write(this.getTrailer());
                    sw.Close();                    
                }
            }
            return true;
        }

        public string noOfBytes
        {
            get
            {
                return (((this.records.Count * SIZEOFGLDSTD) / DATABLOCKSIZE + 2)).ToString().PadLeft(10, '0');
            }
        }        

        private string getHeader()
        {
            return ("##H##2000KASASS" + this.KasMailID + this.CurrentTime.ToString() + "GLDSTD                               " + DateTime.Now.ToString("yyyyMMdd")).PadRight(80, ' ');
        }
        private string getTrailer()
        {
            return ("##T######KASASS" + this.KasMailID + this.CurrentTime.ToString() + "GLDSTD" + noOfBytes).PadRight(80, ' ');
        }

        private IGLDSTDCollection records;
        private IList bagOfRecords = new ArrayList();
        private DateTime creationDate;

    }
}
