using System;

namespace B4F.TotalGiro.Instruments.Classification
{
    public interface IAssetClass
    {
        int Key { get; set; }
        string AssetName { get; }
    }
}
