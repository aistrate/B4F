using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public class CouponHistory : CorporateActionHistory, ICouponHistory
    {
        protected CouponHistory() { }

        public CouponHistory(IBond instrument, DateTime startAccrualDate, DateTime endAccrualDate)
            : base(instrument)
        {
            if (Util.IsNullDate(startAccrualDate) || Util.IsNullDate(endAccrualDate) || startAccrualDate >= endAccrualDate)
                throw new ApplicationException("The accrual dates are not correct.");

            this.StartAccrualDate = startAccrualDate;
            this.EndAccrualDate = endAccrualDate;
            this.Description = string.Format("Coupon {0}: {1} - {2}", 
                instrument.DisplayIsinWithName, 
                startAccrualDate.ToString("dd-MM-yyyy"),
                endAccrualDate.ToString("dd-MM-yyyy"));
        }

        public virtual DateTime StartAccrualDate
        {
            get
            {
                return this.startAccrualDate.HasValue ? this.startAccrualDate.Value : DateTime.MinValue;
            }
            set
            {
                this.startAccrualDate = value;
            }
        }

        public virtual DateTime EndAccrualDate
        {
            get
            {
                return this.endAccrualDate.HasValue ? this.endAccrualDate.Value : DateTime.MinValue;
            }
            set
            {
                this.endAccrualDate = value;
            }
        }

        #region Overrides

        public override string DisplayString
        {
            get { return Description; }
        }

        public override string ToString()
        {
            return Description;
        }

        #endregion

        #region Privates

        private DateTime? startAccrualDate;
        private DateTime? endAccrualDate;
        private DateTime? paymentDate;

        #endregion  
    }
}
