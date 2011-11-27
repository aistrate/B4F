using System;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public abstract class GeneralOperationsBookingTaxeable : GeneralOperationsBooking, IGeneralOperationsBookingTaxeable
    {
        #region Constructor

        protected GeneralOperationsBookingTaxeable() { }

        public GeneralOperationsBookingTaxeable(IAccountTypeCustomer account, IMemorialBooking journalEntry, string description, decimal taxPercentage)
            : base(account, journalEntry, description)
        {
            if (taxPercentage < 0 || taxPercentage > 1)
                throw new ApplicationException("The Tax Percentage can only be between 0 and 1");

            this.TaxPercentage = taxPercentage;
        }

        #endregion

        #region Props

        public virtual IMemorialBooking MemorialBooking { get { return (IMemorialBooking)this.GeneralOpsJournalEntry; } }
        public virtual decimal TaxPercentage { get; set; }

        public abstract Money NettAmount { get; }
        public abstract Money TaxAmount { get; }

        #endregion
    }
}
