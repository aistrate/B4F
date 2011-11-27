using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class MemorialBooking : JournalEntry, IMemorialBooking
    {
        public MemorialBooking() { }

        public MemorialBooking(IJournal journal, string journalEntryNumber)
            : base(journal, journalEntryNumber)
        {
        }

        public override JournalEntryTypes JournalEntryType
        {
            get { return JournalEntryTypes.MemorialBooking; }
        }

        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string description;
    }
}
