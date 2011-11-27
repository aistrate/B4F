using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments.CorporateAction;

namespace B4F.TotalGiro.Instruments
{
    public interface ICouponHistoryCollection : IList<ICouponHistory>
    {
        IBond Parent { get; set; }
        ICouponHistory GetCouponByDate(DateTime date);
        void AddCoupon(ICouponHistory item);
    }
}
