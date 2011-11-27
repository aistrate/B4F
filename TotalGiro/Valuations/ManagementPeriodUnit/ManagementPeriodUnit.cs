using System;
using System.Linq;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ManagementPeriodUnits.ReportData;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public class ManagementPeriodUnit : IManagementPeriodUnit
    {
        #region Constructor

        protected ManagementPeriodUnit() { }

        #endregion

        #region Properties

        /// <summary>
        /// The Key of the Unit.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// The management period
        /// </summary>
        public virtual IManagementPeriod ManagementPeriod
        {
            get { return this.managementPeriod; }
            protected set { this.managementPeriod = value; }
        }

        /// <summary>
        /// The management period
        /// </summary>
        public virtual IManagementPeriodUnitParent UnitParent
        {
            get { return this.unitParent; }
            protected set { this.unitParent = value; }
        }

        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual ICustomerAccount Account
        {
            get { return UnitParent.Account; }
        }

        /// <summary>
        /// The year + month concatenated as an integer
        /// </summary>
        public virtual int Period
        {
            get { return UnitParent.Period; }
        }

        /// <summary>
        /// The month of the period
        /// </summary>
        public virtual int Month
        {
            get { return UnitParent.Month; }
        }

        /// <summary>
        /// The begin date of the holding
        /// </summary>
        public virtual DateTime StartDate
        {
            get { return UnitParent.StartDate; }
        }

        /// <summary>
        /// The end date of the holding
        /// </summary>
        public virtual DateTime EndDate
        {
            get { return UnitParent.EndDate; }
        }

        /// <summary>
        /// The number of days that the holding was owned during the period
        /// </summary>
        public virtual int Days
        {
            get { return UnitParent.Days; }
        }

        /// <summary>
        /// Is this the last period of the managementperiod?
        /// </summary>
        public virtual bool IsPeriodEnd
        {
            get { return ManagementPeriod.EndDate.Equals(EndDate); }
        }

        /// <summary>
        /// Link to the previous holding, in case something changed after a mutation
        /// </summary>
        public virtual IPortfolioModel ModelPortfolio
        {
            get { return UnitParent.ModelPortfolio; }
            set { UnitParent.ModelPortfolio = value; }
        }

        public virtual bool IsExecOnlyCustomer
        {
            get { return UnitParent.IsExecOnlyCustomer; }
            set { UnitParent.IsExecOnlyCustomer = value; }
        }

        public Money TotalValue
        {
            get { return UnitParent.TotalValue; }
        }

        /// <summary>
        /// The transaction that is based on this data
        /// </summary>
        public virtual IManagementFee ManagementFee
        {
            get { return this.managementFee; }
            set { this.managementFee = value; }
        }

        public virtual IKickBackExport KickBackExport { get; set; }

        public virtual bool IsEditable 
        {
            get { return ManagementFee == null; }
        }

        public virtual int DocumentsSentByPost { get; set; }
        public virtual bool IsStornoed { get; set; }
        public virtual FeesCalculatedStates FeesCalculated { get; set; }
        public virtual int RulesFound { get; set; }
        public virtual bool Success { get; set; }
        public virtual string Message { get; set; }

        /// <summary>
        /// Link to the next Unit in case of a storno
        /// </summary>
        public virtual IManagementPeriodUnit NewManagementPeriodUnit
        {
            get { return this.newManagementPeriodUnit; }
            protected set { this.newManagementPeriodUnit = value; }
        }

        /// <summary>
        /// The holdings that belong to this item.
        /// </summary>
        public virtual IAverageHoldingCollection AverageHoldings
        {
            get { return UnitParent.AverageHoldings; }
        }

        /// <summary>
        /// The fee items that belong to this item.
        /// </summary>
        public virtual IManagementPeriodUnitFeeCollection FeeItems
        {
            get
            {
                ManagementPeriodUnitFeeCollection items = (ManagementPeriodUnitFeeCollection)this.feeItems.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        /// <summary>
        /// The fee items that belong to this item.
        /// </summary>
        public virtual IList<IAverageHoldingFee> AverageHoldingFeeItems
        {
            get { return averageHoldingFeeItems; }
        }

        /// <summary>
        /// The fee items that belong to this item.
        /// </summary>
        public virtual Money TotalFeeAmount
        {
            get
            {
                Money total = null;

                if (FeeItems != null)
                    total += FeeItems.TotalAmount;

                if (AverageHoldingFeeItems != null && AverageHoldingFeeItems.Count > 0)
                {
                    total += (from a in AverageHoldingFeeItems
                             select a.Amount).Sum();
                }   
                return total;
            }
        }

        public bool IsRelevantForFees 
        {
            get
            {
                bool retVal = false;
                if (Account.AccountType == AccountTypes.Customer)
                {
                    IAccountFamily family = ((ICustomerAccount)Account).Family;
                    if (family != null)
                    {
                        ManagementTypes mgtType = this.ManagementPeriod.ManagementType;
                        retVal = family.IsManagementTypeCharged(mgtType);

                        // extra check kickback
                        if (mgtType == ManagementTypes.KickBack && !Account.UseKickback)
                            retVal = false;
                    }
                    else
                        retVal = true;
                }
                return retVal;
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (UnitParent != null && ManagementPeriod != null)
                return string.Format("{0}-{1}", UnitParent.ToString(), ManagementPeriod.ManagementType.ToString());
            else
                return base.ToString();
        }

        #endregion

        #region Methods

        public bool IsBetween(DateTime date)
        {
            return UnitParent.IsBetween(date);
        }

        #endregion

        #region Privates

        private int key;
        private IManagementPeriod managementPeriod;
        private IManagementPeriodUnitParent unitParent;
        private IManagementFee managementFee;
        private IManagementPeriodUnit newManagementPeriodUnit;
        private IDomainCollection<IManagementPeriodUnitFee> feeItems;
        private IList<IAverageHoldingFee> averageHoldingFeeItems;

        #endregion

    }
}
