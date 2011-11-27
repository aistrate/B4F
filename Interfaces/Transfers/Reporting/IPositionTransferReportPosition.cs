using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Orders.Transfers.Reporting
{
    public interface IPositionTransferReportPosition
    {
        bool IsFundPosition { get; }
        decimal ExchangeRate { get; set; }
        decimal PercentageOfPortfolioAfter { get; set; }
        decimal PercentageOfPortfolioBefore { get; set; }
        IAccountTypeInternal Account { get; }
        IInstrument InstrumentOfPosition { get; set; }
        InstrumentSize AfterPositionSize { get; set; }
        InstrumentSize BeforePositionSize { get; set; }
        InstrumentSize TransferPositionSize { get; set; }
        int Key { get; set; }
        int SortOrder { get; }
        bool IsChanged { get; }
        IPositionTransferReportPortfolio ParentPortfolio { get; set; }
        Money ValueinEuroAfter { get; set; }
        Money ValueinEuroBefore { get; set; }
        Money ValueVVAfter { get; set; }
        Money ValueVVBefore { get; set; }
        Price ActualPrice { get; set; }
        string AccountDescription { get; }
        string ActualPriceShortDisplayString { get; }
        string AfterPositionSizeDisplayString { get; }
        string Isin { get; }
        string TransferPositionSizeSizeDisplayString { get; }
        string PercentageOfPortfolioBeforeDisplayString { get; }
        string PercentageOfPortfolioAfterDisplayString { get;}

    }
}
