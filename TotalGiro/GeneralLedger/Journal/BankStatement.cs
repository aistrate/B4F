using System;
using System.Text;
using System.Collections;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Communicator.Exact;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class BankStatement : JournalEntry, IBankStatement
    {
        public BankStatement() { }

        public BankStatement(IBankStatement lastBankStatement, IJournal journal, string journalEntryNumber, decimal exchangeRate)
            : base(journal, journalEntryNumber)
        {
            if (lastBankStatement != null)
            {
                Journal = lastBankStatement.Journal;

                TransactionDate = getNextWorkingDay(lastBankStatement.TransactionDate);
                if (TransactionDate > DateTime.Today)
                    throw new ApplicationException(
                        string.Format("Journal '{0}' is up-to-date: a Bank Statement for the last working day already exists.", Journal.JournalNumber));

                PrevBankStatement = lastBankStatement;
            }
        }

        public override JournalEntryTypes JournalEntryType
        {
            get { return JournalEntryTypes.BankStatement; }
        }

        public Money OpenAmount
        {
            get { return (HasClosingBalance ? (ClosingBalance - StartingBalance) + Balance : new Money(0m, this.Journal.Currency)); }
        }

        public IBankStatement PrevBankStatement
        {
            get { return prevBankStatement; }
            private set { prevBankStatement = value; }
        }

        public DateTime StartingBalanceDate
        {
            get { return (PrevBankStatement != null ? PrevBankStatement.TransactionDate : DateTime.MinValue); }
        }
        
        public Money StartingBalance
        {
            get { return (PrevBankStatement != null ? PrevBankStatement.ClosingBalance : new Money(0m, this.Journal.Currency)); }
        }

        public virtual Money ClosingBalance
        {
            get { return closingBalance; }
            set { closingBalance = value; }
        }

        public virtual bool HasClosingBalance
        {
            get { return hasClosingBalance; }
            set { hasClosingBalance = value; }
        }

        public virtual IImportedBankBalance ImportedBankBalance
        {
            get { return importedBankBalance; }
            set { importedBankBalance = value; }
        }

        public IJournalEntryLine FixedAccountLine
        {
            get { return Lines.FixedAccountLine; }
        }

        public Money FixedAccountBalance
        {
            get
            {
                IJournalEntryLine fixedAccountLine = FixedAccountLine;
                return (fixedAccountLine != null ? fixedAccountLine.Balance : new Money(0m, this.Journal.Currency));
            }
        }

        public Money TotalBalance
        {
            get { return Balance + FixedAccountBalance; }
        }

        public string DisplayOpenAmount
        {
            get { return (HasClosingBalance && (Status != JournalEntryStati.Booked || OpenAmount.IsNotZero) ? OpenAmount.ToString("#,##0.00") : ""); }
        }

        private DateTime getNextWorkingDay(DateTime date)
        {
            int daysToAdd = 1;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    daysToAdd = 3;
                    break;
                case DayOfWeek.Saturday:
                    daysToAdd = 2;
                    break;
                default:
                    daysToAdd = 1;
                    break;
            }

            return date.AddDays(daysToAdd);
        }

        private IBankStatement prevBankStatement;
        private Money closingBalance;
        private bool hasClosingBalance = false;
        private IImportedBankBalance importedBankBalance;
    }
}
