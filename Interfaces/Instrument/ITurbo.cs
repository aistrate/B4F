using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios;

namespace B4F.TotalGiro.Instruments
{
    public interface ITurbo : IDerivative
    {
        decimal FinanceLevel { get; set; }
        decimal Leverage { get; set; }
        short Ratio { get; set; }
        IsLong Sign { get; set; }
        Price StopLoss { get; set; }
        string DisplayRatio { get; }
    }
}
