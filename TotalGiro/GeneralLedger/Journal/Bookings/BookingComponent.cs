using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class BookingComponent : IBookingComponent
    {
        public BookingComponent()
        {
            journalLines = new BookingLineCollection(this);
        }

        public BookingComponent(BookingComponentTypes bookingComponentType, IBookingComponentParent parent)
            : this(bookingComponentType, parent, DateTime.Now)
        {
        }

        public BookingComponent(BookingComponentTypes bookingComponentType, IBookingComponentParent parent, DateTime creationDate)
            : this()
        {
            this.BookingComponentType = bookingComponentType;
            this.CreationDate = creationDate;
            this.Parent = parent;
  
        }

        public int Key { get; set; }
        public BookingComponentTypes BookingComponentType { get; set; }
        public IBookingComponentParent Parent { get; set; }
        public DateTime CreationDate
        {
            get
            {
                return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue;
            }
            set
            {
                this.creationDate = value;
            }
        }
        public IJournalEntryLine MainLine { get; set; }

        public Money ComponentValue
        {
            get
            {
                Money returnValue = null;
                if (MainLine != null)
                    returnValue = MainLine.Balance.Negate();
                else
                {
                    ICurrency currency = null;
                    if (JournalLines.FirstOrDefault() != null)
                        currency = JournalLines.FirstOrDefault().Currency;
                    else
                        currency = Parent.BookingJournalEntry.Balance.Underlying as ICurrency;
                    returnValue = new Money(0M, currency);
                }
                return returnValue;
            }
        }

        public void AddLinesToComponent(Money componentValue, IGLLookupRecord lookupRecord,  IAccount accountA, IAccount accountB, DateTime? valueDate)
        {
            IGLAccount glAcctA = lookupRecord.MainAccount;
            IGLAccount glAcctB = lookupRecord.CounterAccount;

            IJournalEntryLine sideA = new JournalEntryLine();
            sideA.GLAccount = glAcctA;
            sideA.Balance = componentValue.Negate();
            if (sideA.GLAccount.RequiresGiroAccount) sideA.GiroAccount = (IAccountTypeInternal)accountA;
            if (valueDate.HasValue) sideA.ValueDate = valueDate.Value;
            this.JournalLines.AddJournalEntryLine(sideA);
            this.MainLine = sideA;

            IJournalEntryLine sideB = new JournalEntryLine();
            sideB.GLAccount = glAcctB;
            sideB.Balance = componentValue;
            if (sideB.GLAccount.RequiresGiroAccount) sideB.GiroAccount = (IAccountTypeInternal)accountB;
            if (valueDate.HasValue) sideB.ValueDate = valueDate.Value;
            this.JournalLines.AddJournalEntryLine(sideB);
        }

        public void SetDescription(string description)
        {
            foreach (IJournalEntryLine line in this.JournalLines)
            {
                line.Description = description;
            }
        }

        public virtual IBookingLineCollection JournalLines
        {
            get
            {
                IBookingLineCollection lines = (IBookingLineCollection)journalLines.AsList();
                if (lines.Parent == null) lines.Parent = this;
                return lines;
            }
        }


        #region Privates

        private DateTime? creationDate;
        private IDomainCollection<IJournalEntryLine> journalLines;

        #endregion

    }
}
