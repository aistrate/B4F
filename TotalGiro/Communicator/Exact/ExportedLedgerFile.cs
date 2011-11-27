using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ExportedLedgerFile : IExportedLedgerFile
    {
        public ExportedLedgerFile() { ledgerEntries = new LedgerEntryCollection(this); }

        public ExportedLedgerFile(string name, string ext, string path, int ordinal) :this()
        {
            this.name = name;
            this.ext = ext;
            this.path = path;
            this.ordinal = ordinal;

        }

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual string Ext
        {
            get { return ext; }
            set { ext = value; }
        }

        public virtual string Path
        {
            get { return path; }
            set { path = value; }
        }

        public virtual int Ordinal
        {
            get { return ordinal; }
            set { ordinal = value; }
        }

        public virtual string FullPathName
        {
            get { return string.Format("{0}{1}{2}.{3}", Path, Name, (Ordinal == 0 ? "" : "_" + Ordinal.ToString("00")), Ext); }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        public virtual ILedgerEntryCollection LedgerEntries
        {
            get
            {
                ILedgerEntryCollection les = (ILedgerEntryCollection)ledgerEntries.AsList();
                if (les.Parent == null) les.Parent = this;
                return les;
            }
        }
        
        private int key;
        private string name;
        private string ext;
        private string path;
        private int ordinal;
        private DateTime creationDate;
        private IDomainCollection<ILedgerEntry> ledgerEntries;
        private IList bagOfledgerEntries = new ArrayList();
    }
}
