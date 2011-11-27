using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;
using System.Collections;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class LedgerEntry : ILedgerEntry, IExactFormatterSource
    {
        #region Constructors
        
        public LedgerEntry()
        {
            exactFormatter = new ExactFormatter(this);
            subLedgerEntries = new SubledgerEntryCollection(this);
        }

        public LedgerEntry(ILedgerType ledgerType, string journal, string bookingNumber, DateTime valueDate, Decimal amount, 
                           string kredietbeperking, bool stornoBooking)
            : this()
        {
            this.ledgerType = ledgerType;
            Journal = journal;
            BookingNumber = bookingNumber;
            ValueDate = valueDate;
            //Amount = amount;
            Kredietbeperking = kredietbeperking;
            PaymentType = "B";
            StornoBooking = stornoBooking;
            //this.Debitor = "000000";
        }

        #endregion
        
        #region Methods

        public virtual string FormatLine()
        {
            return exactFormatter.FormatLine();
        }

        public ExactFieldCollection GetFields()
        {
            ExactFieldCollection fields = new ExactFieldCollection();

            // start at index 1
            fields[1] = 0;
            fields[2] = LedgerType.Type.Substring(0, 1);
            fields[3] = Journal;

            fields[6] = BookingNumber;
            fields[7] = Description;
            fields[8] = ValueDate;
            fields[10] = Debitor;
            fields[11] = Creditor;

            fields[13] = Amount;
            //fields[15] = Currency;
            //if (ExRate != 0m)
            //    fields[16] = ExRate;

            fields[17] = Kredietbeperking;
            fields[18] = KredietbeperkingAmount;
            fields[19] = InvoiceDate;
            fields[20] = KredietbeperkingDate;

            fields[24] = PaymentRef;
            fields[25] = PaymentType;

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

        public virtual ILedgerType LedgerType
        {
            get { return ledgerType; }
            private set { ledgerType = value; }
        }

        public virtual string Journal
        {
            get { return journal; }
            set { journal = value; }
        }

        public virtual string BookingNumber
        {
            get { return bookingNumber; }
            set { bookingNumber = value; }
        }

        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        public virtual DateTime ValueDate
        {
            get { return valueDate; }
            set { valueDate = value; }
        }

        public virtual string Debitor
        {
            get { return debitor; }
            set { debitor = value; }
        }

        public virtual string Creditor
        {
            get { return creditor; }
            set { creditor = value; }
        }

        public virtual Decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        //public virtual string Currency
        //{
        //    get { return currency; }
        //    set { currency = value; }
        //}
        
        //public virtual decimal ExRate
        //{
        //    get { return exRate; }
        //    set { exRate = value; }
        //}
        
        public virtual string Kredietbeperking
        {
            get { return kredietbeperking; }
            set { kredietbeperking = value; }
        }

        public virtual decimal KredietbeperkingAmount
        {
            get { return kredietbeperkingAmount; }
            set { kredietbeperkingAmount = value; }
        }

        public virtual DateTime InvoiceDate
        {
            get { return invoiceDate; }
            set { invoiceDate = value; }
        }

        public virtual DateTime KredietbeperkingDate
        {
            get { return kredietbeperkingDate; }
            set { kredietbeperkingDate = value; }
        }

        public virtual string PaymentRef
        {
            get { return paymentRef; }
            set { paymentRef = value; }
        }

        public virtual string PaymentType
        {
            get { return paymentType; }
            set { paymentType = value; }
        }

        public virtual bool StornoBooking
        {
            get { return stornoBooking; }
            set { stornoBooking = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
            private set { creationDate = value; }
        }

        public virtual IExportedLedgerFile ExportedLedgerFile
        {
            get { return exportedLedgerFile; }
            set { exportedLedgerFile = value; }
        }


        public virtual ISubledgerEntryCollection SubledgerEntries
        {
            get
            {
                ISubledgerEntryCollection sub = (ISubledgerEntryCollection)subLedgerEntries.AsList();
                if (sub.Parent == null) sub.Parent = this;
                return sub;
            }
        }



        #endregion

        #region Private Variables

        private bool stornoBooking;
        private DateTime creationDate;
        private DateTime invoiceDate;
        private DateTime kredietbeperkingDate;
        private DateTime valueDate;
        private decimal amount;
        private decimal kredietbeperkingAmount;
        private ExactFormatter exactFormatter;
        private IDomainCollection<ISubledgerEntry> subLedgerEntries;
        private IExportedLedgerFile exportedLedgerFile;
        private ILedgerType ledgerType;
        private int key;
        private string bookingNumber;
        private string creditor;
        private string currency;
        private string debitor;
        private string description;
        private string journal;
        private string kredietbeperking;
        private string paymentRef;
        private string paymentType;

        
        #endregion


    }
}
