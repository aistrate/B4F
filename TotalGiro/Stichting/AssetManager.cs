using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Collections.Persistence;


namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// Class holds object of Asset Manager
    /// </summary>
    public class AssetManager : ManagementCompany,  IAssetManager
	{
		public AssetManager() {}

	    public string AccountNrPrefix
	    {
		    get { return accountNrPrefix;}
		    set { accountNrPrefix = value;}
	    }

	    public int AccountNrFountain
	    {
		    get { return accountNrFountain;}
		    set { accountNrFountain = value;}
	    }

        public short AccountNrLength
        {
            get { return accountNrLength; }
            set { accountNrLength = value; }
        }

        public string GenerateAccountNumber()
        {
            AccountNrFountain++;
            string format = new string('0', AccountNrLength);
            return accountNrPrefix + AccountNrFountain.ToString(format);
        }

        /// <summary>
        /// Get/set unique identifier of Backoffice
        /// </summary>
		public virtual string BoSymbol
		{
			get { return boSymbol; }
			set { boSymbol = value; }
		}		

        /// <summary>
        /// Get CompanyType 
        /// </summary>
        public override ManagementCompanyType CompanyType 
        { 
            get { return ManagementCompanyType.AssetManager; } 
        }

        public virtual string MgtFeeDescription { get; set; }
        public virtual string MgtFeeFinalDescription { get; set; }
        public virtual decimal MgtFeeThreshold { get; set; }
        public virtual bool DoNotChargeCommissionWithdrawals { get; set; }
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Get/set collection of Remisiers belonging to an Asset Manager
        /// </summary>
        public virtual IRemisierCollection Remisiers
        {
            get
            {
                RemisierCollection items = (RemisierCollection)this.remisiers.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return (IRemisierCollection)items;
            }
        }

        public virtual IAccountCategoryCollection AccountCategories
        {
            get
            {
                if (accountCategories == null)
                    this.accountCategories = new AccountCategoryCollection(bagOfAccountCategories, this);
                return accountCategories;
            }
            set { accountCategories = value; }
        }

        // The CashManagementFund used by the assetmanager
        public ICashManagementFund CashManagementFund
        {
            get { return cashManagementFund; }
            set { cashManagementFund = value; }
        }

        /// <summary>
        /// The model that is used for closed accounts
        /// </summary>
        public IPortfolioModel ClosedModelPortfolio
        {
            get { return closedModel; }
            set { closedModel = value; }
        }

        /// <summary>
        /// The instruments that this assetmanager is trading in.
        /// </summary>
        public virtual IList<ITradeableInstrument> Instruments
        {
            get
            {
                //TradeableInstrumentCollection items = (TradeableInstrumentCollection)this.AssetManagerInstruments.Select(u => u.Instrument).ToList(); 
                TradeableInstrumentCollection items = (TradeableInstrumentCollection)this.instruments.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items.AsReadOnlyList();
            }
        }

        /// <summary>
        /// The instruments that this assetmanager is trading in.
        /// </summary>
        public virtual IList<ITradeableInstrument> ActiveInstruments
        {
            get
            {
                IList<IAssetManagerInstrument> items = this.assetManagerInstruments.AsList();
                return new ReadOnlyCollection<ITradeableInstrument>(items.Where(u => u.IsActive == true).Select(u => u.Instrument).ToList());
            }
        }


        /// <summary>
        /// The instruments info that this assetmanager is trading in.
        /// </summary>
        public virtual IAssetManagerInstrumentCollection AssetManagerInstruments
        {
            get
            {
                AssetManagerInstrumentCollection items = (AssetManagerInstrumentCollection)this.assetManagerInstruments.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        public virtual bool SupportLifecycles { get; set; }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>name</returns>
		public override string ToString()
		{
            if (name != null)
                return this.CompanyName;
            else if (boSymbol != null)
                return this.BoSymbol;
            else
                return "";
        }

		#region Private Variables

		private string name;
		private string boSymbol;
        private IDomainCollection<IRemisier> remisiers;
        private IList bagOfAccountCategories = new ArrayList();
        private IAccountCategoryCollection accountCategories;
        private string accountNrPrefix;
        private int accountNrFountain;
        private short accountNrLength;
        private IDomainCollection<ITradeableInstrument> instruments;
        private IDomainCollection<IAssetManagerInstrument> assetManagerInstruments;
        private ICashManagementFund cashManagementFund;
        private IPortfolioModel closedModel;

		#endregion

		
	}
}
