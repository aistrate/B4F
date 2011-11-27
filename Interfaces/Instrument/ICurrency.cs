using System;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Instruments.Classification;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.Currency">Currency</see> class
    /// </summary>
    public interface ICurrency : IInstrument
	{

        IExRateCollection HistoricalExRates { get; }
		bool EuroMember { get; set; }
        decimal LegacyExchangeRate { get; set; }
		//decimal GetExRate(ICurrency toCurrency, B4F.TotalGiro.Orders.Side side);
		ICountry CountryOfOrigin { get; }
		ICurrency SettlementCurrency { get; }
		IExRate ExchangeRate { get; set; }
		IExRate GetHistoricalExRate(IDalSession session, DateTime date);
		decimal GetExRate(B4F.TotalGiro.Orders.Side side);
		decimal GetExRate(ICurrency toCurrency, B4F.TotalGiro.Orders.Side side);
		string AltSymbol { get; }
		string Symbol { get; }
        IAssetClass AssetClass { get; set; }
		ICurrency BaseCurrency { get; }
		bool IsBase { get; }
        bool IsObsoleteCurrency { get; }

        void SetObsolete(DateTime changeDate, ICurrency succeederCurrency, decimal legacyExchangeRate);
	}
}
