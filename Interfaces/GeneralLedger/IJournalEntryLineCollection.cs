using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface IJournalEntryLineCollection : IList<IJournalEntryLine>
    {
        int GetNextLineNumber();
        IJournalEntryLine GetLineById(int key);
        void ShiftLineNumbers(int startFromLine, int shiftBy);
        IJournalEntryLine FixedAccountLine { get; }
        IJournalEntry Parent { get; set; }
        void AddJournalEntryLine(IJournalEntryLine journalEntryLine);
    }
}
