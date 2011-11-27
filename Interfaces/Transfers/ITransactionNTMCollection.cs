using System;
namespace B4F.TotalGiro.Orders.Transfers
{
    public interface ITransactionNTMCollection
    {
        void AddTransactionNTM(B4F.TotalGiro.Orders.Transactions.ITransactionNTM transaction);
        IPositionTransferDetail TransferDetail { get; set; }
    }
}
