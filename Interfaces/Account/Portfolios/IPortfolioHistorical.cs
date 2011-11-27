using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Instruments.ExRates;
namespace B4F.TotalGiro.Accounts.Portfolios
{
    public interface IPortfolioHistorical
    {
        IAccountTypeInternal ParentAccount { get; set; }
        DateTime PositionDate { get; set; }
        Money TotalPortfolioValue { get; }
        IFundPortfolioHistorical FundPortfolio { get; set; }
        ICashPortfolioHistorical CashPortfolio { get; set; }
        IList<IHistoricalPrice> Prices { get; set; }
        IList<IHistoricalExRate> Rates { get; set; }
    }
}
