using System;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    [Flags]
    public enum JournalEntryStati
    {
        None = 0,
        New = 1,
        Open = 2,
        Booked = 4
    }

    public enum JournalEntryTypes
    {
		BankStatement = 1,
		MemorialBooking = 2,
		Transaction = 3,
		ExternalSettlement = 4
    }

    public interface IJournalEntry : ITotalGiroBase<IJournalEntry>, IAuditable
    {
        int Key { get; }
        bool ContainsForeignCashLines { get; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }
        DateTime TransactionDate { get; set; }
        decimal ExchangeRate { get; }
        IGLBookYear BookYear { get; set; }
        IJournal Journal { get; }
        IJournalEntryLineCollection Lines { get; }
        JournalEntryStati Status { get; set; }
        JournalEntryTypes JournalEntryType { get; }
        Money Balance { get; }
        Money Credit { get; }
        Money Debit { get; }
        string BookedBy { get; set; }
        string CreatedBy { get; set; }
        string DisplayStatus { get; }
        string JournalEntryNumber { get; }
        void BookLines();
    }
}
