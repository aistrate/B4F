using System;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Valuations.Mapping;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationCashMutation : IValuationCashMutation
    {
        #region Constructors

        protected ValuationCashMutation() { }

        /// <summary>
        /// Creates the first ValuationCashMutation for a n instrument
        /// </summary>
        /// <param name="line">The first JournalEntryLine that creates this Valuation Mutation</param>
        internal ValuationCashMutation(IJournalEntryLine line) 
        {
            this.Account = line.GiroAccount;
            this.Instrument = line.BookingRelatedInstrument;
            this.ValuationCashType = line.GLAccount.ValuationCashType;
            this.Date = line.BookDate;
        }

        /// <summary>
        /// Creates a ValuationCashMutation from a previous ValuationCashMutation (with older date)
        /// </summary>
        /// <param name="mutationDate">The date for the new valuation mutation</param>
        /// <param name="prevCashMutation">The previous mutation</param>
        internal ValuationCashMutation(DateTime mutationDate, IValuationCashMutation prevCashMutation)
        {
            if (prevCashMutation != null)
            {
                this.Date = mutationDate;
                this.PreviousCashMutation = prevCashMutation;
                this.ValuationCashType = prevCashMutation.ValuationCashType;
                this.Account = prevCashMutation.Account;
                this.Instrument = prevCashMutation.Instrument;
                this.AmountToDate = prevCashMutation.AmountToDate;
                this.BaseAmountToDate = prevCashMutation.BaseAmountToDate;
            }
            else
                throw new ApplicationException("Previous cash valuation can not be null");
        }

        #endregion

        #region Methods

        public void AddLine(IJournalEntryLine line)
        {
            if (!Account.Equals(line.ParentSubPosition.ParentPosition.Account))
                throw new ApplicationException("It is not possible to create Valuations when multiple positions exists for one instrument.");

            if (Instrument != null && line.BookingRelatedInstrument != null)
            {
                if (!Instrument.Equals(line.BookingRelatedInstrument))
                    throw new ApplicationException("The cash generating instrument does not equal the cash generating instrument on the cash valuation mutation.");
            }
            else if ((Instrument != null && line.BookingRelatedInstrument == null) || (Instrument == null && line.BookingRelatedInstrument != null))
                throw new ApplicationException("The cash generating instrument does not equal the cash generating instrument on the cash valuation mutation.");

            if (!ValuationCashType.Equals(line.GLAccount.ValuationCashType))
                throw new ApplicationException("The cash type does not equal the cash type on the cash valuation mutation.");

            if (line.GLAccount.ValuationCashTypeDetails.IsSettled != line.ParentSubPosition.IsSettled)
                throw new ApplicationException("The settled flag of the cash type does not equal the settled flag on journal entry line.");

            Money amount;
            if (Instrument != null)
                amount = line.Balance.Negate();
            else
                amount = line.BaseBalance.Negate();
            Money baseAmount = line.BaseBalance.Negate();

            Amount += amount;
            amountToDate += amount;
            BaseAmount += baseAmount;
            baseAmountToDate += baseAmount;

            Mappings.Add(new JournalEntryLineValuationCashMapping(line, this));
        }

        public void AddNotRelevantLine(IJournalEntryLine notRelevantLine)
        {
            Mappings.Add(new JournalEntryLineValuationCashMapping(notRelevantLine, this));
        }

        public bool ContainsLine(IJournalEntryLine line)
        {
            if (Mappings.Count > 0)
            {
                foreach (JournalEntryLineValuationCashMapping mapping in Mappings)
                {
                    if (mapping.Key.Equals(line.Key))
                        return true;
                }
            }
            return false;
        }

        public virtual bool Validate()
        {
            if (PreviousCashMutation == null)
            {
                if (Amount != null && Amount.IsNotZero)
                    this.IsValid = true;
            }
            else
                this.IsValid = true;
            return this.IsValid;

        }

        #endregion

        #region ValuationCash Props

        public virtual long Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual IValuationCashMutation PreviousCashMutation
        {
            get { return prevCashMutation; }
            internal set { prevCashMutation = value; }
        }

        public virtual IAccountTypeInternal Account
        {
            get { return account; }
            internal set { account = value; }
        }

        public virtual IInstrument Instrument
        {
            get { return instrument; }
            internal set { instrument = value; }
        }

        public virtual bool IsValid
        {
            get { return isValid; }
            internal set { isValid = value; }
        }

        public virtual DateTime Date
        {
            get { return mutationDate; }
            internal set { mutationDate = value; }
        }

        public virtual ValuationCashTypes ValuationCashType
        {
            get { return valuationCashType; }
            internal set { valuationCashType = value; }
        }

        public virtual Money Amount
        {
            get { return this.amount; }
            internal set { this.amount = value; }
        }

        public virtual Money BaseAmount
        {
            get { return this.baseAmount; }
            internal set { this.baseAmount = value; }
        }

        public virtual Money AmountToDate
        {
            get { return this.amountToDate; }
            internal set { this.amountToDate = value; }
        }

        public virtual Money BaseAmountToDate
        {
            get { return this.baseAmountToDate; }
            internal set { this.baseAmountToDate = value; }
        }
	
        internal GenericCollection<JournalEntryLineValuationCashMapping> Mappings
        {
            get
            {
                if (mappings == null)
                    mappings = new GenericCollection<JournalEntryLineValuationCashMapping>(bagOfMappings);
                return mappings;
            }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        public virtual string GetUniqueCode
        {
            get { return string.Format("{0}.{1}.{2}.{3}", Account.Key.ToString(), (Instrument == null ? "0" : Instrument.Key.ToString()), ((int)ValuationCashType).ToString(), Date.ToString("yyyy-MM-dd")); }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (Account != null && Util.IsNotNullDate(Date))
            {
                if (Instrument != null)
                    return string.Format("{0} {1} {2} {3}", Instrument.DisplayName, ValuationCashType.ToString(), Date.ToShortDateString(), Account.DisplayNumberWithName);
                else
                    return string.Format("{0} {1} {2}", ValuationCashType.ToString(), Date.ToShortDateString(), Account.DisplayNumberWithName);
            }
            else
                return base.ToString();
        }

        #endregion

        #region Privates

        private long key;
        private IValuationCashMutation prevCashMutation;
        private IAccountTypeInternal account;
        private IInstrument instrument;
        private DateTime mutationDate = DateTime.MinValue;
        private ValuationCashTypes valuationCashType;
        private bool isValid;
        private Money amount;
        private Money amountToDate;
        private Money baseAmount;
        private Money baseAmountToDate;
        private DateTime creationDate = DateTime.Now;
        private IList bagOfMappings = new ArrayList();
        private GenericCollection<JournalEntryLineValuationCashMapping> mappings;

        #endregion

    }
}
