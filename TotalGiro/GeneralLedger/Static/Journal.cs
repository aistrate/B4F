using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Communicator.Exact;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class Journal : IJournal
    {
        public Journal() {}

        public Journal(JournalTypes journalType, string journalNumber, IManagementCompany managementCompany, ICurrency currency)
        {
            this.JournalType = journalType;
            this.JournalNumber = journalNumber;
            this.ManagementCompany = managementCompany;
            this.currency = Currency;
        }
        public bool IsAdminAccount { get; set; }
        public bool IsSystem { get; set; }
        
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual JournalTypes JournalType
        {
            get { return journalType; }
            set { journalType = value; }
        }

        public virtual string JournalNumber
        {
            get { return journalNumber; }
            set { journalNumber = value; }
        }

        public virtual string BankAccountNumber
        {
            get { return bankAccountNumber; }
            set { bankAccountNumber = value; }
        }

        public virtual string BankAccountDescription
        {
            get { return bankAccountDescription; }
            set { bankAccountDescription = value; }
        }

        public virtual IGLAccount FixedGLAccount
        {
            get { return fixedGLAccount; }
            set { fixedGLAccount = value; }
        }

        public virtual IManagementCompany ManagementCompany
        {
            get { return managementCompany; }
            set { managementCompany = value; }
        }

        public virtual ICurrency Currency
        {
            get { return this.currency; }
            set { this.currency = value; }
        }

        public virtual IExactJournal ExactJournal { get; set; }
        public virtual bool ShowManualAllowedGLAccountsOnly { get; set; }

        public virtual IBankStatement LastBankStatement
        {
            get { return (bagLastBankStatement != null && bagLastBankStatement.Count == 1 ? (IBankStatement)bagLastBankStatement[0] : null); }
        }

        public virtual Money Balance
        {
            get { return (LastBankStatement != null ? LastBankStatement.ClosingBalance : new Money(0m, this.Currency)); }
        }

        public virtual DateTime DateLastBooked
        {
            get { return (LastBankStatement != null ? LastBankStatement.TransactionDate : DateTime.MinValue); }
        }

        public virtual string FullDescription
        {
            get { return string.Format("{0}  ({1}{2}) {3}",
                            JournalNumber, (BankAccountNumber != null && BankAccountNumber != string.Empty ? BankAccountNumber + " - " : ""), 
                            BankAccountDescription,
                            Currency.AltSymbol); }
        }

        public virtual IList<IJournalEntry> OpenEntries
        {
            get { return entries; }
        }


        private int key;
        private JournalTypes journalType;
        private string journalNumber;
        private string bankAccountNumber;
        private string bankAccountDescription;
        private IGLAccount fixedGLAccount;
        private IManagementCompany managementCompany;
        private ICurrency currency;
        private IList bagLastBankStatement = new ArrayList();
        private IList<IJournalEntry> entries;
    }
}
