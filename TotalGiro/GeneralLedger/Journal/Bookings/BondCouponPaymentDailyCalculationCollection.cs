using System;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using System.Collections.Generic;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class BondCouponPaymentDailyCalculationCollection : TransientDomainCollection<IBondCouponPaymentDailyCalculation>, IBondCouponPaymentDailyCalculationCollection
    {
        public BondCouponPaymentDailyCalculationCollection()
            : base() { }

        public BondCouponPaymentDailyCalculationCollection(IBondCouponPayment parent)
            : base()
        {
            Parent = parent;
        }

        public virtual IBondCouponPayment Parent { get; set; }

        public virtual Money TotalAccruedInterest
        {
            get { return this.Select(x => x.DailyInterest).Sum(); }
        }

        public virtual IBondCouponPaymentDailyCalculation LastCalculation 
        {
            get { return this.OrderByDescending(x => x.SettlementDate).FirstOrDefault(); }
        }

        public IBondCouponPaymentDailyCalculation GetCalculationByDate(DateTime settlementDate)
        {
            return this.Where(x => x.SettlementDate == settlementDate).FirstOrDefault();
        }

        public void AddCalculation(DateTime calcDate, DateTime settlementDate, 
            InstrumentSize positionSize, Money calculatedAccruedInterestUpToDate, 
            IList<IBondCouponPaymentDailyCalculation> oldCalculations)
        {
            IBondCouponPaymentDailyCalculation item = this.Where(x => x.SettlementDate == settlementDate && x.Parent.Key == this.Parent.Key).FirstOrDefault();
            if (item == null)
            {
                item = new BondCouponPaymentDailyCalculation(this.Parent, calcDate, settlementDate, positionSize, calculatedAccruedInterestUpToDate, oldCalculations);
                Add(item);
                item.Parent = this.Parent;
            }
            else
                item.Edit(positionSize, calculatedAccruedInterestUpToDate, oldCalculations);
        }
    }
}
