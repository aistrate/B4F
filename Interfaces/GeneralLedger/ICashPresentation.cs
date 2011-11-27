using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public enum CashPresentationTypes
    {
        CashTransfer = 0,
        ManagementFee,
        CashDividend,
        Transaction,
    }
    
    public interface ICashPresentation
    {
        string CashPresentationKey { get; }
        CashPresentationTypes CashPresentationType { get; }
        string TypeID { get; }
        string TypeDescription { get; }
        IAccountTypeInternal Account { get; }
        string Description { get; }
        Money TotalAmount { get; }
        DateTime BookDate { get; }
        DateTime CreationDate { get; }

        int GetHashCode();
    }
}
