using System;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Orders.Transfers
{
    public interface IPositionTransferDetail
    {
        bool IsDeletable { get; }
        bool IsEditable { get; }
        bool IsFundPosition { get; }
        DateTime CreationDate { get; }
        DateTime TransferDate { get; }
        decimal ExchangeRate { get; set; }
        decimal Size { get; }
        IInstrument InstrumentOfPosition { get; }
        InstrumentSize PositionSize { get; set; }
        int GetHashCode();
        int Key { get; set; }
        IPositionTransfer ParentTransfer { get; set; }
        Money ValueinEuro { get; set; }
        Money ValueVV { get; set; }
        Price TransferPrice { get; set; }
        string InstrumentDescription { get; }
        string InstrumentName { get; }
        string Isin { get; }
        string TransferPriceShortDisplayString { get; }
        string ActualPriceShortDisplayString { get; }
        TransferDirection TxDirection { get; set; }
        Price ActualPrice { get; set; }
        TransferStatus ParentStatus { get; }
        ITransactionNTMCollection Transactions { get; }

    }
}
