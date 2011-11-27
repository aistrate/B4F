using System;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Orders.Transactions
{
    public abstract class TransactionTrading : Transaction, ITransactionTrading
    {
        public TransactionTrading() : base() { }

        public TransactionTrading(IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize, Price price, decimal exRate, DateTime transactionDate,
                DateTime transactionDateTime, Decimal ServiceChargePercentage, Side txSide,
            ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(acctA, acctB, valueSize, price, exRate, transactionDate,
                    transactionDateTime, ServiceChargePercentage, txSide,
                    tradingJournalEntry, lookups, components)
        {

        }

        public bool IsClientSettled { get; set; }
        public virtual void ClientSettle(ITradingJournalEntry clientSettleJournal)
        {
            clientSettle(this.TradingJournalEntry);
        }

        protected void clientSettle(ITradingJournalEntry clientSettleJournal)
        {
            if (!IsClientSettled)
            {
                var lines = this.Components.SelectMany(x => x.JournalLines).Where(c => (c.GLAccount.IsToSettleWithClient && !c.IsSettledStatus));
                foreach (IJournalEntryLine line in lines)
                    line.ClientSettle(clientSettleJournal);
                clientSettleJournal.BookLines();
                IsClientSettled = true;
                
                ClientSettlementDate = clientSettleJournal.TransactionDate;

            }
        }

        protected override void setDescription()
        {
            if (this.IsStorno)
            {
                this.Description = string.Format("Storno ({0}) {1}",
                    this.OriginalTransaction.Key,
                    this.StornoReason);
            }
            else
            {
                string commInfo = "";
                string accIntInfo = "";
                Money provisie = Components.ReturnComponentValue(new BookingComponentTypes[] { BookingComponentTypes.Commission, BookingComponentTypes.ServiceCharge });
                if (provisie != null && provisie.IsNotZero)
                    commInfo = string.Format(" (Provisie {0})", provisie.DisplayString);

                Money accruedInterest = Components.ReturnComponentValue(BookingComponentTypes.AccruedInterest);
                if (accruedInterest != null && accruedInterest.IsNotZero)
                    accIntInfo = string.Format(" Accrued Interest {0}", accruedInterest.DisplayString);

                string exRateInfo = "";
                string foreignAmountInfo = "";
                if (!this.TradedInstrument.CurrencyNominal.IsBase)
                {
                    exRateInfo = string.Format(Environment.NewLine + " Wisselkoers: {0}", this.ExchangeRate);
                    Money foreignAmount = Components.TotalValueComponentsInSpecifiedCurrency(TradedInstrument.CurrencyNominal);
                    if (foreignAmount != null && foreignAmount.IsNotZero)
                        foreignAmountInfo = string.Format("  Vreemd vermogen: {0}", foreignAmount.DisplayString);
                }

                this.Description = string.Format("{0} {1} {2} @ {3}{4}{5}{6}{7}",
                    TxSide == Side.Buy ? "Aankoop" : "Verkoop",
                    ValueSize.ToString("#,###,##0.00####"),
                    TradedInstrument.Name,
                    Price.Amount.DisplayString,
                    accIntInfo,
                    commInfo,
                    exRateInfo,
                    foreignAmountInfo);
            }
        }

        public virtual DateTime ClientSettlementDate
        {
            get
            {
                if (clientSettlementDate.HasValue)
                    return clientSettlementDate.Value;
                else
                    return DateTime.MinValue;
            }
            set
            {
                clientSettlementDate = value;
            }
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.Migration; }
        }

        #region Privates

        private DateTime? clientSettlementDate;

        #endregion
    }
}
