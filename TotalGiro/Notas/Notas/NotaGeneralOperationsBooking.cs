using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Notas
{
    public abstract class NotaGeneralOperationsBooking : Nota, INotaGeneralOperationsBooking
    {
        #region Constructor

        protected NotaGeneralOperationsBooking() { }

        protected NotaGeneralOperationsBooking(IGeneralOperationsBooking underlyingBooking)
            : base((ICustomerAccount)underlyingBooking.Account)
        {
            this.underlyingBooking = underlyingBooking;
            underlyingBooking.BookNota = this;
        }

        #endregion

        #region Props

        public virtual IGeneralOperationsBooking UnderlyingBooking
        {
            get { return underlyingBooking; }
            private set { underlyingBooking = value; }
        }

        public virtual INota StornoedBookNota
        {
            get { return (IsStorno ? UnderlyingBooking.StornoBooking.BookNota : null); }
        }

        public virtual IGeneralOperationsBooking OriginalBooking
        {
            get { return (IsStorno ? UnderlyingBooking.OriginalBooking : UnderlyingBooking); }
        }

        #endregion

        #region Overriden Props

        public override ICustomerAccount Account
        {
            get { return (ICustomerAccount)UnderlyingBooking.Account; }
        }

        public override bool IsStorno
        {
            get { return UnderlyingBooking.IsStorno; }
        }

        public override ICurrency InstrumentCurrency
        {
            get { return UnderlyingBooking.GeneralOpsJournalEntry.Journal.Currency; }
        }

        public override Money TotalValue
        {
            get { return (UnderlyingBooking.Components != null ? UnderlyingBooking.Components.TotalAmount : new Money(0.00m, InstrumentCurrency)); }
        }

        public override DateTime TransactionDate
        {
            get { return UnderlyingBooking.BookDate; }
        }

        public override decimal ExchangeRate
        {
            get { return UnderlyingBooking.GeneralOpsJournalEntry.ExchangeRate; }
        }

        #endregion

        #region Private Variables

        private IGeneralOperationsBooking underlyingBooking;

        #endregion
    }
}
