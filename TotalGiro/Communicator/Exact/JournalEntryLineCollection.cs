using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Collections;
using System.Collections;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class JournalEntryLineCollection : TransientDomainCollection<IJournalEntryLine>, IJournalEntryLineCollection
    {
        public JournalEntryLineCollection(  )   { }

        public JournalEntryLineCollection(ISubledgerEntry parent )
            : base()
        {
            Parent = parent;
        }


        public void AddLine(IJournalEntryLine journalEntryLine)
        {
            journalEntryLine.SubledgerEntry = Parent;
            base.Add(journalEntryLine);
        }

        public bool RemoveLine(IJournalEntryLine journalEntryLine)
        {
            journalEntryLine.SubledgerEntry = null;
            return base.Remove(journalEntryLine);
        }

        public ISubledgerEntry Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        private ISubledgerEntry parent;
    }
}
