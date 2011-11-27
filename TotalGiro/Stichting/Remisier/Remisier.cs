using System;
using System.Collections;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.Stichting.Remisier
{
    /// <summary>
    /// Class holds Remisier object
    /// </summary>
    public class Remisier : IRemisier
    {
        protected Remisier() { }

        public Remisier(IAssetManager assetManager, RemisierTypes remisierType, string name) 
        {
            if (assetManager == null)
                throw new ApplicationException("Asset Manager is mandatory");

            if (string.IsNullOrEmpty(name))
                throw new ApplicationException("Remisier Name is mandatory");

            this.AssetManager = assetManager;
            this.RemisierType = remisierType;
            this.Name = name;
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Key
        {
            get { return key; }
            set { key = value; }
        }        

        public string InternalRef
        {
            get { return internalRef; }
            set { internalRef = value; }
        }

        /// <summary>
        /// The type of remisier
        /// </summary>
        public RemisierTypes RemisierType
        {
            get 
            {
                RemisierTypes t;
                switch (remisierType.ToLower())
                {
                    case "beheerder":
                        t = RemisierTypes.InternalBeheer;
                        break;
                    case "vermogensbeheerder":
                        t = RemisierTypes.InternalVermogensbeheer;
                        break;
                    case "inkooporganisatie":
                        t = RemisierTypes.Inkooporganisatie;
                        break;
                    default:
                        t = RemisierTypes.Remisier;
                        break;
                }
                return t; 
            }
            set 
            {
                string s = "";
                switch (value)
                {
                    case RemisierTypes.Inkooporganisatie:
                        s = "Inkooporganisatie";
                        break;
                    case RemisierTypes.InternalVermogensbeheer:
                        s = "Vermogensbeheerder";
                        break;
                    case RemisierTypes.InternalBeheer:
                        s = "Beheerder";
                        break;
                    default:
                        s = "Remisier";
                        break;
                }
                remisierType = s; 
            }
        }

        /// <summary>
        /// Is this a internal remisier
        /// </summary>
        public bool IsInternal
        {
            get 
            {
                bool isInternal = false;
                if (RemisierType == RemisierTypes.InternalBeheer || RemisierType == RemisierTypes.InternalVermogensbeheer)
                    isInternal = true;
                return isInternal; 
            }
        }

        /// <summary>
        /// Get/set Remisier name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Get/set Remisier name + Ref Number
        /// </summary>
        public string DisplayNameAndRefNumber
        {
            get { return Name + (!string.IsNullOrEmpty(InternalRef) ? string.Format(" ({0})", InternalRef) : ""); }
        }

        /// <summary>
        /// Get/set Remisier name & City
        /// </summary>
        public string DisplayName
        {
            get 
            { 
                string name = Name;
                if (OfficeAddress != null && !string.IsNullOrEmpty(OfficeAddress.City))
                    name += string.Format(" ({0})", OfficeAddress.City);
                else if (PostalAddress != null && !string.IsNullOrEmpty(PostalAddress.City))
                    name += string.Format(" ({0})", PostalAddress.City);
                return name;
            }
        }

        public Address OfficeAddress
        {
            get { return officeAddress; }
            set { officeAddress = value; }
        }        

        public Address PostalAddress
        {
            get { return postalAddress; }
            set { postalAddress = value; }
        }        

        public TelephoneNumber Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }        

        public TelephoneNumber Fax
        {
            get { return fax; }
            set { fax = value; }
        }       

        public string Email
        {
            get { return email; }
            set { email = value; }
        }        

        public Person ContactPerson
        {
            get { return contactPerson; }
            set { contactPerson = value; }
        }

        public IBankDetails BankDetails
        {
            get { return bankDetails; }
            set { bankDetails = value; }
        }

        public virtual IRemisier ParentRemisier { get; set; }
        public virtual decimal ParentRemisierKickBackPercentage { get; set; }

        /// <summary>
        /// Get/set collection of Remisiers belonging to this Parent Remisier
        /// </summary>
        public virtual IRemisierCollection ChildRemisiers
        {
            get
            {
                RemisierCollection items = (RemisierCollection)this.childRemisiers.AsList();
                if (items.Parent == null)
                    items.Parent = this.AssetManager;
                return (IRemisierCollection)items;
            }
        }

        public virtual string ProvisieAfspraak { get; set; }
        public virtual string DatumOvereenkomst { get; set; }
        public virtual string NummerOvereenkomst { get; set; }
        public virtual string NummerAFM { get; set; }
        public virtual string NummerKasbank { get; set; }	 

        ///// <summary>
        ///// Fee percentage used for calculation of the fee calculated for a deposit
        ///// </summary>
        //public decimal DepositFeePercentage
        //{
        //    get { return this.depositFeePercentage; }
        //    set { this.depositFeePercentage = value; }
        //}
	
        ///// <summary>
        ///// Fee percentage used for calculation of the fee calculated for a withdrawal
        ///// </summary>
        //public decimal WithdrawalFeePercentage
        //{
        //    get { return this.withdrawalFeePercentage; }
        //    set { this.withdrawalFeePercentage = value; }
        //}

        ///// <summary>
        ///// Percentage on top of the Deposit Fee. The fee is for the assetmanager
        ///// </summary>
        //public decimal DepositFeeMarginForAssetManager
        //{
        //    get { return depositFeeMarginForAssetManager; }
        //    set { depositFeeMarginForAssetManager = value; }
        //}

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; deleted = !value; }
        }

        public IAssetManager AssetManager
        {
            get { return assetManager; }
            set { assetManager = value; }
        }
	
        ///// <summary>
        ///// Get/set collection of Asset Managers
        ///// </summary>
        //public virtual IAssetManagerCollection AssetManagers
        //{
        //    get
        //    {
        //        if (assetManagers == null)
        //            this.assetManagers = new AssetManagerCollection(bagOfAssetManagers, this);
        //        return assetManagers;
        //    }
        //    set { assetManagers = value; }
        //}

        public virtual IRemisierEmployeesCollection Employees
        {
            get
            {
                RemisierEmployeesCollection items = (RemisierEmployeesCollection)this.employees.AsList();
                if (items.ParentRemisier == null)
                    items.ParentRemisier = this;
                return items;
            }
        }

        #region Private Variables

        private int key;
        private string internalRef;
        private string name;
        private string remisierType;

        private Address officeAddress;
        private Address postalAddress;
        private TelephoneNumber telephone = new TelephoneNumber();
        private TelephoneNumber fax = new TelephoneNumber();
        private string email;
        private Person contactPerson;
        private IBankDetails bankDetails;
        private string provisieAfspraak;

        private IAssetManager assetManager;

        private decimal depositFeePercentage;
        private decimal withdrawalFeePercentage;
        private decimal depositFeeMarginForAssetManager;

        private IDomainCollection<IRemisierEmployee> employees;

        private IList bagOfAssetManagers = new ArrayList();
        private IAssetManagerCollection assetManagers;
        private IDomainCollection<IRemisier> childRemisiers;
        private bool isActive;
        private bool deleted;

        #endregion

    }
}
