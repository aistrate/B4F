using System;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Orders.Transfers;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments.CorporateAction;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class CorporateActionExecution : Transaction, ICorporateActionExecution
    {
        public CorporateActionExecution() : base() { }

        public CorporateActionExecution(IAccountTypeInternal acctA, IAccount acctB,
        InstrumentSize valueSize,
        Price price, decimal exRate, DateTime transactionDate,
        ICorporateActionHistory corporateActionDetails, ITradingJournalEntry tradingJournalEntry,
        string description)
            : base(acctA, acctB, valueSize, price, exRate, transactionDate,
                    transactionDate, 0M, valueSize.Sign ? Side.XI : Side.XO,
                    tradingJournalEntry, null, null)
        {
            if (!valueSize.Underlying.IsCorporateAction)
                throw new ApplicationException("This type of transaction can only be used for corporate actions");

            if (corporateActionDetails == null)
                throw new ApplicationException("Corporate Action details are mandatory when creating a corporate action execution");

            this.CorporateActionDetails = corporateActionDetails;
            this.Description = description;
        }

        public virtual ITransactionNTM CounterTransaction { get; set; }
        public virtual IGeneralOperationsBooking CounterBooking { get; set; }

        /// <summary>
        /// The details of the cash dividend (date & price)
        /// </summary>
        public virtual ICorporateActionHistory CorporateActionDetails
        {
            get { return corporateActionDetails; }
            set { corporateActionDetails = value; }
        }

        public new IInstrumentsWithPrices TradedInstrument
        {
            get { return (IInstrumentsWithPrices)ValueSize.Underlying; }
        }
        
        public override bool Approve(IInternalEmployeeLogin employee)
        {
            return this.Approve(employee, true);
        }

        public override bool Approve(IInternalEmployeeLogin employee, bool raiseStornoLimitExceptions)
        {
            if (corporateActionDetails == null)
                throw new ApplicationException("Corporate Action details are mandatory when creating a corporate action execution");
            if (CounterTransaction == null && CounterBooking == null)
                throw new ApplicationException("Either a counter booking or transaction is mandatory when creating a corporate action execution");

            return base.Approve(employee, raiseStornoLimitExceptions);
        }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            ICorporateActionExecution newStorno = new CorporateActionExecution();
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);

            //if (CounterTransaction != null)
            //    newStorno.CounterTransaction = (ITransactionNTM)CounterTransaction.Storno(stornoAccount, employee, reason, tradingJournalEntry);
            //if (CounterBooking != null)
            //    newStorno.CounterBooking = CounterBooking.Storno(employee, reason, tradingJournalEntry);
            return newStorno;
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.CorporateActionExecution; }
        }

        protected override void setDescription()
        {
            if (string.IsNullOrEmpty(base.description))
            {
                if (this.IsStorno)
                {
                    this.Description = string.Format("Storno ({0}) {1}",
                        this.OriginalTransaction.Key,
                        this.StornoReason);
                }
                else
                {
                    string instrumentName = TradedInstrument.Name;
                    if (TradedInstrument.SecCategory.GetV(e => e.Key) == SecCategories.StockDividend)
                        instrumentName = ((IStockDividend)TradedInstrument).Underlying.Name;
                    this.Description = string.Format("Lichten {0} {1} {2}",
                        TxSide == Side.XI ? "short" : "long",
                        ValueSize.ToString("#,###,##0.00####"),
                        instrumentName);
                }
            }
        }

        public override INota CreateNota()
        {
            throw new ApplicationException("Nota is not supported");
        }

        #region Private Variables

        private ICorporateActionHistory corporateActionDetails;

        #endregion
    }
}
