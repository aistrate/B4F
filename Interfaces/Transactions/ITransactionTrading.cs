using System;
namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ITransactionTrading : ITransaction
    {
        void ClientSettle(B4F.TotalGiro.GeneralLedger.Journal.ITradingJournalEntry clientSettleJournal);
        DateTime ClientSettlementDate { get; set; }
        bool IsClientSettled { get; set; }
    }
}
