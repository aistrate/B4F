using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.BackOffice.Orders;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This enumeration describes all possible stati during processing of an instruction
    /// </summary>
    public enum ClientDepartureInstructionStati
    {
        /// <summary>
        /// The instruction is new and not yet being processed
        /// </summary>
        New = 1,
        /// <summary>
        /// Liquidate portfolio-> size based close orders for all positions created
        /// </summary>
        LiquidatePortfolio = 2,
        /// <summary>
        /// There are still pending orders due to the liquidation
        /// </summary>
        PendingOrders = 3,
        /// <summary>
        /// ManagementPeriods are terminated and Model changed
        /// </summary>
        FixManagementEndDate = 4,
        /// <summary>
        /// Waiting for the relevant valuations
        /// </summary>
        WaitForValuations = 5,
        /// <summary>
        /// Subtract MgtFee and create MoneyTransferOrder to pay the cash to the account
        /// </summary>
        SettleAccount = 6,
        /// <summary>
        /// The instruction and the account are terminated (inactived)
        /// </summary>
        Terminate = 7
    }
    
    /// <summary>
    /// The different conditions that an instruction are tested for during its lifetime
    /// </summary>
    public enum ClientDepartureInstructionConditions
    {
        /// <summary>
        /// Check if we still need to close all positions
        /// </summary>
        CheckNeedForCloseOrders = 1,
        /// <summary>
        /// Check whether there are pending orders
        /// </summary>
        PendingOrders = 2,
        /// <summary>
        /// Is the account completely valuated
        /// </summary>
        IsValuated = 3,
        /// <summary>
        /// Check whether the money transfer order is finished
        /// </summary>
        CheckPendingCashOrder = 4,
        /// <summary>
        /// Check whether it is possible to cancel the instruction
        /// </summary>
        CheckCancel = 5
    }

    /// <summary>
    /// The different actions that an instruction can initiate during its lifetime
    /// </summary>
    public enum ClientDepartureInstructionActions
    {
        /// <summary>
        /// Liquidate portfolio-> create size based close orders for all positions
        /// </summary>
        SizeBasedCloseOrders = 1,
        /// <summary>
        /// Set the management end dates and change model to 'Opheffing'
        /// </summary>
        SetMgtEndDates = 2,
        /// <summary>
        /// Subtract MgtFee and create MoneyTransferOrder to pay the cash to the account
        /// </summary>
        SettleAccount = 3,
        /// <summary>
        /// Create MoneyTransferOrder to pay the cash to the account
        /// </summary>
        TransferAllCash = 6,
        /// <summary>
        /// Terminate the instruction
        /// </summary>
        Terminate = 4,
        /// <summary>
        /// Cancel the instruction
        /// </summary>
        Cancel = 5
    }

    
    public interface IClientDepartureInstruction : IInstructionTypeRebalance
    {
        IMoneyTransferOrder MoneyTransferOrder { get; set; }
        string TransferDescription { get; set; }
        ICounterAccount CounterAccount { get; set; }
    }
}
