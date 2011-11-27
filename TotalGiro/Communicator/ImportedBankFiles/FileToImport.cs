using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles
{
    public enum FileTypesToImport
    {
        FixedWidth = 1,
        CommaSeperatedValue = 2
    }

    public abstract class FileToImport
    {


        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }
        public bool BackUpAfterImport { get; set; }

        public virtual string TextFileName
        {
            get { return textFileName; }
            set { textFileName = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }



        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        public string FileSearchString
        {
            get { return fileSearchString; }
            set { fileSearchString = value; }
        }

        public string SourceDirectory
        {
            get { return sourceDirectory; }
            set { sourceDirectory = value; }
        }

        public bool UseNameForImport
        {
            get { return useNameForImport; }
            set { useNameForImport = value; }
        }

        public string NameToUseForImport
        {
            get { return nameToUseForImport; }
            set { nameToUseForImport = value; }
        }

        public string BackupDirectory
        {
            get { return backupDirectory; }
            set { backupDirectory = value; }
        }

        public void GetAllImportFiles()
        {
            DirectoryInfo kasMail = new DirectoryInfo(this.SourceDirectory);
            if (kasMail.Exists)
            {
                this.TheFiles = kasMail.GetFiles(this.FileSearchString);
            }

        }

        public bool CheckIfFileNotImported(ImportedFile ImportFile)
        {
            return this.FilesImported.Contains(ImportFile);
        }


        public FileInfo[] TheFiles
        {
            get { return theFiles; }
            set { theFiles = value; }
        }


        public ImportedFileCollection FilesImported
        {
            get
            {
                if (filesImported == null)
                    this.filesImported = new ImportedFileCollection(bagOfFiles, this);
                return filesImported;
            }
            set { filesImported = value; }
        }


        //public abstract void PrepareFileForReading(FileInfo FileToImport);

        public abstract void ImportAllRecords(ImportedFile theFile);

        public abstract string ImportType { get; }




        #region Private Variables

        private int key;

        private string textFileName;
        private string description;
        private string fileSearchString;
        private string sourceDirectory;
        private bool useNameForImport;
        private string nameToUseForImport;
        private string backupDirectory;
        private FileInfo[] theFiles;
        private ImportedFileCollection filesImported;
        private IList bagOfFiles = new ArrayList();
        private bool enabled;

        #endregion




    }
}
