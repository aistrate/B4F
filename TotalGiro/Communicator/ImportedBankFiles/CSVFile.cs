using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles
{
    public abstract class CSVFile : FileToImport
    {
        internal StreamReader FileAccess { get; set; }

        public override string ImportType
        {
            get { return "CSV File"; }
        }

        internal void PrepareFileForReading(ImportedFile theFile)
        {
            this.FileAccess = theFile.ImportedFileInfo.OpenText();

        }



    }
}
