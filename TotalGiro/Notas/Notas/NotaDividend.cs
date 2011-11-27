using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Notas 
{
    public class NotaDividend : NotaGeneralOperationsBookingTaxeable, INotaDividend
    {
        #region Constructor

        internal NotaDividend() { }

        public NotaDividend(ICashDividend underlyingBooking)
            : base(underlyingBooking)
        {
        }

        #endregion

        #region Props

        public override string Title
        {
            get { return "Dividenden"; }
        }

        public virtual Money DividendAmount
        {
            get { return ((ICashDividend)OriginalBooking).DividendAmount; }
        }

        public override Money Tax
        {
            get { return ((ICashDividend)OriginalBooking).TaxAmount; }
        }

        public virtual ITradeableInstrument Instrument
        {
            get { return ((ICashDividend)OriginalBooking).DividendDetails.Instrument; }
        }

        public virtual DateTime ExDividendDate
        {
            get 
            { 
                ICashDividend interim = OriginalBooking as ICashDividend;
                return interim.DividendDetails.ExDividendDate; 
            }
        }

        public virtual DateTime SettlementDate
        {
            get { return ((ICashDividend)OriginalBooking).DividendDetails.SettlementDate; }
        }

        public virtual InstrumentSize Units
        {
            get { return ((ICashDividend)OriginalBooking).UnitsInPossession; }
        }

        public virtual Price UnitPrice
        {
            get { return ((ICashDividend)OriginalBooking).UnitPrice; }
        }

        public virtual Money ValueIncludingTax
        {
            get { return base.TotalValue; }
        }

        #endregion
    }
}
