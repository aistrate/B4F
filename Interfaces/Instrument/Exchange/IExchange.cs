using System;
using System.Collections;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.Exchange">Exchange</see> class
    /// </summary>
    public interface IExchange
	{
        int Key { get; set; }
        ICountry DefaultCountry { get; }
		ICurrency DefaultCurrency { get; }
		short DefaultSettlementPeriod { get; }
		string ExchangeName { get; }
        byte DefaultNumberOfDecimals { get; }
        IExchangeHolidayCollection ExchangeHolidays { get; }
	}
}
