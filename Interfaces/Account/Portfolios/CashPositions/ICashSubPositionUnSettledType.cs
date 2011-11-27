using System;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface ICashSubPositionUnSettledType
    {
        int Key { get; set; }
        string Description { get; set; }
        bool IsDefault { get; set; }
        bool IncludeBuyingPower { get; set; }
    }
}
