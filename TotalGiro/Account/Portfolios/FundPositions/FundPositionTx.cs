using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public class FundPositionTx : IFundPositionTx
    {

        public FundPositionTx() 
        { 
            this.CreationDate = DateTime.Now;
        }

        public FundPositionTx(ITransaction transaction, TransactionSide txSide, PositionsTxValueTypes valueType)
            : this()
        {
            this.ParentTransaction = transaction;
            this.TxSide = txSide;
            this.ValueType = valueType;
        }

        public int Key { get; set; }
        public ITransaction ParentTransaction { get; set; }
        public IFundPosition ParentPosition { get; set; }
        public TransactionSide TxSide { get; set; }
        public bool Exported { get; set; }
        public PositionsTxValueTypes ValueType { get; set; }

        public virtual DateTime CreationDate
        {
            get { return (creationDate.HasValue ? creationDate.Value : DateTime.MinValue); }
            set { creationDate = value; }
        }

        public virtual bool IsStornoable
        {
            get
            {
                return ParentTransaction.IsStornoable;
            }
        }

        /// <summary>
        /// If the positionTx belongs to either a storno or a stornoed transaction, it is not relevant
        /// </summary>
        public virtual bool IsRelevant
        {
            get
            {
                bool isRelevant = false;
                if (ParentTransaction != null)
                    isRelevant = ParentTransaction.IsRelevant;
                return isRelevant;
            }
        }

        /// <summary>
        /// Does this positionTx belong to a non-realizing ntm
        /// </summary>
        public virtual bool DoNotRealize
        {
            get
            {
                bool doNotRealize = false;
                if (ParentTransaction != null && ParentTransaction.TransactionType == TransactionTypes.InstrumentConversion && ValueType == PositionsTxValueTypes.Value)
                    doNotRealize = true;
                return doNotRealize;
            }
        }


        /// <summary>
        /// The positionTx belongs to a corporate action and opens a new position
        /// </summary>
        public virtual bool IsConversion
        {
            get
            {
                bool isConversion = false;
                if (ParentTransaction != null && ParentTransaction.TransactionType == TransactionTypes.InstrumentConversion && ValueType == PositionsTxValueTypes.Conversion)
                    isConversion = true;
                return isConversion;
            }
        }

        public virtual string Description { get { return ParentTransaction.Description; } }

        public virtual Price Price { get { return ParentTransaction.Price; } }
        public virtual Money AccruedInterest { get { return ParentTransaction.AccruedInterest; } }

        public virtual DateTime TransactionDate { get { return ParentTransaction.TransactionDate; } }

        public virtual string PriceShortDisplayString
        {
            get { return (Price != null ? Price.ShortDisplayString : ""); }
        }

        /// <summary>
        /// The account that holds the position
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return this.ParentPosition.Account; }
        }

        public virtual Side Side
        {
            get
            {
                int side = (int)ParentTransaction.TxSide;
                if (ValueType == PositionsTxValueTypes.Conversion)
                    return (Side)(side * getSign() * -1);
                else
                    return (Side)(side * getSign());
            }
        }

        /// <summary>
        /// The instrument of the position
        /// </summary>
        public virtual IInstrumentsWithPrices Instrument
        {
            get { return (IInstrumentsWithPrices)Size.Underlying; }
        }

        /// <summary>
        /// The value of the position transaction
        /// </summary>
        public virtual Money Value
        {
            get
            {
                return ParentTransaction.CounterValueSize.Negate();
            }
        }

        protected short getSign()
        {
            return (this.TxSide == TransactionSide.A ? (short)1 : (short)-1);
        }

        public DateTime LastUpdated
        {
            get
            {
                return this.lastUpdated.HasValue ? lastUpdated.Value : DateTime.MinValue;
            }
        }

        /// <summary>
        /// The size of the position transaction
        /// </summary>
        public virtual InstrumentSize Size
        {
            get
            {
                switch (ValueType)
                {
                    case PositionsTxValueTypes.Value:
                        return (ParentTransaction.ValueSize * getSign());
                    case PositionsTxValueTypes.Conversion:
                        return ((IInstrumentConversion)ParentTransaction).ConvertedInstrumentSize * getSign();
                    default:
                        throw new ApplicationException(string.Format("CVType {0} does not exist.", ValueType.ToString()));
                }
            }
        }

        public virtual decimal ExchangeRate
        {
            get
            {
                if (Size.IsMoney)
                {
                    if (ParentTransaction.ValueSize.IsMoney && ParentTransaction.CounterValueSize != null &&
                        ParentTransaction.CounterValueSize.IsNotZero && ParentTransaction.CounterValueSize.IsMoney &&
                        ((ParentTransaction.TransactionType == TransactionTypes.Allocation || ParentTransaction.TransactionType == TransactionTypes.Execution) ||
                        (ParentTransaction.IsStorno && ParentTransaction.OriginalTransaction != null && ParentTransaction.OriginalTransaction.TransactionType == TransactionTypes.Allocation)))
                    {
                        if (((ICurrency)Size.Underlying).IsBase)
                            return 1M;
                        else
                        {
                            return Math.Round(1 / ParentTransaction.Price.Quantity, 7);
                        }
                    }
                    else
                    {
                        if (((ICurrency)Size.Underlying).IsBase)
                            return 1M;
                        else
                            return ParentTransaction.ExchangeRate;
                    }
                }
                else
                {
                    if (((ITradeableInstrument)Size.Underlying).CurrencyNominal.IsBase)
                        return 1M;
                    else
                        return ParentTransaction.ExchangeRate;
                }
            }
        }

        /// <summary>
        /// The value of the position transaction in Base Currency
        /// </summary>
        public virtual Money ValueInBaseCurrency
        {
            get
            {
                Money baseValue = null;
                if (Value != null && Value.IsMoney)
                    baseValue = Value.BaseAmount;
                return baseValue;
            }
        }

        /// <summary>
        /// The book value of the position transaction (in Base Currency)
        /// </summary>
        public virtual Money BookValue
        {
            get
            {
                Money bookValue = ValueInBaseCurrency;
                if ((bookValue == null || bookValue.IsZero) && (Price != null && Price.IsNotZero && Size.IsNotZero))
                {
                    bookValue = Size.CalculateAmount(Price, true).BaseAmount;
                }
                return bookValue;
            }
        }

        /// <summary>
        /// The value of the position transaction (in instrument Currency). Used for CostPrice calculation
        /// </summary>
        public virtual Money BookValueIC
        {
            get
            {
                Money bkValueIC = null;
                if (Value != null && Value.IsMoney)
                    bkValueIC = Value;
                if ((bkValueIC == null || bkValueIC.IsZero) && (Price != null && Price.IsNotZero && Size.IsNotZero))
                    bkValueIC = Size.CalculateAmount(Price, true);
                return bkValueIC;
            }
        }


        private DateTime? lastUpdated;
        private DateTime? creationDate;

    }
}
