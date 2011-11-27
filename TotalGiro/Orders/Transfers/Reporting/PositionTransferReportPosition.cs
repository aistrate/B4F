using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Orders.Transfers.Reporting
{
    public class PositionTransferReportPosition : IPositionTransferReportPosition
    {
        public int Key { get; set; }
        public IPositionTransferReportPortfolio ParentPortfolio { get; set; }
        public IAccountTypeInternal Account { get { return this.ParentPortfolio.Account; } }
        public InstrumentSize BeforePositionSize { get; set; }
        public InstrumentSize TransferPositionSize { get; set; }
        public string TransferPositionSizeSizeDisplayString { get { return this.TransferStatus == TransferStatus.Executed ? this.TransferPositionSize.DisplayString : "n.v.t"; } }
        public InstrumentSize AfterPositionSize { get; set; }
        public string AfterPositionSizeDisplayString { get { return this.TransferStatus == TransferStatus.Executed ? this.AfterPositionSize.DisplayString : "n.v.t"; } }
        public Price ActualPrice { get; set; }
        public string ActualPriceShortDisplayString { get { return (this.ActualPrice != null ? this.ActualPrice.ShortDisplayString : ""); } }
        public Decimal ExchangeRate { get; set; }
        public Money ValueVVBefore { get; set; }
        public Money ValueVVAfter { get; set; }
        public Money ValueinEuroBefore { get; set; }
        public Money ValueinEuroAfter { get; set; }
        public bool IsChanged { get { return !(BeforePositionSize == AfterPositionSize); } }
        public Decimal PercentageOfPortfolioBefore { get; set; }
        public string PercentageOfPortfolioBeforeDisplayString { get { return String.Format("{0:##0.0000}%", (this.PercentageOfPortfolioBefore * 100)); } }
        public Decimal PercentageOfPortfolioAfter { get; set; }
        public string PercentageOfPortfolioAfterDisplayString { get { return this.TransferStatus == TransferStatus.Executed ? String.Format("{0:##0.0000}%", (this.PercentageOfPortfolioAfter * 100)): "n.v.t" ; } }
        TransferStatus TransferStatus { get { return this.ParentPortfolio.ParentTransfer.TransferStatus; } }
        public bool IsFundPosition { get { return !this.InstrumentOfPosition.IsCash; } }
        public IInstrument InstrumentOfPosition { get; set; }
        public string Isin { get { return IsFundPosition ? ((ITradeableInstrument)this.InstrumentOfPosition).Isin : "Cash"; } }
        public int SortOrder { get { return IsFundPosition ? 1 : 0; } }
        public string AccountDescription { get { return this.Account.Number + ":- " + this.Account.ShortName; } }
    }
}
