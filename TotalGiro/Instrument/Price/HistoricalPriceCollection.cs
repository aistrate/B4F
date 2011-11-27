using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments.Prices
{
    /// <summary>
    /// Class representing collection of historical prices of instrument
    /// </summary>
    public class HistoricalPriceCollection : TransientDomainCollection<IPriceDetail>, IPriceDetailCollection
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.Prices.HistoricalPriceCollection">HistoricalPriceCollection</see> class.
        /// </summary>
        public HistoricalPriceCollection()
            : base() { }

        public IInstrumentsWithPrices Parent { get; internal set; }

        /// <summary>
        /// Get latest price
        /// </summary>
        public IHistoricalPrice LatestPrice
		{
			get { return (IHistoricalPrice)this.OrderByDescending(u => u.Date).FirstOrDefault(); }
		}

		#region Methods

        /// <summary>
        /// Add item to collection of historical prices
        /// </summary>
        /// <param name="item">HistoricalPrice object</param>
        public void AddHistoricalPrice(IPriceDetail item)
		{
			Add(item);
			Parent.CurrentPrice = LatestPrice;
		}

        /// <summary>
        /// Does the historical price exist. Matches on date.
        /// </summary>
        /// <param name="item">HistoricalPrice</param>
        /// <returns>Flag</returns>
        public bool ContainsHistoricalPrice(IPriceDetail item)
        {
            return this.Where(u => u.Date == item.Date).Count() > 0;
        }

        public IPriceDetail GetItemByDate(DateTime date)
        {
            return this.Where(u => u.Date == date).FirstOrDefault();
        }

		#endregion
	}
}
