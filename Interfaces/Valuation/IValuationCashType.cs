using System;

namespace B4F.TotalGiro.Valuations
{
    /// <summary>
    /// Enum used to for mapping with the valuations
    /// </summary>
    public enum ValuationCashTypes
    {
        CostsOther = -10,
        CostsVAT = -4,
        CostsFee = -3,
        CostsDividendTax = -2,
        CostsCommission = -1,
        None = 0,
        IncomeCashDividend = 1,
        IncomeInterest = 2,
        AccruedInterestUnSettled = 3,
        AccruedInterest = 4,
        IncomeOther = 10
    }

    public interface IValuationCashType
    {
        ValuationCashTypes Key { get; }
        string CashType { get; }
        string Description { get; }
        short Sign { get; }
        bool IsSettled { get; }
    }
}
