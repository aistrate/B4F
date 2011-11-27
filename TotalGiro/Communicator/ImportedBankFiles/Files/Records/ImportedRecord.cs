using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public abstract class ImportedRecord
    {


        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public ImportedFile ImportedFile
        {
            get { return importedFile; }
            set { importedFile = value; }
        }



        private int key;
        private ImportedFile importedFile;
        internal DateTime nullDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value.Add(new TimeSpan(10, 0, 0, 0));


    }
}
