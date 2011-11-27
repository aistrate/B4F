using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public interface IFundPortfolioHistorical : ISubPortfolioHistorical, IList<IFundPositionHistorical>
    {
        IList<IHistoricalPrice> Prices { get; }
        Money CultureFundValue { get; }
        Money GreenFundValue { get; }
    }
}
