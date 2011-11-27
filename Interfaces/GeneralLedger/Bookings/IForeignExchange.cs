using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IForeignExchange
    {
        Money AmountinBaseCurrency { get; }
        Money AmountinForeignCurrency { get; }
    }
}
