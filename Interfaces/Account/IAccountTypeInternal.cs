using System;
using System.Collections.Generic;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Accounts.Portfolios;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This enumeration is used for filtering an accounts portfolio
    /// </summary>
    public enum PositionAmountReturnValue
    {
        /// <summary>
        /// Only use the cash positions in the porfolio
        /// </summary>
        Cash,
        /// <summary>
        /// Only use the cash fund positions in the porfolio
        /// </summary>
        CashFund,
        /// <summary>
        /// Use both the cash and cash fund positions in the porfolio
        /// </summary>
        BothCash,
        /// <summary>
        /// Use all positions in the porfolio
        /// </summary>
        All
    }

    /// <summary>
    /// This enumeration is used for returning the open order amount
    /// </summary>
    public enum OpenOrderAmountReturnValue
    {
        /// <summary>
        /// Return the nett open order amount
        /// </summary>
        Nett,
        /// <summary>
        /// Return the gross open order amount
        /// </summary>
        Gross
    }

    public enum Tradeability
    {
        Tradeable = 1,
        NonTradeable = 2
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.AccountTypeInternal">AccountTypeInternal</see> class
    /// </summary>
    public interface IAccountTypeInternal : IAccount
    {
        bool CommissionCalcReqd { get; }
        bool ValuationsRequired { get; set; }
        DateTime DateTradeabilityStatusChanged { get; set; }
        DateTime FirstTransactionDate { get; }
        DateTime? ValuationsEndDate { get; set; }
        IAccountOrderCollection OpenOrdersForAccount { get; }
        IAccountTypeExternal DefaultAccountforTransfer { get; }
        IEndTermValueCollection EndTermValues { get; }
        IManagementCompany AccountOwner { get; set; }
        IPortfolio Portfolio { get; set; }
        ITradingAccount AccountforAggregation { get; }
        Money OpenOrderAmount();
        Money OpenOrderAmount(OpenOrderAmountReturnValue retVal);
        Money OpenOrderAmount(OpenOrderAmountReturnValue retVal, OrderSideFilter sideFilter);
        Money TotalAll { get; }
        Money TotalBothCash { get; }
        Money TotalCash { get; }
        Money TotalCashAmount { get; }
        Money TotalCashFund { get; }
        Money TotalPositionAmount(PositionAmountReturnValue retVal);
        Tradeability TradeableStatus { get; set; }


    }
}
