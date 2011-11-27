using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public class BankStatementEditView
    {
        public BankStatementEditView()
        {
        }

        public BankStatementEditView(int journalEntryId, Money closingBalance, bool canChangeTransactionDate)
            : this(journalEntryId, closingBalance.Quantity, ((ICurrency)closingBalance.Underlying).AltSymbol, canChangeTransactionDate)
        {
        }

        public BankStatementEditView(int journalEntryId, decimal closingBalanceQuantity, string closingBalanceCurrency, bool canChangeTransactionDate)
        {
            this.journalEntryId = journalEntryId;
            this.closingBalanceQuantity = closingBalanceQuantity;
            this.closingBalanceCurrency = closingBalanceCurrency;
            this.canChangeTransactionDate = canChangeTransactionDate;
        }

        public int JournalEntryId
        {
            get { return journalEntryId; }
            set { journalEntryId = value; }
        }

        public DateTime TransactionDate
        {
            get { return transactionDate; }
            set { transactionDate = value; }
        }

        public bool CanChangeTransactionDate
        {
            get { return canChangeTransactionDate; }
            set { canChangeTransactionDate = value; }
        }

        public decimal ClosingBalanceQuantity
        {
            get { return closingBalanceQuantity; }
            set { closingBalanceQuantity = value; }
        }

        public string ClosingBalanceCurrency
        {
            get { return closingBalanceCurrency; }
            set { closingBalanceCurrency = value; }
        }

        private int journalEntryId;
        private DateTime transactionDate;
        private bool canChangeTransactionDate;
        private decimal closingBalanceQuantity;
        private string closingBalanceCurrency;
    }
}
