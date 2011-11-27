using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public enum BookingComponentParentTypes
    {
        Transaction = 0,
        CashDividend = 1,
        ManagementFee = 2,
        CashTransfer = 4,
        ForexTransaction = 8,
        AccruedInterest = 16
    }

    public interface IBookingComponentParent
    {
        int Key { get; set; }
        IJournalEntry BookingJournalEntry { get; }
        IBookingLineCollection JournalLines { get; }
        BookingComponentTypes BookingComponentType { get; }
        BookingComponentParentTypes BookingComponentParentType { get; }
        IBookingComponent Component { get; }
        Money ComponentValue { get; }
        IJournalEntryLine MainLine { get; set; }
        DateTime CreationDate { get; set; }
        void AddLinesToComponent(Money componentValue, BookingComponentTypes bookingComponentType, bool isSettled, bool isExternalExecution, bool isInternalExecution, IGLLookupRecords lookups, IAccount accountA);
        void AddLinesToComponent(Money componentValue, BookingComponentTypes bookingComponentType, bool isSettled, bool isExternalExecution, bool isInternalExecution, IGLLookupRecords lookups, IAccount accountA, IAccount accountB);
        void AddLinesToComponent(Money componentValue, BookingComponentTypes bookingComponentType, bool isSettled, bool isExternalExecution, bool isInternalExecution, IGLLookupRecords lookups, IAccount accountA, IAccount accountB, DateTime? valueDate);
    }
}
