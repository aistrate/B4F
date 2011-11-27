using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Instruments.Nav
{
    [Flags]
    public enum NavCalculationStati
    {
        None = 0,
        New = 1,
        Open = 2,
        Booked = 4
    }

    public interface INavCalculation
    {
        int Key { get; set; }
        DateTime ValuationDate { get; set; }
        IVirtualFund Fund { get; set; }
        INavPortfolio Portfolio { get;  }
        Money NettAssetValue { get; set; }
        Money NavPerUnit { get; set; }
        Money PublicOfferPrice { get; set; }
        string PublicOfferPriceDisplayString { get; }
        string NavPerUnitDisplayString { get; }
        decimal TotalParticipationsBeforeFill { get; set; }
        decimal TotalParticipationsAfterFill { get; set; }
        INavCalculation PrevNavCalculation { get; set; }
        NavCalculationStati Status { get; set; }
        string DisplayStatus { get; }
        Money GrossAssetValue { get; set; }
        INavCalculationOrderCollection NewOrders { get; }
        IMemorialBooking Bookings { get; set; }
    }
}
