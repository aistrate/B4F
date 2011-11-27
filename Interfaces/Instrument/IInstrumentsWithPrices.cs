using System;
using System.Collections;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments.Classification;


namespace B4F.TotalGiro.Instruments
{
    public interface IInstrumentsWithPrices : IInstrument
    {
        string Isin { get; set; }
        ICurrency CurrencyNominal { get; set; }
        PricingTypes PriceType { get; set; }
        IPriceDetailCollection HistoricalPrices { get; }
    }
}
