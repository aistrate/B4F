using System;
namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public interface IDividWepFile
    {
        bool CreateCloseRecord();
        bool CreateOutputFile();
        DateTime CreationDate { get; set; }
        IDividWepRecordCollection Records { get; }
        int FinancialYear { get; set; }
        int Key { get; set; }
        int NoOfRecords { get; set; }
        int TotalDividend { get; set; }
        int TotalTax { get; set; }
        int TotalWep { get; set; }
        string CodeFinance { get; set; }
        string FileName { get; set; }
        string InstelRecord { get; set; }
        string Path { get; set; }
        string SluitRecord { get; set; }
        string SluitRecordType { get; set; }

    }
}
