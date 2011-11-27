using System;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface IExternalSettlement
    {
        int Key { get; set; }
        DateTime SettlementDate { get; set; }
        IMemorialBooking MemorialBooking { get; set; }
        IExternalSettlementJournalLinesCollection BankSettlements { get; }
        IExternalSettlementJournalEntriesCollection TradeStatements { get; }
        Money SettleDifference { get; }
        bool Settle(IJournal journal, string nextJournalEntryNumber, IGLAccount settleDiffGLAccount);
        DateTime CreationDate { get; }
    }
}
