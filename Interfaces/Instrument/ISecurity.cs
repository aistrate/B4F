using System;

namespace B4F.TotalGiro.Instruments
{
    public interface ISecurityInstrument: ITradeableInstrument
    {
        bool IsGreenFund { get; set; }
        bool IsCultureFund { get; set; }
        bool SupportsStockDividend { get; }
        IInstrumentCorporateActionCollection CorporateActionInstruments { get; }

        bool Transform(DateTime changeDate, decimal oldChildRatio, byte newParentRatio, bool isSpinOff,
                string instrumentName, string isin, DateTime issueDate);
    }
}
