using System;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface ICashPositionHistorical : ICommonPositionHistorical
    {

        ICashPortfolioHistorical Parent { get; set; }


    }
}
