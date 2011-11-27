using System;
namespace B4F.DataMigration.CleopatraMigration
{
    public interface ICleoJournalEntryLine
    {
        B4F.TotalGiro.Accounts.IAccountTypeInternal Account { get; set; }
        string AccType { get; set; }
        string Badge { get; set; }
        decimal Begin { get; set; }
        DateTime BookDate { get; set; }
        B4F.TotalGiro.Instruments.Money Saldo { get; set; }
        string Currency { get; set; }
        decimal Debit { get; set; }
        string Description { get; set; }
        DateTime EntryDat { get; set; }
        string Exchange { get; set; }
        string Family { get; set; }
        string Firm { get; set; }
        string JournalAccount { get; set; }
        string JournalNumber { get; set; }
        int Key { get; set; }
        string RefNr { get; set; }
        decimal Credit { get; set; }
        string Symbol { get; set; }
        DateTime UserDate { get; set; }
        string UserID { get; set; }
        DateTime ValueDate { get; set; }
        int Volgnr { get; set; }
        bool Migrated { get; set; }
    }
}
