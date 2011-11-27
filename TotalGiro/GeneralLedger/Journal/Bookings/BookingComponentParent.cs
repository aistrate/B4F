using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public abstract class BookingComponentParent : IBookingComponentParent
    {
        protected BookingComponentParent() { }
        public BookingComponentParent(BookingComponentTypes bookingComponentType, DateTime creationDate)
        {
            this.Component = new BookingComponent(bookingComponentType, this, creationDate);
        }

        public virtual int Key { get; set; }
        public abstract IJournalEntry BookingJournalEntry { get; }
        public virtual IBookingComponent Component { get; set; }
        public virtual IGeneralOperationsBooking Parent { get; protected set; }
        public abstract BookingComponentParentTypes BookingComponentParentType { get; }
        protected int bookingComponentParentTypeId;
        public virtual BookingComponentTypes BookingComponentType 
        { 
            get 
            {
                // TODO
                if (this.Component != null)
                    return this.Component.BookingComponentType;
                else
                    return BookingComponentTypes.Value;
            } 
        }

        public virtual Money ComponentValue { get { return this.Component.ComponentValue; } }
        public virtual IBookingLineCollection JournalLines { get { return this.Component.JournalLines; } }

        public virtual IJournalEntryLine MainLine
        {
            get { return this.Component.MainLine; }
            set { this.Component.MainLine = value; }
        }

        public void AddLinesToComponent(Money componentValue, BookingComponentTypes bookingComponentType, bool isSettled, bool isExternalExecution, bool isInternalExecution, IGLLookupRecords lookups, IAccount accountA)
        {
            AddLinesToComponent(componentValue, bookingComponentType, isSettled, isExternalExecution, isInternalExecution, lookups, accountA, null, null);
        }

        public void AddLinesToComponent(Money componentValue, BookingComponentTypes bookingComponentType, bool isSettled, bool isExternalExecution, bool isInternalExecution, IGLLookupRecords lookups, IAccount accountA, IAccount accountB)
        {
            AddLinesToComponent(componentValue, bookingComponentType, isSettled, isExternalExecution, isInternalExecution, lookups, accountA, null, null);
        }

        public void AddLinesToComponent(Money componentValue, BookingComponentTypes bookingComponentType, bool isSettled, bool isExternalExecution, bool isInternalExecution, IGLLookupRecords lookups, IAccount accountA, IAccount accountB, DateTime? valueDate)
        {

            IGLLookupRecord lookupRecord = GetLookupRecord(componentValue, bookingComponentType, isSettled, isExternalExecution, isInternalExecution, lookups);
            this.Component.AddLinesToComponent(componentValue, lookupRecord, accountA, accountB, valueDate);
        }

        private IGLLookupRecord GetLookupRecord(Money componentValue, BookingComponentTypes bookingComponentType, bool isSettled, bool isExternalExecution, bool isInternalExecution, IGLLookupRecords lookups)
        {
            ICurrency currency = componentValue.Underlying.ToCurrency;
            return lookups.GetGLLookupRecord(currency, isExternalExecution, isInternalExecution, !isSettled, bookingComponentType);
        }


        public virtual DateTime CreationDate
        {
            get
            {
                if (creationDate.HasValue)
                    return creationDate.Value;
                else
                    return DateTime.MinValue;
            }
            set
            {
                creationDate = value;
            }
        }

        private DateTime? creationDate = DateTime.Now;
    }
}
