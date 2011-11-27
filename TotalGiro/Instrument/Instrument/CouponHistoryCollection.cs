using System;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    public class CouponHistoryCollection : TransientDomainCollection<ICouponHistory>, ICouponHistoryCollection
    {
        public CouponHistoryCollection()
            : base() { }

        public CouponHistoryCollection(IBond parent)
            : base()
        {
            Parent = parent;
        }

        public virtual IBond Parent { get; set; }

        public ICouponHistory GetCouponByDate(DateTime date)
        {
            if (this.Where(x => Util.DateBetween(x.StartAccrualDate, x.EndAccrualDate, date)).Count() > 1)
                throw new ApplicationException("There are more than 1 coupons found for this date, contact your local developer");
            
            return this.Where(x => Util.DateBetween(x.StartAccrualDate, x.EndAccrualDate, date)).FirstOrDefault();
        }

        public void AddCoupon(ICouponHistory item)
        {
            if (item.Instrument.Key != Parent.Key)
                throw new ApplicationException(string.Format("Coupon History item is not for Instrument {0}.",
                    item.Instrument.DisplayIsinWithName));

            if (this.Where(x => (Util.DateBetween(item.StartAccrualDate, x.StartAccrualDate, x.EndAccrualDate))).Count() > 0)
                throw new ApplicationException(string.Format("Coupon History items for Instrument {0} already exist.",
                    item.Instrument.DisplayIsinWithName));

            Add(item);
        }
    }
}
