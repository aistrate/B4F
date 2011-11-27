using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles
{
    public class FileEFFMTX : FixedWidth
    {


        public override void ImportAllRecords(ImportedFile theFile)
        {
            PrepareFileForReading(theFile);
            ImportedRecord newRecord = null;
            while (!FileAccess.EndOfStream)
            {
                string readLine = FileAccess.ReadLine();

                newRecord = new EFFMTX(readLine);
                theFile.Records.Add(newRecord);

            }
            this.FileAccess.Close();
        }

    }
}
