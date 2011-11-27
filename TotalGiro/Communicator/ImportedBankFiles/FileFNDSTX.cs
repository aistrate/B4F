using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles
{
    public class FileFNDSTX : FixedWidth
    {
        public override void ImportAllRecords(ImportedFile theFile)
        {
            PrepareFileForReading(theFile);
            ImportedRecord newRecord = null;
            while (!FileAccess.EndOfStream)
            {
                string readLine = FileAccess.ReadLine();

                newRecord = new FNDSTXRecord(readLine);
                theFile.Records.Add(newRecord);

            }
            this.FileAccess.Close();
        }
    }
}
