using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Instruments.ExRates
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.ExRates.HistoricalExRate">HistoricalExRate</see> class
    /// </summary>
    public interface IHistoricalExRate : IExRate
    {
        int Key { get; set;  }
        DateTime CreationDate { get; }
    }
}
