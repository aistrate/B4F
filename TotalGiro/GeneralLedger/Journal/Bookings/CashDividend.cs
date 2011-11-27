using System;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class CashDividend : GeneralOperationsBookingTaxeable, ICashDividend
    {
        #region Constructor

        protected CashDividend() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dividendDetails">The details of the cash dividend (date, price)</param>
        /// <param name="units">The total number of units over which dividend is paid</param>
        public CashDividend(IAccountTypeCustomer account, IMemorialBooking journalEntry, string description, decimal taxPercentage, IDividendHistory dividendDetails, InstrumentSize units, IGLLookupRecords lookups)
            : base(account, journalEntry, description, taxPercentage)
        {
            if (dividendDetails == null)
                throw new ApplicationException("Dividend details are mandatory.");

            if (units == null || units.IsZero)
                throw new ApplicationException("The number of units is mandatory.");

            this.DividendDetails = dividendDetails;
            this.UnitsInPossession = units;
            //this.CashGeneratingInstrument = dividendDetails.Instrument;

            createComponents(lookups);
            
        }

        #endregion

        #region Props

        /// <summary>
        /// The gross dividend amount
        /// </summary>
        public virtual Money DividendAmount
        {
            get { return Components.ReturnComponentValue(BookingComponentTypes.Dividend); }
        }

        public override Money NettAmount
        {
            get
            {
                return this.Components.Where(x => x.BookingComponentType != BookingComponentTypes.DividendTax).Select(x => x.ComponentValue).Sum();
            }
        }

        public override Money TaxAmount
        {
            get
            {
                Money divtax = Components.ReturnComponentValue(BookingComponentTypes.DividendTax);
                if (divtax == null)
                {
                    if (NettAmount != null)
                        divtax = new Money(0m, (ICurrency)NettAmount.Underlying);
                    else
                        divtax = new Money(0m, this.GeneralOpsJournalEntry.Journal.Currency);
                }
                return divtax;
            }
        }

        /// <summary>
        /// The details of the cash dividend (date & price)
        /// </summary>
        public virtual IDividendHistory DividendDetails
        {
            get { return dividendDetails; }
            set { dividendDetails = value; }
        }

        /// <summary>
        /// The date that the instrument went ex dividend
        /// </summary>
        public virtual DateTime ExDividendDate
        {
            get { return this.dividendDetails.ExDividendDate; }
        }

        /// <summary>
        /// The date on which the dividend is actually paid
        /// </summary>
        public virtual DateTime SettlementDate
        {
            get { return this.dividendDetails.SettlementDate; }
        }

        /// <summary>
        /// The price of dividend per unit
        /// </summary>
        public virtual Price UnitPrice
        {
            get { return this.dividendDetails.UnitPrice; }
        }



        /// <summary>
        /// The number of units the dividend was paid on
        /// </summary>
        public virtual InstrumentSize UnitsInPossession
        {
            get { return this.unitsInPossession; }
            set { this.unitsInPossession = value; }
        }

        #endregion

        #region Overrides

        public override GeneralOperationsBookingTypes BookingType { get { return GeneralOperationsBookingTypes.CashDividend; } }

        public override IGeneralOperationsBooking Storno(IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry)
        {
            CashDividend newStorno = new CashDividend();
            newStorno.DividendDetails = this.DividendDetails;
            newStorno.UnitsInPossession = this.UnitsInPossession;
            return this.storno(employee, reason, journalEntry, newStorno);
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
                    return new NotaDividend(this);
                else
                    throw new ApplicationException(string.Format("ManagementFee {0} already has a nota ({1}).", Key, BookNota.Key));
            }
            return null;
        }

        #endregion

        #region Methods

        protected void createComponents(IGLLookupRecords lookups)
        {
            Money div = null;
            if (UnitsInPossession != null && UnitsInPossession.Underlying.IsCorporateAction)
                div = UnitsInPossession.CalculateAmount(this.DividendDetails.StockDivUnitPrice);
            else
                div = UnitsInPossession.CalculateAmount(this.DividendDetails.UnitPrice);

            if (div != null && div.IsNotZero)
            {
                Money tax = div * (TaxPercentage * -1);
                ICashDividendComponent newComponent = new CashDividendComponent(this, BookingComponentTypes.Dividend, this.CreationDate);
                newComponent.AddLinesToComponent(div, BookingComponentTypes.Dividend, true, false, false, lookups, Account);
                newComponent.Component.SetDescription(this.DividendDetails.DisplayString);
                this.Components.Add(newComponent);

                if (tax != null && tax.IsNotZero)
                {
                    ICashDividendComponent taxComponent = new CashDividendComponent(this, BookingComponentTypes.DividendTax, this.CreationDate);
                    taxComponent.AddLinesToComponent(tax, BookingComponentTypes.DividendTax, true, false, false, lookups, Account);
                    taxComponent.Component.SetDescription("tax " + this.DividendDetails.DisplayString);
                    this.Components.Add(taxComponent);
                }
            }
        }

        public void Execute()
        {
            this.GeneralOpsJournalEntry.BookLines();
        }

        #endregion

        #region Private Variables

        private IDividendHistory dividendDetails;
        private InstrumentSize unitsInPossession;

        #endregion
    }
}
