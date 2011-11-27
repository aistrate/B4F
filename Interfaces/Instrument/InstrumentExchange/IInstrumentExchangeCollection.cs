using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.InstrumentExchangeCollection">InstrumentExchangeCollection</see> class
    /// </summary>
    public interface IInstrumentExchangeCollection : IList<IInstrumentExchange>
    {
        IInstrumentExchange GetItemByExchange(int exchangeID);
        IInstrumentExchange GetDefault();
    }
}
