using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This enumeration describes all possible stati during processing of an instruction
    /// </summary>
    public enum RebalanceInstructionStati
    {
        /// <summary>
        /// The instruction is new and not yet being processed
        /// </summary>
        New = 1,
        /// <summary>
        /// There are pending orders found, so the rebalance has to wait.
        /// These ordersmight be size based sell orders due to a restructure of the modelportfolio
        /// </summary>
        PendingOrders = 2,
        /// <summary>
        /// The rebalance has been kicked off
        /// </summary>
        Rebalance = 3,
        /// <summary>
        /// There are still pending orders due to the rebalance
        /// </summary>
        PendingRebalanceOrders = 4,
        /// <summary>
        /// All cash remaining (long, short) is sucked up into the cash fund
        /// </summary>
        CashFund = 5,
        /// <summary>
        /// The cash fund order is still pending
        /// </summary>
        PendingCashFundOrders = 6,
        /// <summary>
        /// The instruction is terminated (inactived)
        /// </summary>
        Terminate = 7
    }

    public enum RebalanceResults
    {
        Success,
        NoRebalanceNegativePortfolioAmount,
        NoCashFundOrdersNegativePortfolioAmount,
        RebalanceWasNotNeeded
    }

    public interface IInstructionTypeRebalance : IInstruction
    {
        OrderActionTypes OrderActionType { get; }
        ICashTransferCollection CashTransfers { get; }
        bool Edit(OrderActionTypes orderActionType, DateTime executionDate, bool doNotChargeCommission);
    }
}
