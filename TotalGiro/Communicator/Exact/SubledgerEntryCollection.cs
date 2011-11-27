using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class SubledgerEntryCollection : TransientDomainCollection<ISubledgerEntry>, ISubledgerEntryCollection
    {
        public SubledgerEntryCollection()
            : base() { }

        public SubledgerEntryCollection(ILedgerEntry parent) : this(parent, false) { }

        public SubledgerEntryCollection(ILedgerEntry parent, bool readOnly)
            : base()
        {
            this.Parent = parent;
        }

        public void AddSubLedgerEntry(ISubledgerEntry subledgerEntry)
        {
            subledgerEntry.Parent = Parent;
            base.Add(subledgerEntry);
        }

        public bool RemoveSubLedgerEntry(ISubledgerEntry subledgerEntry)
        {
            subledgerEntry.Parent = null;
            return base.Remove(subledgerEntry);
        }

        public ILedgerEntry Parent { get; set; }
    }
}
