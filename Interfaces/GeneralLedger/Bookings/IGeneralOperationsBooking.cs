using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public enum GeneralOperationsBookingTypes
    {
        ManagementFee = 1,
        CashDividend = 2,
        CashTransfer = 4,
        ForeignExchange = 8,
        BondCouponPayment = 16
    }

    public interface IGeneralOperationsBooking : ICashPresentation
    {
        int Key { get; set; }
        IJournalEntry GeneralOpsJournalEntry { get; set; }
        bool IsStorno { get; set; }
        string StornoReason { get; set; }
        bool IsStornoable { get; }
        DateTime CreationDate { get; set; }
        decimal TaxPercentage { get; set; }
        GeneralOperationsBookingTypes BookingType { get; }
        IAccountTypeInternal Account { get; set; }
        IGeneralOperationsBooking OriginalBooking { get; set; }
        IGeneralOperationsBooking Storno(IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry);
        IGeneralOperationsBooking StornoBooking { get; set; }
        IGeneralOperationsComponentCollection Components { get; }
        IGLBookingType GLBookingType { get; }
        //int MigratedKey { get; set; }
        bool IsNotarizable { get; }
        INota BookNota { get; set; }
        bool NotaMigrated { get; }
        IOrderAmountBased CashInitiatedOrder { get; set; }
        string Description { get; set; }

        INota CreateNota();
        bool NeedToCreateCashInitiatedOrder(out ITradeableInstrument instrument);
    }
}
