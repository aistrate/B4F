using System;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Valuations;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public enum CashPositionSettleStatus
    {
        UnSettled = 0,
        Settled
    }

    public interface ICashPosition : IAuditable, ICommonPosition
    {
        int Key { get; set; }
        IAccountTypeInternal Account { get; set; } 
        ICurrency PositionCurrency { get; set; }
        Money SettledSize { get;  }
        Money SettledSizeInBaseCurrency { get; }
        Money UnSettledSize { get; }
        Money UnSettledSizeInBaseCurrency { get; }
        DateTime CreationDate { get; set; }
        DateTime LastUpdated { get; }
        DateTime OpenDate { get; set;  }
        ICashSubPosition SettledPosition { get; set; }
        ICashSubPosition UnSettledPosition { get; }
        ICashSubPositionUnSettledCollection UnSettledPositions { get; }
        IExRate ExchangeRate { get; }
        IAssetClass AssetClass { get; }
        IValuationMutation LastMutation { get; set;  }
        IValuation LastValuation { get; }

        ICashSubPosition GetSubPosition(IGLAccount glAccount);
    }
}
