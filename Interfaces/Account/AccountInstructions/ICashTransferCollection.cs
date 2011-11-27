using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Accounts.Instructions
{
    public interface ICashTransferCollection : IList<IJournalEntryLine>
    {
        IInstruction Parent { get; }
        Money TotalTransferAmount { get; }
        void AddTransfers(IList<IJournalEntryLine> transfers);
        void DisConnect();
    }
}
