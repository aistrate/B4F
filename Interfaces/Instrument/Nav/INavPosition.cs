using System;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Instruments.ExRates;
namespace B4F.TotalGiro.Instruments.Nav
{
    public interface INavPosition
    {
        int Key { get; set; }
        INavCalculation Parent { get; set; }
        Price ClosingPriceUsed { get; set; }
        decimal ExchangeRateUsed { get; set; }
        Money CurrentBaseValue { get; }
        Money CurrentValue { get; }

        IPriceDetail ClosingPriceRecord { get; set; }
        IExRate ExchangeRateRecord { get; set; }        

        B4F.TotalGiro.Accounts.Portfolios.IsLong Sign { get; }
        InstrumentSize Size { get; set; }
        bool IsSecurityPosition { get; }
        bool IsCashPosition { get; }
        void setCurrentValue();
        void setCurrentBaseValue();
    }
}
