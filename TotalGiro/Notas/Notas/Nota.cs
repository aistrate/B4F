using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Reports.Documents;

namespace B4F.TotalGiro.Notas
{
    public abstract class Nota : INota
    {
        #region Constructor

        protected Nota() { }

        protected Nota(ICustomerAccount account)
        {
            this.creationDate = DateTime.Now;
            this.printCount = 0;
            
            this.formatter = ContactsFormatter.CreateContactsFormatter(account);
            this.contactsNAW = this.formatter.ContactsNAW;
            this.secondContactsNAW = this.formatter.SecondContactsNAW;
        }

        #endregion

        #region Props

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual string NotaNumber
        {
            get { return notaNumber; }
            private set { notaNumber = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
            private set { creationDate = value; }
        }

        public virtual INotaDocument Document { get; set; }

        public virtual IContactsNAW ContactsNAW
        {
            get { return contactsNAW; }
            private set { contactsNAW = value; }
        }

        public virtual IContactsNAW SecondContactsNAW
        {
            get { return secondContactsNAW; }
            private set { secondContactsNAW = value; }
        }

        public virtual int PrintCount
        {
            get { return printCount; }
            private set { printCount = value; }
        }

        public ContactsFormatter Formatter
        {
            get
            {
                if (formatter == null)
                    formatter = ContactsFormatter.CreateContactsFormatter(Account, ContactsNAW, SecondContactsNAW);
                return formatter;
            }
        }

        public virtual string ModelPortfolioName
        {
            get
            {
                IModelHistory modelHistory = Account.ModelPortfolioChanges.GetItemByDate(ModelDate);
                if (modelHistory == null || modelHistory.ModelPortfolio == null)
                    return (Account.ModelPortfolio != null ? Account.ModelPortfolio.ModelName : "");
                else
                    return modelHistory.ModelPortfolio.ModelName;
            }
        }

        public virtual DateTime ModelDate
        {
            get { return TransactionDate; }
        }

        public virtual INotaAccount NotaAccount { get; private set; }

        public virtual NotaTypes NotaType { get; private set; }

        #endregion

        #region Abstract Props

        public abstract ICustomerAccount Account { get; }
        public abstract bool IsStorno { get; }
        public abstract ICurrency InstrumentCurrency { get; }
        public abstract Money GrandTotalValue { get; }
        public abstract Money TotalValue { get; }
        public abstract DateTime TransactionDate { get; }
        public abstract decimal ExchangeRate { get; }
        public abstract string Title { get; }

        #endregion

        #region Methods

        public int IncrementPrintCount()
        {
            return ++printCount;
        }

        #endregion

        #region Private Variables

        private int key;
        private string notaNumber;
        private DateTime creationDate;
        private ContactsFormatter formatter;
        private IContactsNAW contactsNAW;
        private IContactsNAW secondContactsNAW;
        private int printCount;

        #endregion
    }
}
