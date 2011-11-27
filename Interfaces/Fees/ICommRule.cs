using System;
using System.Collections.Generic;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Fees.CommRules
{
	/// <summary>
	/// The type of commission rule
	/// </summary>
    public enum CommRuleTypes
	{
		/// <summary>
		/// A specific commission rule (for a particular account or a account/instrument combination)
		/// </summary>
        Specific = 0,
        /// <summary>
        /// The default commission rule
        /// </summary>
        Default
	}

    /// <summary>
    /// Does this rule apply to either open, close or both type of orders
    /// </summary>
    public enum CommRuleOpenClose
	{
		/// <summary>
        /// This rule applies to open orders only
		/// </summary>
        Open = 1,
        /// <summary>
        /// This rule applies to close orders only
        /// </summary>
        Close = 2,
        /// <summary>
        /// This rule applies is not dependant on the open/close flag
        /// </summary>
        Both = 3
	}

    /// <summary>
    /// Does this rule apply to either buy, sell or both type of orders
    /// </summary>
    public enum CommRuleBuySell
	{
        /// <summary>
        /// This rule applies to buy orders only
        /// </summary>
        Buy = 1,
        /// <summary>
        /// This rule applies to buy orders only
        /// </summary>
        Sell = 2,
        /// <summary>
        /// This rule applies is not dependant on the buy/sell flag
        /// </summary>
        Both = 3
	}

    /// <summary>
    /// Enumeration used for identifying the base order types.
    /// </summary>
    public enum BaseOrderTypes
    {
        /// <summary>
        /// Amount and size based orders
        /// </summary>
        Both,
        /// <summary>
        /// Amount based orders
        /// </summar
        AmountBased,
        /// <summary>
        /// Size based orders
        /// </summary>
        SizeBased
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Fees.CommRules.CommRule">CommRule</see> class
    /// </summary>
    public interface ICommRule
	{
		int Key { get; set; }
        IAssetManager AssetManager { get; }
        int Weight { get; set; }
		string CommRuleName { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        OrderActionTypes ActionType { get; }
		bool ApplyToAllAccounts { get; }
		AccountTypes AccountType { get; }
        IPortfolioModel ModelPortfolio { get; }
		IAccount Account { get; }
        bool HasEmployerRelation { get; }
		IExchange Exchange { get; }
		ISecCategory RuleSecCategory { get; }
		IInstrument Instrument { get; }
		CommRuleBuySell BuySell { get; }
        CommRuleOpenClose OpenClose { get; }
		ICommCalc CommCalculation { get; }
        ICommCalc AdditionalCalculation { get; }
        CommRuleTypes CommRuleType { get; }
        BaseOrderTypes OriginalOrderType { get; }
        string DisplayRule { get; }
        bool CalculateWeight(ICommClient client);
	}
}
