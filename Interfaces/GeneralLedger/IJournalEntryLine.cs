using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Communicator.Exact;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    [Flags]
    public enum JournalEntryLineStati
    {
        None = 0,
        New = 1,
        Booked = 4
    }

    public enum CashTransferDetailTypes
    {
        None = 0,
        WithdrawalOneOff = 1,
        WithdrawalPeriodic = 2,
        WithdrawalTermination = 3,
        Deposit = 4
    }

    public interface IJournalEntryLine
    {
        bool DoNotExport { get; set; }
        bool GLAccountIsFixed { get; }
        bool GroupingbyTransaction { get; }
        bool IsAdminBooking { get; }
        bool IsCashTransfer { get; }
        bool IsForeignExchange { get; }
        bool IsDeletable { get; }
        bool IsEditable { get; }
        bool IsRelevant { get; }
        bool IsSettledStatus { get; set; }
        bool IsStorno { get; }
        bool IsStornoable { get; }
        bool SkipOrders { get; set; }
        bool IsAllowedToAddTransferFee { get; }
        DateTime CreationDate { get; }
        DateTime BookDate { get; }
        DateTime ValueDate { get; set; }
        DateTime LastUpdated { get; }
        decimal ExchangeRate { get; }
        IAccountTypeInternal GiroAccount { get; set; }
        IBookingComponent BookComponent { get; set; }
        ICashSubPosition ParentSubPosition { get; set; }
        ICurrency Currency { get; }
        IExternalSettlement MatchedSettlement { get; set; }
        IGLAccount GLAccount { get; set; }
        IImportedBankMovement ImportedBankMovement { get; set; }
        IInstruction Instruction { get; set; }
        IInstrument BookingRelatedInstrument { get; }
        IJournalEntry Parent { get; set; }
        IJournalEntryLine Clone();
        IJournalEntryLine Clone(int lineNumber);
        IJournalEntryLine CreateStorno();
        IJournalEntryLine CreateStorno(int lineNumber);
        IJournalEntryLine Storno { get; }
        IJournalEntryLine StornoedLine { get; set; }
        int GroupingKey { get; }
        int Key { get; set; }
        int LineNumber { get; set; }
        IOrderAmountBased CashInitiatedOrder { get; set; }
        ISubledgerEntry SubledgerEntry { get; set; }
        JournalEntryLineStati Status { get; }
        Money Balance { get; set; }
        Money BaseBalance { get; }
        Money Credit { get; set; }
        Money Debit { get; set; }
        string CreatedBy { get; set; }
        string BookedBy { get; set; }
        string Description { get; set; }
        string DisplayStatus { get; }
        string OriginalDescription { get; set; }
        string TegenrekeningNumber { get; set; }
        CashTransferDetailTypes CashTransferDetailType { get; set; }
        void BookLine();
        void ClientSettle(ITradingJournalEntry clientSettleJournal);


    }
}

