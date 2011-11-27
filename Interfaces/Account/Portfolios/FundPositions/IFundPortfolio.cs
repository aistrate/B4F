using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public interface IFundPortfolio : IList<IFundPosition>
    {
        IAccountTypeInternal ParentAccount { get; set; }
        IFundPositionTx CreatePositionTx(ITransaction transaction, TransactionSide txSide, PositionsTxValueTypes isCV);
        DateTime FirstTxDate { get; }
        Money TotalValueInBaseCurrency { get; }
        Money CashFundValueInBaseCurrency { get; }
        IFundPosition GetPosition(IInstrumentsWithPrices positionInstrument);
        IFundPortfolio NewCollection(Func<IFundPosition, bool> criteria);
        IFundPortfolio Exclude(IList<ITradeableInstrument> excludedInstruments);
        IFundPortfolio ExcludeNonTradeableInstruments();
    }
}
