using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios
{
    #region enums

    /// <summary>
    /// Is this a long or short position
    /// </summary>
    public enum IsLong
    {
        /// <summary>
        /// This is a long position
        /// </summary>
        Long = 1,
        /// <summary>
        /// This is a short position
        /// </summary>
        Short = -1
    }

    #endregion

    public interface IPortfolio
    {
        IAccountTypeInternal ParentAccount { get; set; }
        ICashPortfolio PortfolioCashGL { get; }
        IFundPortfolio PortfolioInstrument { get; }
        DateTime LastTransactionDate { get; }
        //DateTime LastCashTransactionDate { get; }
        List<ICommonPosition> AllPositions { get; }
        List<ICommonPosition> AllActivePositions { get; }
        Money TotalValue();
        Money MaxWithdrawalAmount();
        bool HasPositions { get; }
    }
}
