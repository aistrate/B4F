using System;
using System.Collections.Generic;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public interface ICashDividendCollection : IList<ICashDividend>
    {
        void AddCashDividend( ICashDividend line);
        IDividendHistory Parent { get; set; }
        InstrumentSize TotalUnits { get; }
        Money TotalDividendAmount { get; }
    }
}
