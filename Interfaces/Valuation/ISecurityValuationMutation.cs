using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Valuations
{
    public interface ISecurityValuationMutation : IValuationMutation
    {
        IFundPosition Position { get; }
        Money RealisedAmount { get; }
        Money RealisedAmountToDate { get; }
        Money BaseRealisedAmount { get; }
        Money BaseRealisedAmountToDate { get; }
        Price CostPrice { get; }
        Price BookPrice { get; }
        Money BaseCommission { get; }
        Money TotalTradeAmount { get; }
        Money TotalBaseTradeAmount { get; }
        IList<ITransaction> Transactions { get; }

        void AddTx(IFundPositionTx posTx);
        void AddNotRelevantPositionTx(IFundPositionTx notRelevantPosTx);
    }
}
