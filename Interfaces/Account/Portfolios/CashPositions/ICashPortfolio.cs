using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface ICashPortfolio : IList<ICashPosition>
    {
        IAccountTypeInternal ParentAccount { get; set; }
        Money SettledCashTotalInBaseValue { get; }
        Money UnSettledCashTotalInBaseValue { get; }
        DateTime FirstCashTxDate { get; }
        ICashSubPosition GetSubPosition(ICurrency positionCurrency, IGLAccount glAccount);
        ICashPosition GetPosition(ICurrency positionCurrency);
        ICashSubPosition GetSettledBaseSubPosition();
    }
}
