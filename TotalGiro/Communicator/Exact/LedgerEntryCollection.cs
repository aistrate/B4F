using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class LedgerEntryCollection : TransientDomainCollection<ILedgerEntry>, ILedgerEntryCollection
    {
        public LedgerEntryCollection() : base() { }

        public LedgerEntryCollection(IExportedLedgerFile parent)
            : base()
        {
            this.Parent = parent;
        }

        public void AddLedgerEntry(ILedgerEntry ledgerEntry)
        {
            ledgerEntry.ExportedLedgerFile = Parent;
            base.Add(ledgerEntry);
        }

        public bool RemoveLedgerEntry(ILedgerEntry ledgerEntry)
        {
            ledgerEntry.ExportedLedgerFile = null;
            return base.Remove(ledgerEntry);
        }

        public IExportedLedgerFile Parent { get; set; }

    }
}
