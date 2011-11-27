using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;

namespace B4F.TotalGiro.Stichting
{
    public class AssetManagerInstrument : IAssetManagerInstrument
    {
        protected AssetManagerInstrument() 
        {
            MaxWithdrawalAmountPercentage = 100;
            IsActive = true;
        }
        
        internal AssetManagerInstrument(IAssetManager assetManager, ITradeableInstrument instrument)
            :this()
        {
            this.AssetManager = assetManager;
            this.Instrument = instrument;
        }
        
        public virtual int Key { get; set; }
        public virtual IAssetManager AssetManager { get; set; }
        public virtual ITradeableInstrument Instrument { get; set; }
        public virtual IAssetClass AssetClass { get; set; }
        public virtual IRegionClass RegionClass { get; set; }
        public virtual IInstrumentsCategories InstrumentsCategories { get; set; }
        public virtual ISectorClass SectorClass { get; set; }
        public virtual short MaxWithdrawalAmountPercentage { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
