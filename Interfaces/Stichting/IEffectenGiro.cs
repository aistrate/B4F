using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.EffectenGiro">EffectenGiro</see> class
    /// </summary>
    public interface IEffectenGiro : IManagementCompany
	{
        Address ResidentialAddress { get; set; }
        IAssetManagerCollection AssetManagers { get; }
        ICountry Country { get; }
        ICrumbleAccount CrumbleAccount { get; }
        ICustodyAccount CustodianAccount { get; }
        IJournal DefaultWithdrawJournal { get; set; }
        string StichtingName { get; set; }
        IGLBookYear CurrentBookYear { get; set; }
        string ClientWebsiteUrl { get; set; }
	}
}
