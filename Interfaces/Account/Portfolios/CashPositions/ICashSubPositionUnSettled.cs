using System;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface ICashSubPositionUnSettled : ICashSubPosition
    {
        ICashSubPositionUnSettledType UnSettledType { get; set; }
    }
}
