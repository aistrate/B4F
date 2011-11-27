using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    /// <summary>
    /// Abstract class from which all Fee Calculation classes are derived.
    /// </summary>
    public class FeeCalc : IFeeCalc
    {
        #region Constructor

        protected FeeCalc() 
        {
            this.IsActive = true;
        }

        public FeeCalc(FeeType feeType, string name, ICurrency feeCurrency, IAssetManager assetManager)
            : this()
        {
            if ((feeType == null || feeType.Key == FeeTypes.None)|| string.IsNullOrEmpty(name) || feeCurrency == null || assetManager == null)
                throw new ApplicationException("Not all parameters are passed in to instantiate a new fee calculation");
            
            FeeType = feeType;
            Name = name;
            FeeCurrency = feeCurrency;
            this.AssetManager = assetManager;

            checkData();
        }

        private void checkData()
        {
            if (Name.Equals(string.Empty))
                throw new ApplicationException("The name is mandatory");

            if (FeeCurrency == null)
                throw new ApplicationException("fee currency is mandatory");

            if (AssetManager == null)
                throw new ApplicationException("The AssetManager is mandatory");
        }

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the fee calculation.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// The Asset manager this calculation belongs to.
        /// </summary>
        public virtual IAssetManager AssetManager
        {
            get { return this.assetManager; }
            set { this.assetManager = value; }
        }

        /// <summary>
        /// The name of the fee calculation.
        /// </summary>
        public virtual string Name
        {
            get { return this.calcName; }
            set { this.calcName = value; }
        }

        /// <summary>
        /// The currency of the fee.
        /// </summary>
        public virtual ICurrency FeeCurrency
        {
            get { return this.feeCurrency; }
            internal set { this.feeCurrency = value; }
        }

        /// <summary>
        /// Indicates whether the fee calculation is either flat or simple.
        /// </summary>
        public virtual FeeType FeeType
        {
            get { return this.feeType; }
            internal set { this.feeType = value; }
        }

        public virtual bool IsActive { get; set; }

        public ManagementTypes ManagementType 
        {
            get
            {
                return FeeType.ManagementType;
            }
        }

        /// <summary>
        /// Date/time this fee calc was created
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
            internal set { this.creationDate = value; }
        }

        /// <summary>
        /// Date/time when this fee calc has last been updated
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        /// <summary>
        /// A collection of child <b>FeeCalcVersions</b> objects belonging to the Fee Calculation.
        /// </summary>
        public virtual IFeeCalcVersionCollection Versions
        {
            get
            {
                if (this.feeVersions == null)
                    this.feeVersions = new FeeCalcVersionCollection(this, versions);
                return feeVersions;
            }
        }

        public virtual IFeeCalcVersion LatestVersion 
        {
            get { return Versions.LatestVersion(); }
        }

        #endregion

        #region Override

        /// <summary>
        /// A string representation of the fee calculation; returns the value of property <b>Name</b>.
        /// </summary>
        /// <returns>The fee calculation's <b>Name</b> property value.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Hash function for this type. 
        /// </summary>
        /// <returns>A hash code for the current FeeCalc object.</returns>
        public override int GetHashCode()
        {
            return this.key.GetHashCode();
        }

        #endregion

        #region Private Variables

        private int key;
        private IAssetManager assetManager;
        private string calcName;
        private ICurrency feeCurrency;
        private FeeType feeType;
        private DateTime creationDate = DateTime.Now;
        private DateTime lastUpdated;
        private IList versions = new ArrayList();
        private IFeeCalcVersionCollection feeVersions;


        #endregion

    }

}
