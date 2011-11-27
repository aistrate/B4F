using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for all the Stichting orders
    /// </summary>
    public interface IStgOrder : IOrder
    {
        IRoute Route { get; set;}
        //InstrumentSize TotalOpenValue();
        bool IsSendable { get; }
        bool IsEditable { get; }
        bool IsTypeConverted { get; }
        InstrumentSize ConvertedValue { get; }
        InstrumentSize ValueToBuy { get; }
        InstrumentSize ValueToSell { get; }
        bool Send();
        bool Place();
        bool Reset();
        void SetIsNetted(bool isNetted);
        bool ChangeRoute(IRoute route);
        IOrderExecution Fill(InstrumentSize size, Price price, Money amount, decimal exRate,
            IAccount counterParty, DateTime transactionDate, DateTime transactionDateTime, IExchange exchange,
            bool isCompleteFill, Money serviceCharge, decimal serviceChargePercentage, Money accruedInterest,
            ITradingJournalEntry tradingJournalEntry, IGLLookupRecords lookups);
        bool RemoveTransaction(ITransactionOrder trade);
        bool NeedsCurrencyConversion { get; }
        //ITradeableInstrument TradedInstrument { get; }
        
    }
}
