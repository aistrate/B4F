using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles
{
    public abstract class FixedWidth : FileToImport
    {



        internal StreamReader FileAccess
        {
            get { return fileAccess; }
            set { fileAccess = value; }
        }

        public override string ImportType
        {
            get { return "Fixed Width"; }
        }





        internal void PrepareFileForReading(ImportedFile theFile)
        {
            this.FileAccess = theFile.ImportedFileInfo.OpenText();

        }



        private StreamReader fileAccess;

    }
}
