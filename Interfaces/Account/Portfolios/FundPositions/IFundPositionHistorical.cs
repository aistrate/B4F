using System;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public interface IFundPositionHistorical : ICommonPositionHistorical
    {
        IFundPortfolioHistorical Parent { get; set; }
        InstrumentSize Size { get; set; }
        Price Price { get; set; }

    }
}
