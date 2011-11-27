using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
namespace B4F.TotalGiro.Orders.Transfers
{
    public enum TransferType
    {
        Full = 0,
        Amount,
        Manual
    }

    public enum TransferStatus
    {
        New = 0,
        ReadyForTransfer,
        Executed,
        Cancelled
    }

    public interface IPositionTransfer
    {
        B4F.TotalGiro.Accounts.IAccountTypeInternal AccountB { get; set; }
        B4F.TotalGiro.Accounts.IAccountTypeInternal AccountA { get; set; }
        bool AIsInternal { get; set; }
        bool BIsInternal { get; set; }
        int Key { get; set; }
        DateTime TransferDate { get; set; }
        bool Executed { get; set; }
        IInternalEmployeeLogin CreatedBy { get; set; }
        DateTime CreationDate { get; }
        IPositionTransferPortfolio APortfolioBefore { get; set; }
        IPositionTransferPortfolio BPortfolioBefore { get; set; }
        IPositionTransferPortfolio APortfolioAfter { get; set; }
        IPositionTransferPortfolio BPortfolioAfter { get; set; }
        TransferType TypeOfTransfer { get; set; }
        Money TransferAmount { get; set; }
        IPositionTransferDetailCollection TransferDetails { get;  }
        string Reason { get; set; }
        string AccountNumberA { get; }
        string AccountNumberB { get; }
        TransferStatus TransferStatus { get; set; }
        string DescriptionAccountA { get; }
        string DescriptionAccountB { get; }
        //ITransactionNTMCollection Transactions { get; }
        ICurrency BaseCurrency { get; }
        bool CanBeBiDirectional { get; }
        bool IsInitialised { get; set; }
        bool PriceCanBeAltered { get; }
        bool IsEditable { get; }
    }
}
