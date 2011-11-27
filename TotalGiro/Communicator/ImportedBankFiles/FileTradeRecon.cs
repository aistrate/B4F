using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files;
using B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records;


namespace B4F.TotalGiro.Communicator.ImportedBankFiles
{
    public class FileTradeRecon : CSVFile
    {
        public override void ImportAllRecords(ImportedFile theFile)
        {
            PrepareFileForReading(theFile);
            ImportedRecord newRecord = null;
            while (!FileAccess.EndOfStream)
            {
                string readLine = FileAccess.ReadLine();

                newRecord = new TradeReconRecord(readLine);
                theFile.Records.Add(newRecord);

            }
            this.FileAccess.Close();
        }
    }
}
