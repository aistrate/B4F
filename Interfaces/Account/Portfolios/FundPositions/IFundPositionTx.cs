using System;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;


namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public enum PositionsTxValueTypes
    {
        Value = 0,
        Conversion = 6
    }
    public interface IFundPositionTx
    {
        DateTime CreationDate { get; set; }
        bool Exported { get; set; }
        int Key { get; set; }
        PositionsTxValueTypes ValueType { get; set; }
        ITransaction ParentTransaction { get; set; }
        IFundPosition ParentPosition { get; set; }
        TransactionSide TxSide { get; set; }
        InstrumentSize Size { get; }
        IInstrumentsWithPrices Instrument { get; }
        bool IsStornoable { get; }
        bool IsRelevant { get; }
        bool DoNotRealize { get; }
        bool IsConversion { get; }
        IAccountTypeInternal Account { get; }
        Price Price { get; }
        Money AccruedInterest { get; }
        string PriceShortDisplayString { get; }
        Money Value { get; }
        string Description { get; }
        DateTime TransactionDate { get; }
        Side Side { get; }
        decimal ExchangeRate { get; }
        Money ValueInBaseCurrency { get; }
        Money BookValue { get; }
        Money BookValueIC { get; }
    }
}
