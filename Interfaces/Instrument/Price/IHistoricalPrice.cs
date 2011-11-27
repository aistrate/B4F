using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments.Prices
{
    public interface IHistoricalPrice : IPriceDetail
    {
        int Key { get; set; }
        Price OpenPrice { get; set; }
        Price ClosedPrice { get; set; }
        Price HighPrice { get; set; }
        Price LowPrice { get; set; }
        IInstrumentsWithPrices Instrument { get; }
        DateTime CreationDate { get; }
    }
}
