using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class BookingLineCollection : TransientDomainCollection<IJournalEntryLine>, IBookingLineCollection
    {
        public BookingLineCollection()
            : base() { }

        public BookingLineCollection(IBookingComponent parent)
            : base()
        {
            Parent = parent;
        }

        public void AddJournalEntryLine(IJournalEntryLine journalEntryLine)
        {
            journalEntryLine.BookComponent = Parent;
            Parent.Parent.BookingJournalEntry.Lines.AddJournalEntryLine(journalEntryLine);
            base.Add(journalEntryLine);
        }

        public IBookingComponent Parent { get; set; }
    }
}
