using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// Class representing a company which has a EffectenGiro permit
    /// </summary>
    public class EffectenGiro : ManagementCompany, IEffectenGiro
    {
        protected EffectenGiro() { }

        /// <summary>
        /// Get/set a Custodian belonging to an EffectenGiro company
        /// </summary>
        /// 
        public Address ResidentialAddress { get; set; }
        public ICountry Country { get; internal set; }
        public ICrumbleAccount CrumbleAccount { get; set; }
        public ICustodyAccount CustodianAccount { get; set; }
        public IJournal DefaultWithdrawJournal { get; set; }
        public string StichtingName { get; set; }
        public IGLBookYear CurrentBookYear { get; set; }
        public string ClientWebsiteUrl { get; set; }

        public override ManagementCompanyType CompanyType { get { return ManagementCompanyType.EffectenGiro; } }

        /// <summary>
        /// Get/set collection of Asset Managers in relation with an EfectenGiro company
        /// </summary>
        public virtual IAssetManagerCollection AssetManagers
        {
            get
            {
                AssetManagerCollection items = (AssetManagerCollection)this.assetManagers.AsList();
                if (items.ParentEffectenGiro == null)
                    items.ParentEffectenGiro = this;
                return items;
            }
        }


        #region Private Variables

        private IDomainCollection<IAssetManager> assetManagers;
        private IAccountTypeExternal defaultAccountforTransfer;

        #endregion
    }
}
