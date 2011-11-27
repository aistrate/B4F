using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface IExternalSettlementJournalEntriesCollection : IList<ITradingJournalEntry>
    {
        void Add(ITradingJournalEntry entry);
        IExternalSettlement Parent { get; set; }
        Money TotalExternalSettlementAmount { get; }
        Money TotalExternalSettlementBaseAmount { get; }
    }
}
