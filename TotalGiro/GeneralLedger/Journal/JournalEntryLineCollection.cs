using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections;
using System.Collections;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class JournalEntryLineCollection : TransientDomainCollection<IJournalEntryLine>, IJournalEntryLineCollection
    {
        public JournalEntryLineCollection()
            : base() { }

        public JournalEntryLineCollection(IJournalEntry parent)
            : base()
        {
            Parent = parent;
        }

        //public JournalEntryLineCollection(IJournalEntry parent, IList bagCollection) : this(parent, bagCollection, false) { }

        //public JournalEntryLineCollection(IJournalEntry parent, IList bagCollection, bool readOnly)
        //    : base(bagCollection, readOnly)
        //{
        //    this.parent = parent;
        //}

        public int GetNextLineNumber()
        {
            return (Count > 0 ? (this[Count - 1].LineNumber + 1) : 1);
        }

        public void AddJournalEntryLine(IJournalEntryLine journalEntryLine)
        {
            journalEntryLine.LineNumber = GetNextLineNumber();
            journalEntryLine.Parent = Parent;
            if (Parent.Status == JournalEntryStati.Booked)
                Parent.Status = JournalEntryStati.Open;
            base.Add(journalEntryLine);
        }

        public bool Remove(IJournalEntryLine line)
        {
            if (Parent.Status == JournalEntryStati.Open)
            {
                bool otherNewLines = false;
                foreach (IJournalEntryLine l in this)
                    if (l.Status == JournalEntryLineStati.New && l.Key != line.Key)
                    {
                        otherNewLines = true;
                        break;
                    }
                if (!otherNewLines)
                    Parent.Status = JournalEntryStati.Booked;
            }

            line.Parent = null;
            if (line.BookComponent != null && line.BookComponent.Parent != null)
            {
                switch (line.BookComponent.Parent.BookingComponentParentType)
                {
                    case BookingComponentParentTypes.CashTransfer:
                        ICashTransferComponent cashComp = (ICashTransferComponent)line.BookComponent.Parent;
                        if (cashComp.JournalLines != null && cashComp.JournalLines.Count > 1)
                        {
                            cashComp.JournalLines.Remove(line);
                            line.BookComponent = null;
                            if (cashComp.MainLine != null && cashComp.MainLine.Equals(line))
                                cashComp.MainLine = cashComp.JournalLines.Where(x => x.GLAccount != null).FirstOrDefault();
                        }
                        else
                        {
                            ICashTransfer mutation = cashComp.ParentBooking as ICashTransfer;
                            if (mutation != null)
                            {
                                mutation.Components.Remove(cashComp);
                                cashComp.ParentBooking = null;
                            }
                        }
                        break;
                }
            }
            return base.Remove(line);
        }

        public IJournalEntryLine GetLineById(int key)
        {
            foreach (IJournalEntryLine line in this)
                if (line.Key == key)
                    return line;

            return null;
        }

        public void ShiftLineNumbers(int startFromLine, int shiftBy)
        {
            foreach (IJournalEntryLine line in this)
                if (line.LineNumber >= startFromLine)
                    line.LineNumber += shiftBy;
        }

        public IJournalEntryLine FixedAccountLine
        {
            get
            {
                foreach (IJournalEntryLine line in this)
                    if (line.GLAccountIsFixed)
                        return line;
                return null;
            }
        }

        public IJournalEntry Parent { get; set; }
    }
}
