using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Instructions.Exclusions;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// The different conditions that an instruction are tested for during its lifetime
    /// </summary>
    public enum RebalanceInstructionConditions
    {
        /// <summary>
        /// Check whether there are either pending orders or new size based closing orders need to be entered due to a restucture
        /// </summary>
        CheckSizeBasedClose = 1,
        /// <summary>
        /// Check whether there are pending orders
        /// </summary>
        PendingCloseOrders = 2,
        /// <summary>
        /// Check whether there are pending (amount based) orders due to a rebalance
        /// </summary>
        PendingRebalance = 3,
        /// <summary>
        /// Check whether there are pending (monetary) orders
        /// </summary>
        PendingCashFund = 4,
        /// <summary>
        /// Check whether it is possible to cancel the instruction
        /// </summary>
        CheckCancel = 5
    }

    /// <summary>
    /// The different actions that an instruction can initiate during its lifetime
    /// </summary>
    public enum RebalanceInstructionActions
    {
        /// <summary>
        /// Create size based close orders due to a restructure of the modelportfolio
        /// </summary>
        SizeBasedCloseOrders = 1,
        /// <summary>
        /// The actual rebalance (bringing the portfolio back to the proportions of the model
        /// </summary>
        Rebalance = 2,
        /// <summary>
        /// Placing cash fund orders to get rid of the cash
        /// </summary>
        CashFundOrders = 3,
        /// <summary>
        /// Terminate the instruction
        /// </summary>
        Terminate = 4,
        /// <summary>
        /// Cancel the instruction
        /// </summary>
        Cancel = 5
    }

    public interface IRebalanceInstruction : IInstructionTypeRebalance
    {
        bool NeedsToCheckInstructionInSyncWithModel { get; }
        bool NeedsToProcessCashTransfers { get; }
        ICashWithdrawalInstruction CashWithdrawalInstruction { get; set; }
        IRebalanceExclusionCollection ExcludedComponents { get; }
    }
}
