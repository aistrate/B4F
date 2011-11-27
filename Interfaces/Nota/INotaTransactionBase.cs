using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Notas
{
    public interface INotaTransactionBase : INota
    {
        ITradeableInstrument TradedInstrument { get; }
        Money Commission { get; }
        Money CounterValue { get; }
        ITransaction OriginalTransaction { get; }
        Price Price { get; }
        Money ServiceCharge { get; }
        INota StornoedTransactionNota { get; }
        string TxSide { get; }
        ITransaction UnderlyingTx { get; }
        InstrumentSize ValueSize { get; }
        InstrumentSize ValueSizeAbs { get; }
    }
}
