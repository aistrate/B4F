using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class ManagementFee : GeneralOperationsBookingTaxeable, IManagementFee
    {
        #region Constructor

        protected ManagementFee() { }

        public ManagementFee(IAccountTypeCustomer account, DateTime startDate, DateTime endDate, IList<IManagementPeriodUnit> units, IMemorialBooking journalEntry, decimal taxPercentage, IGLLookupRecords lookups)
            : base(account, journalEntry, journalEntry.Description, taxPercentage)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;

            if (units == null)
                throw new ApplicationException("The units can not be null");

            foreach (IManagementPeriodUnit unit in units)
            {
                if (unit.ManagementFee != null)
                    throw new ApplicationException(string.Format("The unit {0} is already used for a management fee transaction.", unit.Key.ToString()));

                if (!(unit.Success && unit.FeesCalculated == FeesCalculatedStates.Yes))
                    throw new ApplicationException(string.Format("The unit {0} is not correct for the management fee transaction.", unit.Key.ToString()));
            }

            this.Units = units;
            this.GeneralOpsJournalEntry = journalEntry;
            createComponents(lookups);
        }

        //public ManagementFee(B4F.TotalGiro.Orders.OldTransactions.IObsoleteManagementFee oldFee, IMemorialBooking journalEntry, decimal taxPercentage, IGLLookupRecords lookups)
        //{
        //    this.GeneralOpsJournalEntry = journalEntry;
        //    if (oldFee.Units != null) this.Units = units;
        //    if (oldFee.StartDate != null) this.StartDate = oldFee.StartDate;
        //    if (oldFee.EndDate != null) this.EndDate = oldFee.EndDate;
        //    migrateOldComponents(lookups, oldFee, taxPercentage);
        //}

        #endregion

        #region Props

        /// <summary>
        /// The gross Management Fee amount
        /// </summary>
        public virtual Money ManagementFeeAmount
        {
            get { return Components.ReturnComponentValue(new BookingComponentTypes[] { BookingComponentTypes.ManagementFee, BookingComponentTypes.ManagementFeeFixedCosts, BookingComponentTypes.AdministrationCosts }); }
        }

        public override Money NettAmount
        {
            get
            {
                return this.Components.Where(x => x.BookingComponentType != BookingComponentTypes.Tax).Select(x => x.ComponentValue).Sum();
            }
        }

        public override Money TaxAmount
        {
            get
            {
                Money tax = Components.ReturnComponentValue(BookingComponentTypes.Tax);
                if (tax == null)
                {
                    if (NettAmount != null)
                        tax = new Money(0m, (ICurrency)NettAmount.Underlying);
                    else
                        tax = new Money(0m, this.GeneralOpsJournalEntry.Journal.Currency);
                }
                return tax;
            }
        }

        /// <summary>
        /// the start date
        /// </summary>
        public virtual DateTime StartDate
        {
            get { return startDate; }
            internal set { startDate = value; }
        }

        /// <summary>
        /// The end date
        /// </summary>
        public virtual DateTime EndDate
        {
            get { return endDate; }
            internal set { endDate = value; }
        }

        public virtual DateTime NotaStartDate
        {
            get
            {
                if (Util.IsNullDate(this.notaStartDate))
                    getNotaDates();
                return this.notaStartDate;
            }
        }

        public virtual DateTime NotaEndDate
        {
            get
            {
                if (Util.IsNullDate(this.notaEndDate))
                    getNotaDates();
                return this.notaEndDate;
            }
        }

        private void getNotaDates()
        {
            var p = from a in Units
                    group a by a.Period into g
                    select g.Key;

            if (p != null && p.Count() > 0)
            {
                int year;
                int quarter;
                if (Util.GetQuarterYearByPeriod(p.First(), out year, out quarter))
                    Util.GetDatesFromQuarter(year, quarter, out this.notaStartDate, out this.notaEndDate);
            }
        }

        public virtual IList<IManagementPeriodUnit> Units
        {
            get { return units; }
            internal set
            {
                this.units = value;
                if (this.units != null && this.units.Count > 0)
                {
                    foreach (IManagementPeriodUnit unit in this.units)
                        unit.ManagementFee = this;
                }

            }
        }

        #endregion

        #region Overrides

        public override GeneralOperationsBookingTypes BookingType { get { return GeneralOperationsBookingTypes.ManagementFee; } }

        public override IGeneralOperationsBooking Storno(IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry)
        {
            ManagementFee newStorno = new ManagementFee();
            newStorno.StartDate = this.StartDate;
            newStorno.EndDate = this.EndDate;
            newStorno.Units = this.Units;
            IManagementFee stornoBooking = (IManagementFee)this.storno(employee, reason, journalEntry, newStorno);
            stornoBooking.BookLines();
            return stornoBooking;
        }

        /// <summary>
        /// This method creates the nota of the ManagementFee
        /// </summary>
        /// <returns>True when successfull</returns>
        public override INota CreateNota()
        {
            if (this.GeneralOpsJournalEntry.Status == JournalEntryStati.Booked && !NotaMigrated && StornoBooking == null)
            {
                if (BookNota == null)
                    return new NotaFees(this);
                else
                    throw new ApplicationException(string.Format("ManagementFee {0} already has a nota ({1}).", Key, BookNota.Key));
            }
            return null;
        }

        #endregion

        #region Methods

        public void BookLines()
        {
            this.GeneralOpsJournalEntry.BookLines();
        }

        protected void createComponents(IGLLookupRecords lookups)
        {
            var fees =
                (from u in Units
                from f in u.FeeItems
                group f by new { FeeType = f.FeeType, Period = f.Parent.Period } into g
                let amounts = from pair in g select pair.Amount
                 select new { FeeTypePeriod = g.Key, Amount = amounts.Sum() })
                .Union(
                from u in Units
                from f in u.AverageHoldingFeeItems
                group f by new { FeeType = f.FeeType, Period = f.Parent.Period } into g
                let amounts = from pair in g select pair.Amount
                select new { FeeTypePeriod = g.Key, Amount = amounts.Sum() });

            if (fees != null && fees.Count() > 0)
            {
                IAccountTypeCustomer account = ((IAccountTypeCustomer)Account).ExitFeePayingAccount;
                Money taxeableAmount = null;
                foreach (var feeItem in fees.OrderBy(x => x.FeeTypePeriod.Period))
                {
                    if ((feeItem != null) && ((feeItem.Amount != null) && (feeItem.Amount.IsNotZero)))
                    {
                        IManagementFeeComponent newComponent = new ManagementFeeComponent(this, feeItem.FeeTypePeriod.FeeType.BookingComponentType, feeItem.FeeTypePeriod.Period, feeItem.FeeTypePeriod.FeeType, this.CreationDate);
                        newComponent.AddLinesToComponent(feeItem.Amount, feeItem.FeeTypePeriod.FeeType.BookingComponentType, true, false, false, lookups, account);
                        newComponent.Component.SetDescription(string.Format("{0} {1}", feeItem.FeeTypePeriod.FeeType.Description, feeItem.FeeTypePeriod.Period));
                        this.Components.Add(newComponent);

                        if (feeItem.FeeTypePeriod.FeeType.UseTax)
                            taxeableAmount += feeItem.Amount;
                    }
                }

                if (taxeableAmount != null && taxeableAmount.IsNotZero)
                {
                    if (TaxPercentage < 0 || TaxPercentage > 1)
                        throw new ApplicationException("The BTW Percentage can only be between 0 and 1");
                    Money tax = Money.Multiply(taxeableAmount, TaxPercentage);
                    if (tax != null && tax.IsNotZero)
                    {
                        IManagementFeeComponent taxComponent = new ManagementFeeComponent(this, BookingComponentTypes.Tax, 0, null, this.CreationDate);
                        taxComponent.AddLinesToComponent(tax, BookingComponentTypes.Tax, true, false, false, lookups, account);
                        taxComponent.Component.SetDescription("BTW " + getTaxDescription());
                        this.Components.Add(taxComponent);
                    }
                }
            }
        }

        //private void migrateOldComponents(IGLLookupRecords lookups, B4F.TotalGiro.Orders.OldTransactions.IObsoleteManagementFee oldFee, decimal taxPercentage)
        //{
        //    int period = (this.StartDate.Year * 100) + this.StartDate.Month;
        //    IAccountTypeCustomer account = (IAccountTypeCustomer)oldFee.AccountA;

        //    foreach (MgtFeeBreakupLine line in oldFee.FeeDetails.BreakupLines)
        //    {
        //        if ((line.Amount != null) && (line.Amount.IsNotZero))
        //        {
        //            IManagementFeeComponent newComponent = new ManagementFeeComponent(this, line.MgtFeeType.BookingComponentType, period, line.MgtFeeType, this.CreationDate);
        //            newComponent.AddLinesToComponent(line.Amount, line.MgtFeeType.BookingComponentType, true, false, false, lookups, account);
        //            newComponent.Component.SetDescription(string.Format("{0} {1}", line.MgtFeeType.Description, period));
        //            this.Components.Add(newComponent);
        //        }
        //    }

        //    if ((oldFee.Tax != null) && (oldFee.Tax.IsNotZero))
        //    {
        //        Money tax = oldFee.Tax;
        //        IManagementFeeComponent taxComponent = new ManagementFeeComponent(this, BookingComponentTypes.Tax, 0,  null, this.CreationDate);
        //        taxComponent.AddLinesToComponent(tax, BookingComponentTypes.Tax, true, false, false, lookups, account);
        //        taxComponent.Component.SetDescription(string.Format("BTW " + getTaxDescription()));
        //        this.Components.Add(taxComponent);
        //    }

        //}

        private string getTaxDescription()
        {
            string description = "";
            int year;
            int quarter;
            if (Util.GetQuarterYearByPeriod(Util.GetPeriodFromDate(StartDate), out year, out quarter))
                description = string.Format("Q{0} {1}", quarter, year);
            return description;
        }

        #endregion

        #region Private Variables

        // This is only used to store the ManagementFeeID on the last used AverageHolding
        private IList<IManagementPeriodUnit> units;
        private DateTime startDate;
        private DateTime endDate;
        private DateTime notaStartDate;
        private DateTime notaEndDate;

        #endregion

    }
}
