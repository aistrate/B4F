using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;

namespace B4F.TotalGiro.Accounts.Portfolios
{
    public interface ISubPortfolioHistorical
    {
        IPortfolioHistorical ParentPortfolio { get; set; }
        IAccountTypeInternal ParentAccount { get; }
        DateTime PositionDate { get; }
        Money TotalPortfolioValue { get; }
        IList<IHistoricalExRate> Rates { get; }
    }
}
