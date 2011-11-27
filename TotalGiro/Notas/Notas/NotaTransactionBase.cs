using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Notas
{
    public abstract class NotaTransactionBase : Nota, INotaTransactionBase
    {
        #region Constructor

        protected NotaTransactionBase() { }

        protected NotaTransactionBase(ITransaction underlyingTx)
            : base((ICustomerAccount)underlyingTx.AccountA)
        {
            this.underlyingTx = underlyingTx;
            underlyingTx.TxNota = this;
        }

        #endregion

        #region Props

        public virtual ITransaction UnderlyingTx
        {
            get { return underlyingTx; }
            private set { underlyingTx = value; }
        }

        public virtual ITransaction OriginalTransaction
        {
            get { return (IsStorno ? UnderlyingTx.OriginalTransaction : UnderlyingTx); }
        }

        public virtual INota StornoedTransactionNota
        {
            get { return (IsStorno ? UnderlyingTx.OriginalTransaction.TxNota : null); }
        }

        public virtual ITradeableInstrument TradedInstrument
        {
            get { return (ITradeableInstrument)UnderlyingTx.TradedInstrument; }
        }

        public virtual string TxSide
        {
            get { return UnderlyingTx.TxSide.ToString(); }
        }

        public virtual InstrumentSize ValueSize
        {
            get { return UnderlyingTx.ValueSize; }
        }

        public virtual InstrumentSize ValueSizeAbs
        {
            get { return ValueSize.Abs(); }
        }

        public virtual Price Price
        {
            get { return UnderlyingTx.Price; }
        }

        public virtual Money CounterValue
        {
            get { return UnderlyingTx.CounterValueSize; }
        }

        public virtual Money ServiceCharge
        {
            get
            {
                return (UnderlyingTx.ServiceCharge != null ?
                            UnderlyingTx.ServiceCharge.BaseAmount :
                            new Money(0.00m, Account.BaseCurrency));
            }
        }

        public virtual Money Commission
        {
            get
            {
                return (UnderlyingTx.Commission != null ?
                            UnderlyingTx.Commission.BaseAmount :
                            new Money(0.00m, Account.BaseCurrency));
            }
        }

        #endregion

        #region Overriden Props

        public override ICustomerAccount Account
        {
            get { return (ICustomerAccount)UnderlyingTx.AccountA; }
        }

        public override bool IsStorno
        {
            get { return UnderlyingTx.IsStorno; }
        }

        public override ICurrency InstrumentCurrency
        {
            get { return ((ITradeableInstrument)ValueSize.Underlying).CurrencyNominal; }
        }

        public override Money GrandTotalValue
        {
            get { return TotalValue.BaseAmount + ServiceCharge + Commission; }
        }

        public override Money TotalValue
        {
            get { return CounterValue; }
        }

        public override DateTime TransactionDate
        {
            get { return UnderlyingTx.TransactionDate; }
        }

        public override decimal ExchangeRate
        {
            get { return UnderlyingTx.ExchangeRate; }
        }

        #endregion

        #region Private Variables

        private ITransaction underlyingTx;

        #endregion
    }
}
