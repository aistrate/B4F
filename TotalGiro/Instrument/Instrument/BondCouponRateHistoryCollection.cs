using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    public class BondCouponRateHistoryCollection : TransientDomainCollection<IBondCouponRateHistory>, IBondCouponRateHistoryCollection
    {
        public BondCouponRateHistoryCollection()
            : base() { }

        public BondCouponRateHistoryCollection(IBond parent)
            : base()
        {
            Parent = parent;
        }

        public virtual IBond Parent { get; set; }

        public IBondCouponRateHistory GetCouponRateByDate(DateTime date)
        {
            if (this.Where(x => Util.DateBetween(x.StartDate, x.EndDate, date)).Count() > 1)
                throw new ApplicationException("There are more than 1 coupons found for this date, contact your local developer");

            return this.Where(x => Util.DateBetween(x.StartDate, x.EndDate, date)).FirstOrDefault();
        }

        public decimal GetWeightedCouponRateForPeriod(DateTime startDate, DateTime endDate)
        {
            var crhs = this.Where(x => (Util.IsNotNullDate(x.EndDate) ? x.EndDate : DateTime.MaxValue) >= startDate &&
                x.StartDate <= endDate);

            if (crhs.Count() == 0)
                throw new ApplicationException(string.Format("No 1 coupon rate(s) found for this period ({0} <-> {1})", 
                    startDate.ToString("dd-MM-yyyy"), endDate.ToString("dd-MM-yyyy")));
            else if (crhs.Count() == 1)
                return crhs.First().CouponRate;
            else
            {
                int totalDays = 0;
                decimal dayRates = 0M;
                foreach (var item in crhs.OrderBy(x => x.StartDate))
                {
                    int days = item.GetRelevantDaysInPeriod(startDate, endDate);
                    dayRates += (days * item.CouponRate);
                    totalDays += days;
                }
                if (totalDays > 0)
                    return dayRates / totalDays;
                else
                    throw new ApplicationException("It's no work."); 
            }
        }


        public IBondCouponRateHistory LastCouponRate
        {
            get { return this.OrderByDescending(x => x.StartDate).FirstOrDefault(); }
        }

        public void AddCouponRate(IBondCouponRateHistory item)
        {
            IBondCouponRateHistory lastCouponRate = LastCouponRate;
            if (lastCouponRate != null && Util.IsNullDate(LastCouponRate.EndDate) && lastCouponRate.StartDate < item.StartDate)
                lastCouponRate.EndDate = item.StartDate.AddDays(-1);

            item.Parent = Parent;

            if (this.Where(x => (Util.DateBetween(item.StartDate, x.StartDate, x.EndDate))).Count() > 0)
                throw new ApplicationException(string.Format("CouponRate History items for Instrument {0} already exist.",
                    item.Parent.DisplayIsinWithName));
            Add(item);
        }

        /// <exclude/>
        public bool RemoveCouponRate(IBondCouponRateHistory item)
        {
            bool success = false;
            if (Contains(item))
            {
                success = this.Remove(item);
                item.Parent = null;
            }
            return success;
        }        
        
        public void CheckPeriodOverlap()
        {
            var rr = (
              from t1 in this
              from t2 in this
              where t1.Key != t2.Key
              && (Util.DateBetween(t2.StartDate, (Util.IsNotNullDate(t2.EndDate) ? t2.EndDate : DateTime.MaxValue), t1.StartDate) ||
                Util.DateBetween(t1.StartDate, (Util.IsNotNullDate(t1.EndDate) ? t1.EndDate : DateTime.MaxValue), t2.StartDate))
              select t1).Distinct();
            if (rr.Count() > 0)
                throw new ApplicationException(string.Format("{0} CouponRate History items have been found that overlap.", rr.Count()));
        }

        
    }
}
