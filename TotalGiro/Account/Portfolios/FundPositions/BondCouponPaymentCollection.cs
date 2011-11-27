using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public class BondCouponPaymentCollection : TransientDomainCollection<IBondCouponPayment>, IBondCouponPaymentCollection
    {
        public BondCouponPaymentCollection()
            : base() { }

        public BondCouponPaymentCollection(IFundPosition parentPosition)
            : base()
        {
            ParentPosition = parentPosition;
        }

        public IFundPosition ParentPosition { get; set; }

        public IBondCouponPayment ActivePayment
        {
            get { return this.Find(x => x.Status == BondCouponPaymentStati.Active); }
        }

        public List<IBondCouponPayment> ToBeSettledPayments(DateTime date)
        {
            return this.Where(x => x.Status == BondCouponPaymentStati.ToBeSettled && x.CouponHistory.EndAccrualDate <= date).ToList(); 
        }

        public void AddPayment(IBondCouponPayment item)
        {
            if (this.Where(x => x.Status == BondCouponPaymentStati.Active ||
                (Util.DateBetween(item.CouponHistory.StartAccrualDate, x.CouponHistory.StartAccrualDate, x.CouponHistory.EndAccrualDate) && !(x.StornoBooking != null || x.IsStorno)))
                .Count() > 0)
                throw new ApplicationException(string.Format("Bond interest payments for account {0} and Instrument {1} already exist.",
                    ParentPosition.Account.DisplayNumberWithName,
                    ParentPosition.Instrument.DisplayIsinWithName));
            
            Add(item);
            item.Position = ParentPosition;
        }

        public IBondCouponPayment GetBondCouponPaymentByDate(DateTime date)
        {
            IBondCouponPayment bip = null;
            IList<IBondCouponPayment> bipsForPos = this.Where(x =>
                x.Status == BondCouponPaymentStati.Active && Util.DateBetween(x.CouponHistory.StartAccrualDate, x.CouponHistory.EndAccrualDate, date))
                .ToList();

            if (bipsForPos.Count > 1)
                throw new ApplicationException(string.Format("There are {0} active bond interest payments for account {1} and Instrument {2}. Only one is allowed.",
                    bipsForPos.Count,
                    ParentPosition.Account.DisplayNumberWithName,
                    ParentPosition.Instrument.DisplayIsinWithName));
            else if (bipsForPos.Count == 1)
                bip = bipsForPos[0];
            return bip;
        }
    }
}
