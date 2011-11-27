using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Accounts.ManagementPeriods
{
    public class ManagementPeriod : IManagementPeriod
    {
        #region Constructor
        
        protected ManagementPeriod() { }

        public ManagementPeriod(ManagementTypes managementType, DateTime startDate)
        {
            if (Util.IsNullDate(startDate))
                throw new ApplicationException("Start date can not be empty.");
            
            this.ManagementType = managementType;
            this.StartDate = startDate.Date;
            this.Employee = SecurityManager.CurrentUser;
            this.lastUpdated = DateTime.Now;
        }

        #endregion

        #region Props

        public virtual int Key { get; set; }
        public virtual ICustomerAccount Account { get; set; }
        public virtual ManagementTypes ManagementType { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate 
        {
            get { return endDate; }
            set
            {
                if (validateNewEndDate(
                    EndDate.HasValue ? EndDate.Value : DateTime.MinValue, 
                    value.HasValue ? value.Value : DateTime.MinValue,
                    false))
                    endDate = value;
            }
        }

        public virtual string EndDateDisplayString
        {
            get
            {
                if (EndDate.HasValue)
                    return EndDate.Value.ToString("dd MMM yyyy");
                else
                    return "";
            }
        }

        public virtual string Employee
        {
            get { return this.employee; }
            private set { this.employee = value; }
        }

        /// <summary>
        /// The units that belong to this item.
        /// </summary>
        public virtual IManagementPeriodUnitCollection ManagementPeriodUnits
        {
            get
            {
                IManagementPeriodUnitCollection units = (IManagementPeriodUnitCollection)managementPeriodUnits.AsList();
                if (units.Parent == null)
                    units.Parent = this;
                return units;
            }
        }

        /// <summary>
        /// The date that this management period was created
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
        }

        /// <summary>
        /// The last modification date of this management period
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        /// <summary>
        /// The last modification date of this management period
        /// </summary>
        public virtual bool IsEditable
        {
            get 
            {
                bool isEditable = false;
                switch (ManagementType)
                {
                    case ManagementTypes.ManagementFee:
                        if (Account.CurrentManagementFeePeriod != null && Account.CurrentManagementFeePeriod.Equals(this))
                            isEditable = true;
                        break;
                    case ManagementTypes.KickBack:
                        var p = (from mp in Account.ManagementPeriods
                                where mp.ManagementType == ManagementTypes.KickBack
                                orderby mp.StartDate descending
                                select mp).ToArray();
                        
                        if (p.Count() == 1)
                            isEditable = true;
                        else
                        {
                            if (p[0].Equals(this))
                                isEditable = true;
                        }
                        break;
                }
                return isEditable; 
            }
        }


        #endregion

        #region Methods

        public bool Edit(DateTime startDate, DateTime endDate)
        {
            DateTime currentEndDate = DateTime.MinValue;
            if (this.EndDate.HasValue) currentEndDate = this.EndDate.Value.Date;
            
            if (this.StartDate.Equals(startDate) && (this.EndDate.Equals(endDate) || (Util.IsNullDate(this.EndDate) && Util.IsNullDate(endDate))))
                return true;
            
            if (Util.IsNullDate(startDate))
                throw new ApplicationException("Start date can not be empty.");

            if (Util.IsNotNullDate(this.StartDate) && !this.StartDate.Equals(startDate))
            {
                if (this.ManagementPeriodUnits != null && this.ManagementPeriodUnits.Count > 0)
                    throw new ApplicationException("It is not possible to change the start date when units already exist. Contact your system administrator.");
            }

            if (Util.IsNotNullDate(endDate) && endDate <= startDate)
                throw new ApplicationException("End date can not be before the start date.");

            if (Account != null && Key != 0)
            {
                foreach (IManagementPeriod hist in Account.ManagementPeriods)
                {
                    if (hist.Key != Key)
                    {
                        if (Util.IsNullDate(endDate) && hist.ManagementType.Equals(ManagementType) && Util.IsNullDate(hist.EndDate))
                            throw new ApplicationException("It is not possible to have two management periods with an open end date.");

                        if (hist.IsBetween(ManagementType, startDate) ||
                            (Util.IsNotNullDate(endDate) && hist.IsBetween(ManagementType, endDate)))
                            throw new ApplicationException("An overlap was found within two management periods.");
                    }
                }
            }


            this.StartDate = startDate.Date;
            if (validateNewEndDate(currentEndDate, endDate, true))
                this.EndDate = endDate.Date;
            this.Employee = SecurityManager.CurrentUser;
            this.lastUpdated = DateTime.Now;
            return true;
        }

        private bool validateNewEndDate(DateTime currentEndDate, DateTime newEndDate, bool raiseError)
        {
            bool success = true;
            if (newEndDate != currentEndDate)
            {
                // check if management Period is still editable
                if (Util.IsNotNullDate(currentEndDate)) // && (currentEndDate.Day < Util.GetLastDayOfMonth(currentEndDate).Day))
                {
                    int p = (from a in this.ManagementPeriodUnits
                             where a.Period == Util.GetPeriodFromDate(currentEndDate)
                             select a).Count();
                    if (p > 0)
                        success = false;
                }

                else if (Util.IsNotNullDate(newEndDate))
                {
                    ////if last day of month -> check if not already units exists
                    //if (newEndDate.Day < Util.GetLastDayOfMonth(newEndDate).Day)
                    //{
                    //    int p = (from a in this.ManagementPeriodUnits
                    //             where a.Period >= Util.GetPeriodFromDate(newEndDate)
                    //             select a).Count();
                    //    if (p > 0)
                    //        success = false;
                    //}
                    //else
                    //{
                        int p = (from a in this.ManagementPeriodUnits
                                 where a.Period >= Util.GetPeriodFromDate(newEndDate)
                                 select a).Count();
                        if (p > 0)
                            success = false;
                    //}

                        if (raiseError && !success)
                            throw new ApplicationException("This management period is no longer editable. Contact your local developer.");
                }
            }
            return success;
        }

        public bool IsBetween(DateTime date)
        {
            return IsBetween(this.ManagementType, date);
        }

        public bool IsBetween(ManagementTypes managementType, DateTime date)
        {
            bool retVal = false;
            if (managementType == this.ManagementType)
            {
                if (date >= StartDate && date <= EndDate)
                    retVal = true;
            }
            return retVal;
        }

        #endregion

        #region Privates

        private IDomainCollection<IManagementPeriodUnit> managementPeriodUnits;
        private string employee;
        private DateTime? endDate;
        private DateTime creationDate;
        private DateTime lastUpdated = DateTime.Now;

        #endregion

    }
}
