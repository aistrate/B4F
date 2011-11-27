using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public class MemorialBookingEditView
    {
        public MemorialBookingEditView()
        {
        }

        public MemorialBookingEditView(int journalEntryId)
        {
            this.journalEntryId = journalEntryId;
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

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public decimal ExRate
        {
            get { return exRate; }
            set { exRate = value; }
        }

        private int journalEntryId;
        private DateTime transactionDate;
        private string description;
        private decimal exRate;
    }
}
