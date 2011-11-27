using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files
{
    public class ImportedFile
    {
        public ImportedFile(FileInfo UnReadFile, FileToImport Parent)
        {
            this.Parent = Parent;
            this.ImportedFileInfo = UnReadFile;

            this.FileName = this.ImportedFileInfo.FullName;
            this.FileShortName = this.ImportedFileInfo.Name;
            this.FileCreationDate = this.ImportedFileInfo.CreationTime;
            this.DateFileImported = DateTime.Now;
        }

        protected ImportedFile()
        {

        }

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public FileToImport Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string FileShortName
        {
            get { return fileShortName; }
            set { fileShortName = value; }
        }

        public DateTime FileCreationDate
        {
            get { return fileCreationDate; }
            set { fileCreationDate = value; }
        }

        public int NumberOfRecords
        {
            get { return numberOfRecords; }
            set { numberOfRecords = value; }
        }

        public DateTime DateFileImported
        {
            get { return dateFileImported; }
            set { dateFileImported = value; }
        }

        public Records.ImportedRecordCollection Records
        {
            get
            {
                if (records == null)
                    this.records = new Records.ImportedRecordCollection(bagOfRecords, this);
                return records;
            }
            set { records = value; }
        }

        public bool ImportAllRecords()
        {
            this.Parent.ImportAllRecords(this);


            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ImportedFile))
            {
                return false;
            }
            return this == (ImportedFile)obj;
        }

        public static bool operator ==(ImportedFile lhs, ImportedFile rhs)
        {
            if ((Object)lhs == null || (Object)rhs == null)
            {
                if ((Object)lhs == null && (Object)rhs == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (lhs.FileShortName.ToUpper() == rhs.FileShortName.ToUpper())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public static bool operator !=(ImportedFile lhs, ImportedFile rhs)
        {
            if ((Object)lhs == null || (Object)rhs == null)
            {
                if ((Object)lhs == null && (Object)rhs == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (lhs.FileShortName.ToUpper() != rhs.FileShortName.ToUpper())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }





        public FileInfo ImportedFileInfo
        {
            get { return importedFileInfo; }
            set { importedFileInfo = value; }
        }



        private int key;
        private FileToImport parent;
        private FileInfo importedFileInfo;
        private string fileName;
        private string fileShortName;
        private DateTime dateFileImported = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private int numberOfRecords;
        private DateTime fileCreationDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));
        private Records.ImportedRecordCollection records;
        private IList bagOfRecords = new ArrayList();

    }
}

