using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments.ExRates
{
    /// <summary>
    /// Class hold collection of historical exchange rates
    /// </summary>
    public class HistoricalExRateCollection : TransientDomainCollection<IExRate>, IExRateCollection
	{
        /// <summary>
        /// Sets a collection  
        /// </summary>
        public HistoricalExRateCollection()
            : base() { }

        public ICurrency Parent { get; internal set; }

        /// <summary>
        /// Get the latest rate in time
        /// </summary>
		public IExRate LatestRate
		{
            get { return this.OrderByDescending(u => u.RateDate).FirstOrDefault(); }
		}


        /// <summary>
        /// Add historical exchange rate to collection
        /// </summary>
        /// <param name="item"></param>
        public void AddExRate(IExRate item)
		{
			base.Add(item);
			item.Currency.ExchangeRate = LatestRate;
        }

        /// <summary>
        /// Does the historical exchange rate exist. Matches on date.
        /// </summary>
        /// <param name="item">HistoricalExRate</param>
        /// <returns>Flag</returns>
        public bool ContainsExRate(IExRate item)
        {
            return this.Where(u => u.RateDate == item.RateDate).Count() > 0;
        }

        public IExRate GetItemByDate(DateTime date)
        {
            return this.Where(u => u.RateDate == date).FirstOrDefault();
        }

    }
}
