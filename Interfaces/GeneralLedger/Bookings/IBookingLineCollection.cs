using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IBookingLineCollection : IList<IJournalEntryLine>

    {
        void AddJournalEntryLine(IJournalEntryLine journalEntry);
        IBookingComponent Parent { get; set; }
    }
}
