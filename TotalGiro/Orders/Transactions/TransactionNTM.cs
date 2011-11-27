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

namespace B4F.TotalGiro.Orders.Transactions
{
    public class TransactionNTM : Transaction , ITransactionNTM
    {
        public TransactionNTM() : base() { }

        public TransactionNTM(IAccountTypeInternal acctA, IAccount acctB,
        InstrumentSize valueSize,
        Price price, decimal exRate, DateTime transactionDate, DateTime transactionDateTime,
        Decimal ServiceChargePercentage, Side txSide,
        ITradingJournalEntry tradingJournalEntry,
        IGLLookupRecords lookups, ListOfTransactionComponents[] components, string description)
            : base(acctA, acctB, valueSize, price, exRate, transactionDate,
                    transactionDateTime, ServiceChargePercentage, txSide,
                    tradingJournalEntry, lookups, components)
        {
            this.Description = description;
        }

        public IPositionTransferDetail TransferDetail { get; set; }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            ITransactionNTM newStorno = new TransactionNTM();
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            return newStorno;
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.NTM; }
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
                    string exRateInfo = "";
                    if (!this.TradedInstrument.CurrencyNominal.IsBase)
                        exRateInfo = string.Format(Environment.NewLine + "Wisselkoers: {0}", this.ExchangeRate);

                    this.Description = string.Format("Transfer {0} {1} {2} @ {3}{4}",
                        TxSide == Side.XI ? "in" : "out",
                        ValueSize.ToString("#,###,##0.00####"),
                        TradedInstrument.Name,
                        Price.Get(e => e.Amount).GetS(e => e.DisplayString),
                        exRateInfo);
                }
            }
        }

        public override INota CreateNota()
        {
            if (Approved && !NotaMigrated && StornoTransaction == null)
            {
                if (TxNota == null)
                    return new NotaTransfer(this);
                else
                    throw new ApplicationException(string.Format("Transaction {0} already has a nota ({1}).", Key, TxNota.Key));
            }
            return null;
        }
    }
}
