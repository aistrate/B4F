using System;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Instruments
{
    public interface ILifecycle
    {
        int Key { get; set; }
        string Name { get; set; }
        IAssetManager AssetManager { get; set; }
        bool IsActive { get; set; }
        string CreatedBy { get; set; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; set; }
        ILifecycleLineCollection Lines { get; }
        IPortfolioModel GetRelevantModel(DateTime birthdate);
        IPortfolioModel GetRelevantModel(int currentAge);
    }
}
