using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Notas
{
    public interface INotaTransaction : INotaTransactionBase
    {
        IOrder Order { get; }
        ITradeableInstrument TradedInstrument { get; }
        string ExchangeName { get; }
        DateTime TransactionDateTime { get; }
        decimal ServiceChargePercentage { get; }
    }
}
