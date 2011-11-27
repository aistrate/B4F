using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface IExternalSettlementJournalLinesCollection : IList<IJournalEntryLine>
    {
        void Add(IJournalEntryLine line);
        IExternalSettlement Parent { get; set; }
        Money Balance { get; }
        Money BaseBalance { get; }
    }
}
