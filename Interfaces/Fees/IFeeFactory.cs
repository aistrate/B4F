using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.TaxRates;

namespace B4F.TotalGiro.Fees
{
    #region enum

    /// <summary>
    /// This enumeration lists the possible feefactory instances
    /// </summary>
    public enum FeeFactoryInstanceTypes
    {
        /// <summary>
        /// Starts up a feefactory instance for both commission & fees
        /// </summary>
        All = 0,
        /// <summary>
        /// Starts up a commission feefactory instance (default)
        /// </summary>
        Commission = 1,
        /// <summary>
        /// Starts up a fees feefactory instance
        /// </summary>
        Fee = 2
    }
    
    /// <summary>
    /// This enumeration lists the possible calculation types
    /// </summary>
    public enum FeeCalcTypes
    {
        /// <summary>
        /// This is a flat calculation rule (staffel in dutch)
        /// </summary>
        Flat = 1,
        /// <summary>
        /// This is a slab calculation rule (like calculating tax)
        /// </summary>
        Slab = 2,
        /// <summary>
        /// This is a simple calculation rule with just one rule
        /// </summary>
        Simple = 3,
        /// <summary>
        /// This is a Flat Size Based calculation
        /// </summary>
        FlatSizeBased = 4
    }

    #endregion

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Fees.FeeFactory">FeeFactory</see> class
    /// </summary>
    public interface IFeeFactory
	{
        void InitiateInstance(IDalSession session, FeeFactoryInstanceTypes instanceType);
        void InitiateInstance(IDalSession session, FeeFactoryInstanceTypes instanceType, bool keepSession);
        bool IsInstanceTypeActivated(FeeFactoryInstanceTypes instanceType);
        Commission CalculateCommission(IOrder order);
        Commission CalculateCommission(ITransactionOrder transaction);
        Commission CalculateCommission(ICommClient client);
        ICommRule GetRelevantCommRule(ICommClient client);
        ICommRule GetRelevantCommRule(IAccountTypeInternal account, IInstrument instrument, Side side, OrderActionTypes actiontype, DateTime transactionDate, bool isAmountBased, out ICommClient client);
        bool CalculateFeePerUnit(IDalSession session, IManagementPeriodUnit unit);
        IHistoricalTaxRate GetHistoricalTaxRate(DateTime date);
        void CloseSession();
    }
}
