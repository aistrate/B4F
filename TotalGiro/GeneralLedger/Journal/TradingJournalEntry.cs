using System;
using System.Text;
using System.Collections;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Communicator.Exact;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public class TradingJournalEntry : JournalEntry, ITradingJournalEntry
    {
        public TradingJournalEntry() { }

        public TradingJournalEntry(IJournal journal, string journalEntryNumber, DateTime transactionDate)
            : base(journal, journalEntryNumber)
        {
            this.TransactionDate = transactionDate;            
        }

        public override JournalEntryTypes JournalEntryType
        {
            get { return JournalEntryTypes.Transaction; }
        }

        public Instruments.InstrumentSize TradeSize
        {
            get
            {
                return Trade.ValueSize;
            }
        }

        public string TradeSizeDisplay
        {
            get
            {
                return this.TradeSize.DisplayString;
            }
        }

        public string CounterParty
        {
            get
            {
                return this.Trade.AccountB.ShortName;
            }
        }


        public string TradedInstrument
        {
            get
            {
                return Trade.TradedInstrument.Name;
            }
        }

        public Instruments.Price TradePrice
        {
            get
            {
                if (Trade.Price != null)
                    return Trade.Price;
                else
                    return new B4F.TotalGiro.Instruments.Price(0m, Trade.ValueSize.Underlying.CurrentPrice.Price.Underlying, Trade.TradedInstrument, Trade.ExchangeRate);
            }
        }

        public virtual ITransaction Trade { get; set; }
        public virtual IExternalSettlement MatchedSettlement { get; set; }

        public override decimal ExchangeRate
        {
            get { return Trade.ExchangeRate; }
        }
    }
}
