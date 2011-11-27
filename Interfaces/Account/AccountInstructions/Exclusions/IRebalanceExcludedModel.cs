using System;

namespace B4F.TotalGiro.Accounts.Instructions.Exclusions
{
    public interface IRebalanceExcludedModel : IRebalanceExclusion
    {
        B4F.TotalGiro.Instruments.IPortfolioModel Model { get; set; }
    }
}
