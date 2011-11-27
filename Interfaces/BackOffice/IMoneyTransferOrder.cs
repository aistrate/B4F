using System;
using B4F.TotalGiro.Communicator.KasBank;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.BackOffice.Orders
{
    public enum MoneyTransferOrderStati
    {
        /// <summary>
        /// The order has just been entered
        /// </summary>
        New = 1,
        /// <summary>
        /// The order has been assembled into a file for external despatch
        /// </summary>
        FileCreated = 2,
        /// <summary>
        /// The order has been sent to the relevant Bank/Institutuion
        /// </summary>
        Placed = 3,
        /// <summary>
        /// The order is finished
        /// </summary>
        Terminated = 4,
        /// <summary>
        /// The order is not sent but cancelled
        /// </summary>
        Cancelled = 5
    }

    /// <summary>
    /// This enumeration lists the possible cancel stati an order can receive
    /// </summary>
    public enum MoneyTransferOrderCancelStati
    {
        /// <summary>
        /// The order is in neutral mode, so no cancel request has been made
        /// </summary>
        Neutral = 1,
        /// <summary>
        /// A cancel request was done
        /// </summary>
        CancelRequested = 2,
        /// <summary>
        /// The order has been cancelled
        /// </summary>
        Cancelled = 3
    }

    public enum IndicationOfCosts
    {
        Beneficiary = 0 ,
        Ours,
        Shared
    }

    public interface IMoneyTransferOrder
    {
        B4F.TotalGiro.Instruments.Money Amount { get; set; }
        string BenefBankAcctNr { get; set; }
        DateTime CreationDate { get; }
        int Key { get; set; }
        string NarDebet1 { get; set; }
        string NarDebet2 { get; set; }
        string NarDebet3 { get; set; }
        string NarDebet4 { get; set; }
        string NarBenef1 { get; set; }
        string NarBenef2 { get; set; }
        string NarBenef3 { get; set; }
        string NarBenef4 { get; set; }
        DateTime ProcessDate { get; set; }
        string Reference { get; set; }
        string SwiftAddress { get; set; }
        string TransferDescription1 { get; set; }
        string TransferDescription2 { get; set; }
        string TransferDescription3 { get; set; }
        string TransferDescription4 { get; set; }
        string DisplayDescription { get; }
        B4F.TotalGiro.Accounts.ICustomerAccount TransfereeAccount { get; set; }
        B4F.TotalGiro.Accounts.ICounterAccount TransfereeCounterAccount { get; set; }
        B4F.TotalGiro.GeneralLedger.Static.IJournal TransferorJournal { get; set; }
        string CreatedBy { get; set; }
        bool Approved { get; set; }
        string ApprovedBy { get; set; }
        DateTime ApprovalDate { get; set; }
        IGLDSTD GLDSTDRecord { get; set; }
        MoneyTransferOrderStati Status { get; }
        string DisplayStatus { get; }
        bool IsApproveable { get; }
        bool IsEditable { get; }
        bool IsSendable { get; }
        IndicationOfCosts CostIndication { get; set; }
        bool IsCancellable { get; }

        bool Approve();
        bool UnApprove();
        bool Cancel();
        Tuple<bool, string> Validate();
        void setReference();
        void SetStatus(MoneyTransferOrderStati newStatus);
    }
}
