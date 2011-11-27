using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    public class BondCouponRateHistory : IBondCouponRateHistory
    {
        public BondCouponRateHistory()
        {
            CreationDate = DateTime.Now;
        }

        public BondCouponRateHistory(DateTime startDate)
            : this()
        {
            CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
            this.StartDate = startDate;
        }

        public virtual int Key { get; set; }
        public virtual IBond Parent { get; set; }
        public virtual decimal CouponRate { get; set; }

        public virtual DateTime StartDate 
        { 
            get { return this.startDate; }
            set
            {
                if (Util.IsNotNullDate(this.EndDate) && value >= this.EndDate)
                    throw new ApplicationException("Start date can not be after the end date.");
                this.startDate = value;
            }
        }

        public virtual DateTime EndDate
        {
            get { return this.endDate.HasValue ? this.endDate.Value : DateTime.MinValue; }
            set 
            {
                if (Util.IsNotNullDate(value) && value <= this.StartDate)
                    throw new ApplicationException("End date can not be before the start date.");
                if (Util.IsNotNullDate(value))
                    this.endDate = value;
                else
                    this.endDate = null;
            }
        }

        public virtual DateTime CreationDate
        {
            get { return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; }
            set { this.creationDate = value; }
        }

        public virtual string CreatedBy { get; set; }
        public virtual DateTime LastUpdated { get; internal set; }
        public virtual string LastUpdatedBy { get; set; }

        #region Methods

        public int GetRelevantDaysInPeriod(DateTime startDate, DateTime endDate)
        {
            if (this.StartDate > startDate) startDate = this.StartDate;
            if (Util.IsNotNullDate(this.EndDate) && this.EndDate < endDate) endDate = this.EndDate;
            return Util.DateDiff(DateInterval.Day, startDate, endDate) - 1;
        }

        #endregion

        #region Privates

        private DateTime startDate;
        private DateTime? endDate;
        private DateTime? creationDate;

        #endregion

    }
}
