using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public interface ICouponHistory : ICorporateActionHistory
    {
        DateTime StartAccrualDate { get; }
        DateTime EndAccrualDate { get; }
    }
}
