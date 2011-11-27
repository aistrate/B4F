using System;
namespace B4F.TotalGiro.Communicator.KasBank
{
    public interface IGLDSTDFile
    {
        int Key { get; set; }
        string ExportPath { get; set; }
        IGLDSTDCollection Records { get; }
        string KasMailID { get; set; }
        bool WriteOutFile();
        DateTime CreationDate { get; set; }
        string FullFileName { get; set; }

    }
}
