using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface ICashDividend : IGeneralOperationsBookingTaxeable
    {
        IDividendHistory DividendDetails { get; set; }
        DateTime ExDividendDate { get;  }
        DateTime SettlementDate { get; }
        Money DividendAmount { get; }
        Price UnitPrice { get; }
        InstrumentSize UnitsInPossession { get; set; }
        void Execute();
    }
}
