using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments.ExRates
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.ExRates.HistoricalExRateCollection">HistoricalExRateCollection</see> class
    /// </summary>
    public interface IExRateCollection : IList<IExRate>
    {
        ICurrency Parent { get; }
        IExRate LatestRate { get; }
        void AddExRate(IExRate item);
        bool ContainsExRate(IExRate item);
        IExRate GetItemByDate(DateTime date);
    }
}
