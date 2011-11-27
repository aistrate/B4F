using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public class ManagementPeriodUnitParent : IManagementPeriodUnitParent
    {
        #region Constructor

        protected ManagementPeriodUnitParent() { }

        #endregion

        #region Properties

        /// <summary>
        /// The Key of the UnitParent.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual ICustomerAccount Account
        {
            get { return this.account; }
            protected set { this.account = value; }
        }

        /// <summary>
        /// The year + month concatenated as an integer
        /// </summary>
        public virtual int Period
        {
            get { return period; }
            protected set { period = value; }
        }

        /// <summary>
        /// The month of the period
        /// </summary>
        public virtual int Month
        {
            get { return Convert.ToInt32(Period.ToString().Substring(4)); }
        }

        /// <summary>
        /// The begin date of the holding
        /// </summary>
        public virtual DateTime StartDate
        {
            get { return this.startDate; }
            protected set { this.startDate = value; }
        }

        /// <summary>
        /// The end date of the holding
        /// </summary>
        public virtual DateTime EndDate
        {
            get { return this.endDate; }
            protected set { this.endDate = value; }
        }

        /// <summary>
        /// The number of days that the holding was owned during the period
        /// </summary>
        public virtual int Days
        {
            get { return Util.DateDiff(DateInterval.Day, StartDate, EndDate); }
        }

        /// <summary>
        /// Link to the previous holding, in case something changed after a mutation
        /// </summary>
        public virtual IPortfolioModel ModelPortfolio
        {
            get { return model; }
            set { model = value; }
        }

        public virtual bool IsExecOnlyCustomer { get; set; }

        public Money TotalValue { get; protected set; }

        /// <summary>
        /// The holdings that belong to this item.
        /// </summary>
        public virtual IAverageHoldingCollection AverageHoldings
        {
            get
            {
                AverageHoldingCollection holdings = (AverageHoldingCollection)averageHoldings.AsList();
                if (holdings.Parent == null)
                    holdings.Parent = this;
                return holdings;
            }
        }

        /// <summary>
        /// The units that belong to this item.
        /// </summary>
        public virtual IManagementPeriodUnitCollection ManagementPeriodUnits
        {
            get
            {
                IManagementPeriodUnitCollection units = (IManagementPeriodUnitCollection)managementPeriodUnits.AsList();
                return units;
            }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        #endregion

        #region Methods

        public bool IsBetween(DateTime date)
        {
            bool retVal = false;
            if (date >= StartDate && date <= EndDate)
                retVal = true;
            return retVal;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (Account != null && Period > 0)
                return string.Format("{0}-{1}", Account.Number, Period.ToString());
            else
                return base.ToString();
        }

        #endregion

        #region Privates

        private int key;
        private ICustomerAccount account;
        private int period;
        private DateTime startDate;
        private DateTime endDate;
        private DateTime creationDate = DateTime.Now;
        private IPortfolioModel model;
        private IDomainCollection<IAverageHolding> averageHoldings;
        private IDomainCollection<IManagementPeriodUnit> managementPeriodUnits;

        #endregion

    }
}
