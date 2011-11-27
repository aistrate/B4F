using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Stichting
{
    public interface IAssetManagerInstrumentCollection : IList<IAssetManagerInstrument>
    {
        void AddInstrument(ITradeableInstrument instrument);
        IAssetManagerInstrument GetItemByInstrument(ITradeableInstrument instrument);
    }
}
