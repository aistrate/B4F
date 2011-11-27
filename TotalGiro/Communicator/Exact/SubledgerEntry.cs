using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using System.Collections;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class SubledgerEntry : ISubledgerEntry, IExactFormatterSource
    {
        #region Constructors

        public SubledgerEntry()
        {
            exactFormatter = new ExactFormatter(this);
            journalEntryLines = new JournalEntryLineCollection(this);
        }

        public SubledgerEntry(string description, string generalLedgerAccount, Decimal amount, string currency, bool stornoBooking, decimal exRate)
            : this()
        {
            Description = description;
            GeneralLedgerAccount = generalLedgerAccount;
            Amount = amount;
            Currency = currency;
            StornoBooking = stornoBooking;
            this.exRate = exRate;
            this.Debitor = "000000";
        }

        #endregion
        
        #region Methods

        public virtual string FormatLine()
        {
            return exactFormatter.FormatLine();
        }

        public virtual ExactFieldCollection GetFields()
        {
            ExactFieldCollection fields = new ExactFieldCollection();

            // start at index 1
            fields[1] = LineNumber;
            fields[2] = Parent.LedgerType.Type.Substring(0, 1);
            fields[3] = Parent.Journal;

            fields[7] = Description;
            fields[8] = Parent.ValueDate;
            fields[9] = GeneralLedgerAccount;
            fields[10] = Debitor;

            fields[13] = (IsBankMutation || IsSalesMutation) ? Amount * -1 : Amount;
            fields[15] = Currency;
            if (ExRateforExact != 0m)
                fields[16] = ExRateforExact;

            fields[21] = BtwCode;
            if ((BtwCode != null && BtwCode != string.Empty) || BtwAmount != 0m)
                fields[22] = BtwAmount;

            fields[24] = Parent.PaymentRef;

            fields[28] = KostendragerCode;

            fields[32] = (StornoBooking ? "Y" : "N");
            
            return fields;
        }

        #endregion
        
        #region Properties

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual ILedgerEntry Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }

        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        public virtual string GeneralLedgerAccount
        {
            get { return generalLedgerAccount; }
            set { generalLedgerAccount = value; }
        }

        public virtual string Debitor
        {
            get { return debitor; }
            set { debitor = value; }
        }

        public virtual Decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public virtual string Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public virtual decimal ExRateforExact
        {
            get { return (exRate == 0 ? 1 : decimal.Divide(1, exRate)); }
            set { exRate = value; }
        }

        public virtual string BtwCode
        {
            get { return btwCode; }
            set { btwCode = value; }
        }

        public virtual Decimal BtwAmount
        {
            get { return btwAmount; }
            set { btwAmount = value; }
        }

        public virtual string KostendragerCode
        {
            get { return kostendragerCode; }
            set { kostendragerCode = value; }
        }

        public virtual bool StornoBooking
        {
            get { return stornoBooking; }
            set { stornoBooking = value; }
        }

        public virtual bool IsBankMutation { get { return this.Parent.LedgerType.TypeOfLedger == LedgerTypes.Mutations; } }
        public virtual bool IsSalesMutation { get { return this.Parent.LedgerType.TypeOfLedger == LedgerTypes.ClientTransactions; } }

        public virtual IJournalEntryLineCollection JournalEntryLines
        {
            get
            {
                IJournalEntryLineCollection lines = (IJournalEntryLineCollection)journalEntryLines.AsList();
                if (lines.Parent == null) lines.Parent = this;
                return lines;
            }
        }

        #endregion

        #region Private Variables

        private ExactFormatter exactFormatter;
        private int key;
        private ILedgerEntry parent;
        private int lineNumber;
        private string description;
        private string generalLedgerAccount;
        private string debitor;
        private Decimal amount;
        private string currency;
        private decimal exRate;
        private string btwCode;
        private Decimal btwAmount;
        private string kostendragerCode;
        private bool stornoBooking;
        private IDomainCollection<IJournalEntryLine> journalEntryLines;

        #endregion
    }
}
