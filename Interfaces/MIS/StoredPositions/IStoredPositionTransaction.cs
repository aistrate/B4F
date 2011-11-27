using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.MIS.StoredPositions
{
    public interface IStoredPositionTransaction
    {
        IAccountTypeInternal Account { get; set; }
        IFundPosition FundPosition { get; set; }
        IFundPositionTx FundPositionTransaction { get; set; }
        int Key { get; set; }
        ITransaction OriginalTransaction { get; set; }
        InstrumentSize Size { get; set; }
        DateTime TransactionDate { get; set; }
        DateTime CreationDate { get; set; }
    }
}
