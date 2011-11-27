using System;
using B4F.TotalGiro.Orders.Transactions;
namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface ITradingJournalEntry : IJournalEntry
    {
        ITransaction Trade { get; set; }
        IExternalSettlement MatchedSettlement { get; set; }
        Instruments.InstrumentSize TradeSize { get; }
        string TradeSizeDisplay { get; }
        string CounterParty { get; }
        string TradedInstrument { get; }
        Instruments.Price TradePrice { get; }
    }
}
