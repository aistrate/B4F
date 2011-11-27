using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface ICashSubPositionUnSettledCollection : IList<ICashSubPositionUnSettled>
    {
        ICashPosition ParentPosition { get; set; }
        ICashSubPositionUnSettled DefaultSubPosition { get; }
        ICashSubPositionUnSettled GetSubPosition(ICashSubPositionUnSettledType unSettledType);
        void AddSubPosition(ICashSubPositionUnSettled item);
    }
}
