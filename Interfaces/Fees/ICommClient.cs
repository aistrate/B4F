using System;
using System.Collections.Generic;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Fees.CommRules;

namespace B4F.TotalGiro.Fees
{
    public enum CommClientType
    {
        Order,
        Transaction,
        Test
    }

    public interface ICommClient
    {
        //ArrayList SelectedRules { get; set; }
        BaseOrderTypes OriginalOrderType { get; }
        bool IsSizeBased { get; }
        bool IsValueInclComm { get; }
        CommClientType Type { get; }
        DateTime TransactionDate { get; set; }
        DateTime SettlementDate { get; set; }
        IAccountTypeInternal Account { get; }
        bool HasEmployerRelation { get; }
        ICurrency OrderCurrency { get; }
        IInstrument TradedInstrument { get; }
        InstrumentSize Value { get; }
        Money Amount { get; }
        Money GrossAmount { get; }
        bool AmountIsNett { get; }
        Money AccruedInterest { get; }
        Money PreviousCalculatedFee { get; set; }
        OrderActionTypes ActionType { get; }
        OrderTypes OrderType { get; }
        Price Price { get; }
        Side Side { get; }
        string CommissionInfo { get; set; }

        ICommClient GetNewInstance(InstrumentSize size, Price price);
        ICommClient GetNewInstance(InstrumentSize size, Price price, Money previousCalculatedFee);
    }
}
