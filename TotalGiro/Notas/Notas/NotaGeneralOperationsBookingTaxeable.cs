using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Notas
{
    public abstract class NotaGeneralOperationsBookingTaxeable : NotaGeneralOperationsBooking, INotaGeneralOperationsBookingTaxeable
    {
        #region Constructor

        protected NotaGeneralOperationsBookingTaxeable() { }

        protected NotaGeneralOperationsBookingTaxeable(IGeneralOperationsBooking underlyingBooking)
            : base(underlyingBooking)
        {
        }

        #endregion

        #region Props

        public abstract Money Tax { get; }

        public virtual decimal TaxPercentage
        {
            get { return UnderlyingBooking.TaxPercentage; }
        }

        public override Money GrandTotalValue
        {
            get { return TotalValue.BaseAmount + Tax; }
        }

        #endregion
    }
}
