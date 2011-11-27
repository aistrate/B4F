using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Communicator.Exact
{
    public interface ISubledgerEntryCollection : IList<ISubledgerEntry>
    {
        ILedgerEntry Parent { get; set; }
        void AddSubLedgerEntry(ISubledgerEntry subledgerEntry);
        bool RemoveSubLedgerEntry(ISubledgerEntry subledgerEntry);
    }
}
