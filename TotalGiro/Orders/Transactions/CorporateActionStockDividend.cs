using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class CorporateActionStockDividend : CorporateAction, ICorporateActionStockDividend
    {
        public CorporateActionStockDividend() { }

        public CorporateActionStockDividend(IAccountTypeInternal acctA, IAccount acctB,
        InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
        IDividendHistory dividendDetails, InstrumentSize previousSize,
        ITradingJournalEntry tradingJournalEntry)
            : base(acctA, acctB, valueSize,
                 price, exRate, transactionDate, transactionDate,
                 0M, valueSize.Sign ? Side.XI : Side.XO,
                 tradingJournalEntry, null, null)
        {
            if (dividendDetails == null)
                throw new ApplicationException("Dividend details are mandatory when creating a stock dividend corporate action");

            if (previousSize == null)
                throw new ApplicationException("The units in possession are mandatory when creating a stock dividend corporate action");
            else if (previousSize.IsZero)
                throw new ApplicationException("The units can not be zero when creating a stock dividend corporate action");

            this.CorporateActionType = CorporateActionTypes.StockDividend;
            this.CorporateActionDetails = dividendDetails;
            this.PreviousSize = previousSize;
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.StockDividend; }
        }

        /// <summary>
        /// The details of the cash dividend (date & price)
        /// </summary>
        public virtual IDividendHistory DividendDetails
        {
            get { return (IDividendHistory)this.CorporateActionDetails; }
        }

        public new IInstrumentsWithPrices TradedInstrument
        {
            get { return (IInstrumentsWithPrices)ValueSize.Underlying; }
        }

        /// <summary>
        /// The number of units the dividend was paid on
        /// </summary>
        public virtual Money DividendAmount
        {
            get 
            {
                Price price = Price;
                if (price == null)
                    price = new Price(DividendDetails.UnitPrice.Quantity, TradedInstrument.CurrencyNominal, TradedInstrument);
                return this.ValueSize.CalculateAmount(price); 
            }
        }

        public virtual Money TaxAmount
        {
            get
            {
                Money divtax = null;
                if (DividendDetails.TypeOfDividendTax == DividendTaxStyle.Gross)
                    divtax = DividendAmount * DividendDetails.TaxPercentage;
                return divtax;
            }
        }

        /// <summary>
        /// Is the stock dividend already gelicht
        /// </summary>
        public virtual bool IsGelicht { get; set; }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            CorporateActionStockDividend newStorno = new CorporateActionStockDividend();
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            return newStorno;
        }

        public override string DisplayTradedInstrumentIsin
        {
            get
            {
                string isin = "";
                if (TradedInstrument != null && TradedInstrument.IsTradeable)
                    isin = ((ITradeableInstrument)TradedInstrument).Isin;
                return isin;
            }
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
                    this.Description = string.Format("Dividend {0} {1} {2}",
                        TxSide == Side.XI ? "in" : "out",
                        ValueSize.ToString("#,###,##0.00####"),
                        instrumentName);
                }
            }
        }

        public override INota CreateNota()
        {
            // TODO: 'CorporateActionType == CorporateActionTypes.Conversion' is TEMPORARY, until Bonus Emission nota is created
            if (Approved && !NotaMigrated && StornoTransaction == null)
                throw new ApplicationException(string.Format("Transaction {0} already has a nota ({1}).", Key, TxNota.Key));

            return null;
        }
    }
}
