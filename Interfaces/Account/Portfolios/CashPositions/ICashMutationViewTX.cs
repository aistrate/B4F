using System;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface ICashMutationViewTX : ICashMutationView
    {
        InstrumentSize Size { get; }
        Price Price { get; }
        Money TotalCommission { get; }
        B4F.TotalGiro.Orders.Side SideOfTX { get; }
        B4F.TotalGiro.Orders.Transactions.ITransaction Trade { get; set; }
        decimal SizeQuantity { get; }
        decimal TotalCommissionQuantity { get; }
        string PriceShortDisplayString { get; }
        string TradedInstrumentName { get; }
    }
}
