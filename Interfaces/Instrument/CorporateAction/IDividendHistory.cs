using System;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public enum DividendTypes
    {
        Cash = 1,
        Scrip
    }
    
    public enum DividendTaxStyle
    {
        Gross = 1,
        Nett
    }

    public interface IDividendHistory : ICorporateActionHistory
    {
        DividendTypes DividendType { get; set; }
        DateTime ExDividendDate { get; set; }
        DateTime SettlementDate { get; set; }
        Price UnitPrice { get; set; }
        Price StockDivUnitPrice { get; }
        decimal ScripRatio { get; set; }
        string DisplayStatus { get; }
        bool IsInitialised { get; set; }
        bool IsExecuted { get; set; }
        bool IsGelicht { get; set; }
        decimal TaxPercentage { get; set; }
        string StockDivIsin { get; set; }
        IStockDividend StockDividend { get; set; }
        DividendTaxStyle TypeOfDividendTax { get; set; }
        bool NeedsStockDividend { get; }
        IStockDividendCollection StockDividends { get; }
        ICashDividendCollection CashDividends { get; }
    }
}
