using System;
using System.Collections.Generic;
using B4F.TotalGiro.GeneralLedger.Journal;
namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IJournalEntryLineCollection : IList<IJournalEntryLine>
    {
        void AddLine( IJournalEntryLine journalEntryLine);
        ISubledgerEntry Parent { get; set; }
        bool RemoveLine(IJournalEntryLine journalEntryLine);
    }
}
