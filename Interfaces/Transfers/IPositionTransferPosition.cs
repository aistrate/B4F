using System;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Orders.Transfers
{
    public enum TransferDirection
    {
        FromAtoB = 0,
        FromBtoA
    }

    public interface IPositionTransferPosition
    {
        decimal ExchangeRate { get; set; }
        int Key { get; set; }
        IPositionTransferPortfolio ParentPortfolio { get; set; }
        B4F.TotalGiro.Instruments.InstrumentSize PositionSize { get; set; }
        B4F.TotalGiro.Instruments.Price ActualPrice { get; set; }
        B4F.TotalGiro.Instruments.Money ValueinEuro { get; set; }
        B4F.TotalGiro.Instruments.Money ValueVV { get; set; }
        Decimal PercentageOfPortfolio { get; set; }
        string Isin { get; }
        string InstrumentName { get; }
        Decimal Size { get; }
        string PriceShortDisplayString { get; }
        bool IsFundPosition { get; }
        string InstrumentDescription { get; }
        Decimal FundPercentageOfPortfolio { get; set; }
        bool IsEditable { get; }
        bool IsDeletable { get; }
        IInstrument InstrumentOfPosition { get; }
    }
}
