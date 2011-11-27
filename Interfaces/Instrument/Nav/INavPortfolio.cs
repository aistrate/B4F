using System;
using B4F.TotalGiro.Collections.Persistence;
using System.Collections.Generic;
namespace B4F.TotalGiro.Instruments.Nav
{
    public interface INavPortfolio : IList<INavPosition>
    {
        void AddNavPosition(INavPosition entry);
        INavCalculation Parent { get; }
        Money PortfolioValue();
    }
}
