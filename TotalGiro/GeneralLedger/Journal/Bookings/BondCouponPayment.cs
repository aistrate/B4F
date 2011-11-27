using System;
using System.Linq;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments.CorporateAction;
using System.Collections.Generic;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class BondCouponPayment : GeneralOperationsBooking, IBondCouponPayment
    {
        #region Constructor

        protected BondCouponPayment() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dividendDetails">The details of the cash dividend (date, price)</param>
        /// <param name="units">The total number of units over which dividend is paid</param>
        public BondCouponPayment(IFundPosition position,
            ICouponHistory couponHistory,
            IMemorialBooking journalEntry)
            : base(position.Account, journalEntry, "")
        {
            if (position == null)
                throw new ApplicationException("The position is mandatory.");

            if (couponHistory == null)
                throw new ApplicationException("The coupon history item is mandatory.");

            if (position.Instrument.SecCategory.Key != SecCategories.Bond)
                throw new ApplicationException("The Bond is mandatory.");

            this.Position = position;
            this.CouponHistory = couponHistory;
            this.Description = couponHistory.Description;
            this.Status = BondCouponPaymentStati.Active;
            // Only print the nota after the Booking Settles
            this.IsNotarizable = false;

            IBondCouponPaymentComponent newComponent = new BondCouponPaymentComponent(this, BookingComponentTypes.AccruedInterest, this.CreationDate);
            this.Components.Add(newComponent);
        }

        #endregion

        #region Props

        /// <summary>
        /// The bond where interest is paid for
        /// </summary>
        public virtual IFundPosition Position { get; set; }
        public virtual ICouponHistory CouponHistory { get; set; }
        public virtual BondCouponPaymentStati Status { get; set; }

        public virtual IBond Bond 
        {
            get { return (IBond)Position.Instrument; }
        }

        public virtual IBondCouponPaymentDailyCalculationCollection DailyCalculations
        {
            get
            {
                IBondCouponPaymentDailyCalculationCollection calculations = (IBondCouponPaymentDailyCalculationCollection)dailyCalculations.AsList();
                if (calculations.Parent == null) calculations.Parent = this;
                return calculations;
            }
        }

        public virtual bool HasSettled 
        {
            get 
            {
                bool hasSettled = false;
                IJournalEntryLine mainLine = this.Components[0].MainLine;
                if (mainLine != null && mainLine.GLAccount.IsSettledWithClient)
                    hasSettled = true;
                return hasSettled; 
            }
        }

        #endregion

        #region Overrides

        public override GeneralOperationsBookingTypes BookingType { get { return GeneralOperationsBookingTypes.BondCouponPayment; } }

        public override IGeneralOperationsBooking Storno(IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry)
        {
            if (Status != BondCouponPaymentStati.Settled)
                throw new ApplicationException("It is not possible to storno a bond interest payements that has not a status of paid.");
            
            if (Position.BondCouponPayments.Where(x => 
                x.CouponHistory.StartAccrualDate > this.CouponHistory.EndAccrualDate &&
                ((x.Status == BondCouponPaymentStati.Settled &&
                !(x.StornoBooking != null || x.IsStorno)) ||
                x.Status == BondCouponPaymentStati.Active)).Count() > 0)
                throw new ApplicationException(string.Format("Bond interest payements exists after {0} that should be stornoed first.", CouponHistory.EndAccrualDate.ToShortDateString()));
            
            BondCouponPayment newStorno = new BondCouponPayment();
            newStorno.Position = this.Position;
            newStorno.CouponHistory = this.CouponHistory;
            newStorno.Status = this.Status;
            newStorno.IsNotarizable = this.IsNotarizable;

            DateTime newLastDate = CouponHistory.StartAccrualDate.AddDays(-1);
            if (newLastDate < Position.OpenDate) newLastDate = Position.OpenDate;
            Position.LastBondCouponCalcDate = newLastDate;

            return this.storno(employee, reason, journalEntry, newStorno);
        }

        /// <summary>
        /// This method creates the nota of the ManagementFee
        /// </summary>
        /// <returns>True when successfull</returns>
        public override INota CreateNota()
        {
            //if (this.GeneralOpsJournalEntry.Status == JournalEntryStati.Booked && !NotaMigrated && StornoBooking == null)
            //{
            //    if (BookNota == null)
            //        return new NotaDividend(this);
            //    else
            //        throw new ApplicationException(string.Format("ManagementFee {0} already has a nota ({1}).", Key, BookNota.Key));
            //}
            return null;
        }

        public override Money TotalAmount
        {
            get
            {
                if (Status == BondCouponPaymentStati.Settled)
                    return base.TotalAmount;
                else
                    return DailyCalculations.Get(e => e.LastCalculation).Get(e => e.CalculatedAccruedInterestUpToDate);
            }
        }

        public virtual Money TotalAmountUnSettled
        {
            get
            {
                Money unSettledAmount = null;
                IBondCouponPaymentComponent component = this.Components.FirstOrDefault() as IBondCouponPaymentComponent;
                if (component != null)
                    unSettledAmount = component.Component.JournalLines
                        .Where(x => !x.GLAccount.IsSettledWithClient && x.GiroAccount != null)
                        .Select(x => x.Balance)
                        .Sum();
                return unSettledAmount;
            }
        }

        public override string ToString()
        {
            if (Position != null && CouponHistory != null)
                return CouponHistory.DisplayString;
            else
                return base.ToString();
        }

        #endregion

        #region Methods

        public void CalculateDailyInterest(InstrumentSize size, DateTime calcDate, DateTime settlementDate,
            IList<IBondCouponPaymentDailyCalculation> oldCalculations, IGLLookupRecords lookups)
        {
            if (!HasSettled)
            {
                AccruedInterestDetails details = Bond.AccruedInterest(size, settlementDate, Bond.DefaultExchange ?? Bond.HomeExchange);
                if (details.IsRelevant || (oldCalculations != null && oldCalculations.Count > 0))
                {
                    this.DailyCalculations.AddCalculation(calcDate, settlementDate, size, details.AccruedInterest, oldCalculations);
                    Money dailyInterest = DailyCalculations.GetCalculationByDate(settlementDate).DailyInterest;

                    IBondCouponPaymentComponent component = (IBondCouponPaymentComponent)this.Components[0];
                    Money lineAmountAlreadyCharged = null;
                    if (component.JournalLines.Any(x => x.ValueDate.Equals(calcDate) && x.GiroAccount != null && x.GiroAccount.Key == this.Account.Key))
                        lineAmountAlreadyCharged = component.JournalLines.Where(x => x.ValueDate.Equals(calcDate) && x.GiroAccount != null && x.GiroAccount.Key == this.Account.Key).Select(x => x.Balance).Sum();

                    if ((dailyInterest != null && dailyInterest.IsNotZero) || (lineAmountAlreadyCharged != null && lineAmountAlreadyCharged.IsNotZero))
                    {
                        Money lineAmount = dailyInterest + lineAmountAlreadyCharged;
                        if (lineAmount != null && lineAmount.IsNotZero)
                        {
                            component.AddLinesToComponent(lineAmount, BookingComponentTypes.AccruedInterest, false, false, false, lookups, Account, null, calcDate);
                            foreach (IJournalEntryLine line in component.Component.JournalLines.Where(x => x.ValueDate.Equals(calcDate) && string.IsNullOrEmpty(x.Description)))
                                line.Description = string.Format("Interest {0}", settlementDate.ToString("dd-MM-yyyy"));

                            this.GeneralOpsJournalEntry.BookLines();
                        }
                    }
                }
            }
            else
                throw new ApplicationException("This interest payment already settled.");
        }

        
        /// <summary>
        /// This method is used to deactivate this payment.
        /// The interest is not paid yet, but the client owns it.
        /// </summary>
        /// <param name="settlementDate"></param>
        public void SetToBeSettled(DateTime calcDate, DateTime settlementDate)
        {
            if ((int)Status > 0)
                throw new ApplicationException("This interest payment already is set to 'to be settled'.");

            const string DefErr = "Can not set the status to to-be-settled  of the accrued interest. ";

            if (this.DailyCalculations == null || this.DailyCalculations.Count == 0)
                throw new ApplicationException(DefErr + "No calculations.");

            if (this.CouponHistory.EndAccrualDate != this.DailyCalculations.LastCalculation.SettlementDate)
                throw new ApplicationException(DefErr + "Calculations do not match the accrual end date.");

            InstrumentSize size = Position.PositionTransactions.Where(x => x.TransactionDate <= calcDate).Select(x => x.Size).Sum();
            AccruedInterestDetails details = Bond.AccruedInterest(size, settlementDate, Bond.DefaultExchange ?? Bond.HomeExchange);
            if (!details.IsRelevant)
                throw new ApplicationException(DefErr + "Accrued interest could not be calculated.");

            if (details.AccruedInterest != this.DailyCalculations.LastCalculation.CalculatedAccruedInterestUpToDate ||
                details.AccruedInterest != this.DailyCalculations.TotalAccruedInterest)
                throw new ApplicationException(DefErr + "Accrued interest does not equal the unsettled interest.");

            this.Status = BondCouponPaymentStati.ToBeSettled;
        }

        public void SettleInterest(DateTime calcDate)
        {
            if (HasSettled)
                throw new ApplicationException("This interest payment already settled.");

            const string DefErr = "Can not settle the accrued interest. ";

            if (this.Status != BondCouponPaymentStati.ToBeSettled)
                throw new ApplicationException(DefErr + "The status does not equal to-be-settled.");

            if (this.DailyCalculations == null || this.DailyCalculations.Count == 0)
                throw new ApplicationException(DefErr + "No calculations.");
            
            if (this.CouponHistory.EndAccrualDate > calcDate)
                throw new ApplicationException(DefErr + "The date to sttle the interest is in the future.");

            Money accruedInterest = this.DailyCalculations.LastCalculation.CalculatedAccruedInterestUpToDate;
            IGLAccount glAccount = Components[0].MainLine.GLAccount;
            JournalEntryLine newLine1 = new JournalEntryLine();
            newLine1.BookDate = CouponHistory.EndAccrualDate;
            newLine1.clientSettle(GeneralOpsJournalEntry, newLine1, glAccount, accruedInterest, this.Components[0].Component, this.Account);
            newLine1.Description = "Settle Interest " + newLine1.BookDate.ToString("dd-MM-yyyy");

            JournalEntryLine newLine2 = new JournalEntryLine();
            newLine2.BookDate = CouponHistory.EndAccrualDate;
            newLine2.clientSettle(GeneralOpsJournalEntry, newLine2, glAccount.GLSettledAccount, accruedInterest.Negate(), this.Components[0].Component, this.Account);
            newLine2.Description = "Settle Interest " + newLine2.BookDate.ToString("dd-MM-yyyy");
            Components[0].MainLine = newLine2;

            foreach (IJournalEntryLine line in Components[0].JournalLines)
            {
                if (!line.GLAccount.IsSettledWithClient)
                    line.IsSettledStatus = true;
            }
            this.Status = BondCouponPaymentStati.Settled;
            this.IsNotarizable = true;
        }

        public bool Cancel()
        {
            return Cancel(false);
        }

        public bool Cancel(bool ignoreSizeNotZero)
        {
            const string defErr = "It is not possible to Cancel an inactive BondCouponPayment, use storno instead. ";
            if (!(this.Status == BondCouponPaymentStati.Active))
                throw new ApplicationException(defErr + "The payment is not active");

            if (HasSettled)
                throw new ApplicationException(defErr + "This interest payment already settled");

            if (!ignoreSizeNotZero)
            {
                InstrumentSize size = Position.PositionTransactions.Where(x => x.TransactionDate <= CouponHistory.EndAccrualDate).Select(x => x.Size).Sum();
                if (size.IsNotZero)
                    throw new ApplicationException(defErr + "The position size is not equal to zero");
            }

            Money cancelAmount = this.DailyCalculations.TotalAccruedInterest;
            if (cancelAmount.IsNotZero)
            {
                // Find day where Pos Size got zero
                DateTime posClosingDate = CouponHistory.EndAccrualDate;
                while (Position.PositionTransactions.Any(x => x.TransactionDate <= posClosingDate) && Position.PositionTransactions.Where(x => x.TransactionDate <= posClosingDate).Select(x => x.Size).Sum().IsZero)
                {
                    posClosingDate = posClosingDate.AddDays(-1);
                    if (posClosingDate < CouponHistory.StartAccrualDate)
                        throw new ApplicationException(defErr + "The position closing date could not be determined");
                }

                foreach (IJournalEntryLine line in this.GeneralOpsJournalEntry.Lines)
                {
                    JournalEntryLine newLine = new JournalEntryLine();
                    newLine.clientSettle(
                        GeneralOpsJournalEntry,
                        newLine,
                        line.GLAccount,
                        line.Balance.Negate(),
                        this.Components[0].Component,
                        line.GiroAccount);
                    newLine.ValueDate = line.ValueDate;
                    newLine.Description = string.Format("Cancellation {0}", line.ValueDate.ToString("dd-MM-yyyy"));
                }

                this.GeneralOpsJournalEntry.BookLines();
                this.Position.LastBondCouponCalcDate = posClosingDate;
            }
            this.Status = BondCouponPaymentStati.Cancelled;

            return !(this.Status == BondCouponPaymentStati.Active);
        }

        #endregion

        #region Private Variables

        private IDomainCollection<IBondCouponPaymentDailyCalculation> dailyCalculations = new BondCouponPaymentDailyCalculationCollection();

        #endregion
    }
}
