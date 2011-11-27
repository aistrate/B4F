using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments.Prices
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.Prices.PriceDetailCollection">PriceDetailCollection</see> class
    /// </summary>
    public interface IPriceDetailCollection : IList<IPriceDetail>
	{
        IInstrumentsWithPrices Parent { get; }
        IHistoricalPrice LatestPrice { get; }
        void AddHistoricalPrice(IPriceDetail item);
        bool ContainsHistoricalPrice(IPriceDetail item);
        IPriceDetail GetItemByDate(DateTime date);
	}
}
