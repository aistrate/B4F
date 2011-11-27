using System;

namespace B4F.DataMigration.EffectenGiro
{
    public interface IEGAccount
    {
        IEGAanvraag AccountRequest { get; set; }
        string Bank { get; set; } 
        int LoginId { get; set; }
        string Nummer { get; set; }
        string NummerPreFix { get; }
        B4F.TotalGiro.Accounts.IAccount TGAccount { get; set; }
        B4F.TotalGiro.Stichting.IManagementCompany AssetManager { get; set; }
    }
}
