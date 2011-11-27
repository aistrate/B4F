using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public interface IFeeCalc
    {
        int Key { get; set; }
        string Name { get; set; }
        IAssetManager AssetManager { get; set; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }
        ICurrency FeeCurrency { get; }
        FeeType FeeType { get; }
        bool IsActive { get; set; }
        IFeeCalcVersionCollection Versions { get; }
        IFeeCalcVersion LatestVersion { get; }
    }
}
