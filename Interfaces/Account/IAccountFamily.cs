using System;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Accounts.ManagementPeriods;

namespace B4F.TotalGiro.Accounts
{
    public enum ManagementFeeInstalments
    {
        Quarterly = 4,
        Monthly = 12
    }
    
    public interface IAccountFamily
    {
        int Key { get; set; }
        IAssetManager AssetManager { get; set; }
        string AccountPrefix { get; set; }
        int AccountSeed { get; set; }
        DateTime CreationDate { get; set; }
        ManagementFeeInstalments ManagementFeeInstalment { get; set; }

        bool IsManagementTypeCharged(ManagementTypes managementType);
    }
}
