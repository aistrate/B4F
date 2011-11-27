using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public class JournalEntryLineEditView
    {
        public JournalEntryLineEditView()
        {
        }

        public int Key
        {
            get { return journalEntryLineId; }
            set { journalEntryLineId = value; }
        }

        public int JournalEntryLineId
        {
            get { return journalEntryLineId; }
            set { journalEntryLineId = value; }
        }

        public int JournalEntryId
        {
            get { return journalEntryId; }
            set { journalEntryId = value; }
        }

        public int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }

        public JournalEntryLineStati Status
        {
            get { return status; }
            set { status = value; }
        }

        public int GLAccountId
        {
            get { return glAccountId; }
            set { glAccountId = value; }
        }

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public decimal ExchangeRate
        {
            get { return exRate; }
            set { exRate = value; }
        }

        public decimal DebitQuantity
        {
            get { return debitQuantity; }
            set { debitQuantity = value; }
        }

        public string DebitDisplayString
        {
            get { return debitDisplayString; }
            set { debitDisplayString = value; }
        }

        public decimal CreditQuantity
        {
            get { return creditQuantity; }
            set { creditQuantity = value; }
        }

        public string CreditDisplayString
        {
            get { return creditDisplayString; }
            set { creditDisplayString = value; }
        }

        public string GiroAccountNumber
        {
            get { return giroAccountNumber; }
            set { giroAccountNumber = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int StornoedLineId
        {
            get { return stornoedLineId; }
            set { stornoedLineId = value; }
        }

        private int journalEntryLineId;
        private int journalEntryId;
        private int lineNumber;
        private JournalEntryLineStati status;
        private int glAccountId;
        private int currencyId = 0;
        private decimal exRate;
        private decimal debitQuantity;
        private string debitDisplayString;
        private decimal creditQuantity;
        private string creditDisplayString;
        private string giroAccountNumber;
        private string description;
        private int stornoedLineId = 0;
    }
}
