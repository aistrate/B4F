using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public enum BookingComponentTypes
    {
        /// <summary>
        /// Value
        /// </summary>
        Value = 0,
        /// <summary>
        /// Counter Value
        /// </summary>
        CValue = 1,
        /// <summary>
        /// Commission
        /// </summary>
        Commission = 2,
        /// <summary>
        /// Service Charge
        /// </summary>
        ServiceCharge = 3,
        /// <summary>
        /// Tax
        /// </summary>
        Tax = 4,
        /// <summary>
        /// Transfer Fee
        /// </summary>
        CashTransferFee = 5,
        /// <summary>
        /// Converted Size from a corporate action
        /// </summary>
        Conversion = 6,
        /// <summary>
        /// Dividend
        /// </summary>
        Dividend = 7,
        /// <summary>
        /// Dividend Tax
        /// </summary>
        DividendTax = 8,
        /// <summary>
        /// ManagementFee
        /// </summary>
        ManagementFee = 9,
        /// <summary>
        /// ManagementFeeFixedCosts
        /// </summary>
        ManagementFeeFixedCosts = 10,
        /// <summary>
        /// AdministrationCosts
        /// </summary>
        AdministrationCosts = 11,
        /// <summary>
        /// Cash Deposit/withdrawal
        /// </summary>
        CashTransfer = 12,
        /// <summary>
        /// Cash Deposit/withdrawal
        /// </summary>
        ForexBaseCurrency = 13,
        /// <summary>
        /// Cash Deposit/withdrawal
        /// </summary>
        ForexNonBaseCurrency = 14,
        /// <summary>
        /// Accrued interest
        /// </summary>
        AccruedInterest = 15
    }

    public interface IBookingComponent
    {
        int Key { get; set; }
        IBookingComponentParent Parent { get; set; }
        Money ComponentValue { get;  }
        BookingComponentTypes BookingComponentType { get; set; }
        DateTime CreationDate { get; }
        IBookingLineCollection JournalLines { get; }
        IJournalEntryLine MainLine { get; set; }
        void AddLinesToComponent(Money componentValue, IGLLookupRecord lookupRecord, IAccount accountA, IAccount accountB, DateTime? valueDate);
        void SetDescription(string description);
    }
}
