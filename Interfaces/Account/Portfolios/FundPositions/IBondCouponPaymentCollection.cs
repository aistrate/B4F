using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public interface IBondCouponPaymentCollection : IList<IBondCouponPayment>
    {
        IFundPosition ParentPosition { get; set; }
        IBondCouponPayment ActivePayment { get; }
        List<IBondCouponPayment> ToBeSettledPayments(DateTime date);
        void AddPayment(IBondCouponPayment item);
        IBondCouponPayment GetBondCouponPaymentByDate(DateTime date);
    }
}
