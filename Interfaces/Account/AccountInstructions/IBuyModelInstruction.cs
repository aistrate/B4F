using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// The different conditions that an instruction are tested for during its lifetime
    /// </summary>
    public enum BuyModelInstructionConditions
    {
        /// <summary>
        /// Check whether there are pending (amount based) orders due to a rebalance
        /// </summary>
        EnoughBuyingPower = 1,
        /// <summary>
        /// Check whether there are pending (amount based) orders due to the buy model action
        /// </summary>
        PendingOrders = 3,
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
    public enum BuyModelInstructionActions
    {
        /// <summary>
        /// The actual buying of the mdel
        /// </summary>
        BuyModel = 2,
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


    public interface IBuyModelInstruction : IInstructionTypeRebalance
    {
        Money DepositCashPositionDifference { get; set; }
        bool GetRoundingDifference(out Money diff);
    }
}
