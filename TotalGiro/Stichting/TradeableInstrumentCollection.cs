using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// This class holds collection of instruments that a AssetManager is interested in.
    /// </summary>
    internal class TradeableInstrumentCollection : TransientDomainCollection<ITradeableInstrument>
    {
        public TradeableInstrumentCollection()
            : base() { }

        public TradeableInstrumentCollection(IAssetManager parent)
            : base()
        {
            Parent = parent;
        }

        /// <summary>
        /// Get/set associated Asset Manager
        /// </summary>
        public IAssetManager Parent { get; set; }

        //public ReadOnlyCollection<ITradeableInstrument> ToReadOnlyCollection()
        //{
        //    return new ReadOnlyCollection<ITradeableInstrument>(this);
        //}
    }
}