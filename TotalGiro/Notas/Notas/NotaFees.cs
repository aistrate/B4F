using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Notas
{
    public class NotaFees : NotaGeneralOperationsBookingTaxeable, INotaFees
    {
        #region Constructor

        internal NotaFees() { }

        public NotaFees(IManagementFee underlyingBooking)
            : base(underlyingBooking)
        {
        }

        #endregion

        #region Props

        public override string Title
        {
            get { return "Beheerloon"; }
        }

        /// <summary>
        /// The gross Management Fee amount
        /// </summary>
        public virtual Money ManagementFeeAmount
        {
            get { return ((IManagementFee)OriginalBooking).ManagementFeeAmount; }
        }

        public override Money Tax
        {
            get { return ((IManagementFee)OriginalBooking).TaxAmount; }
        }

        public override DateTime ModelDate
        {
            get { return PeriodEndDate; }
        }

        public virtual string Description
        {
            get { return UnderlyingBooking.Description; }
        }

        public virtual Money ValueIncludingTax
        {
            get { return base.TotalValue; }
        }

        public virtual DateTime TxStartDate
        {
            get { return ((IManagementFee)OriginalBooking).NotaStartDate; }
        }

        public virtual DateTime TxEndDate
        {
            get { return ((IManagementFee)OriginalBooking).NotaEndDate; }
        }

        public virtual DateTime PeriodStartDate
        {
            get { return TxStartDate; }
        }

        public virtual DateTime PeriodEndDate
        {
            get { return TxEndDate; }
        }

        public virtual IGeneralOperationsComponentCollection FeeDetails
        {
            get { return OriginalBooking.Components; }
        }

        public virtual IAverageHolding[] AverageHoldings
        {
            get 
            {
                return ((IManagementFee)OriginalBooking).Units.SelectMany(x => x.AverageHoldings).ToArray();
            }
        }

        #endregion
    }
}
