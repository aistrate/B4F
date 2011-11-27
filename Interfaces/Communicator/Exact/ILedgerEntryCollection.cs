using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Communicator.Exact
{
    public interface ILedgerEntryCollection :IList<ILedgerEntry>
    {
        void AddLedgerEntry(ILedgerEntry ledgerEntry);
        IExportedLedgerFile Parent { get; set; }
        bool RemoveLedgerEntry(ILedgerEntry ledgerEntry);
    }
}
