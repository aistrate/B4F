using System;
using System.Linq;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public abstract class JournalEntry : TotalGiroBase<IJournalEntry>, IJournalEntry
    {
        protected JournalEntry() 
        { 
            lines = new JournalEntryLineCollection(this);
            this.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
        }

        public JournalEntry(IJournal journal, string journalEntryNumber)
            : this()
        {
            Status = JournalEntryStati.New;
            Journal = journal;
            this.JournalEntryNumber = journalEntryNumber;
        }

        public abstract JournalEntryTypes JournalEntryType { get; }

        public virtual string JournalEntryNumber
        {
            get { return journalEntryNumber; }
            protected set { journalEntryNumber = value; }
        }

        public virtual JournalEntryStati Status
        {
            get { return status; }
            set { status = value; }
        }

        public virtual IJournal Journal
        {
            get { return journal; }
            protected set { journal = value; }
        }

        public virtual DateTime TransactionDate
        {
            get { return transactionDate; }
            set { transactionDate = value; }
        }

        public virtual Money Debit
        {
            get
            {
                Money debits = new Money(0m, this.Journal.Currency);
                foreach (IJournalEntryLine line in Lines)
                    if (!line.GLAccountIsFixed && line.Debit.Underlying.Key == this.Journal.Currency.Key)
                        debits += line.Debit;
                return debits;
            }
        }

        public virtual Money Credit
        {
            get
            {
                Money credits = new Money(0m, this.Journal.Currency);
                foreach (IJournalEntryLine line in Lines)
                    if (!line.GLAccountIsFixed && line.Credit.Underlying.Key == this.Journal.Currency.Key)
                        credits += line.Credit;
                return credits;
            }
        }

        public virtual Money Balance
        {
            get { return Debit - Credit; }
        }

        public virtual bool ContainsForeignCashLines 
        {
            get { return Lines.Where(u => !u.Currency.IsBase).Count() > 0; }
        }

        public virtual decimal ExchangeRate 
        {
            get { return Balance.XRate; }
        }

        public virtual string DisplayStatus
        {
            get { return Status.ToString(); }
        }

        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
        }

        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        public virtual string CreatedBy { get; set; }
        public virtual string BookedBy { get; set; }
        public IGLBookYear BookYear { get; set; }

        public void BookLines()
        {
            foreach (IJournalEntryLine line in Lines.Where(x => x.Status == JournalEntryLineStati.New))
                    line.BookLine();
            this.Status = JournalEntryStati.Booked;
            this.BookedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
        }

        public virtual IJournalEntryLineCollection Lines
        {
            get
            {
                IJournalEntryLineCollection pos = (IJournalEntryLineCollection)lines.AsList();
                if (pos.Parent == null) pos.Parent = this;
                return pos;
            }
        }

        private string journalEntryNumber;
        private DateTime lastUpdated = DateTime.MinValue;
        private JournalEntryStati status;
        private IJournal journal;
        private DateTime transactionDate;
        private IDomainCollection<IJournalEntryLine> lines;
        private DateTime creationDate = DateTime.Now;
    }
}
