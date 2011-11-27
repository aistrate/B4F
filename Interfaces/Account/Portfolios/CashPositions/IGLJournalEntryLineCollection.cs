using System;
using System.Collections.Generic;
using B4F.TotalGiro.GeneralLedger.Journal;
namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface IGLJournalEntryLineCollection : IList<IJournalEntryLine>
    {
        ICashSubPosition Parent { get; set; }
        void AddLine(IJournalEntryLine item);
    }
}
