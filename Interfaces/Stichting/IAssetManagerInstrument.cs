using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;

namespace B4F.TotalGiro.Stichting
{
    public interface IAssetManagerInstrument
    {
        int Key { get; set; }
        IAssetManager AssetManager { get;  }
        ITradeableInstrument Instrument { get; }
        IAssetClass AssetClass { get; set; }
        IRegionClass RegionClass { get; set; }
        IInstrumentsCategories InstrumentsCategories { get; set; }
        ISectorClass SectorClass { get; set; }
        short MaxWithdrawalAmountPercentage { get; set; }
        bool IsActive { get; set; }
    }
}
