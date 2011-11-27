using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Communicator.Exact;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface IBankStatement : IJournalEntry
    {
        Money OpenAmount { get; }
        IBankStatement PrevBankStatement { get; }
        DateTime StartingBalanceDate { get; }
        Money StartingBalance { get; }
        Money ClosingBalance { get; set; }
        bool HasClosingBalance { get; set; }
        IImportedBankBalance ImportedBankBalance { get; set; }
        IJournalEntryLine FixedAccountLine { get; }
        Money FixedAccountBalance { get; }
        Money TotalBalance { get; }
        string DisplayOpenAmount { get; }
    }
}
