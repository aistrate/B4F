using System;
using System.Collections;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This enumeration lists all possible account instruction types
    /// </summary>
    public enum InstructionTypes
    {
        /// <summary>
        /// Currently the only instruction type is a rebalance instruction
        /// </summary>
        Rebalance = 1,
        /// <summary>
        /// Use cash only to buy according to the model portfolio
        /// </summary>
        BuyModel = 2,
        /// <summary>
        /// Used for Cash withdrawals
        /// </summary>
        CashWithdrawal = 3,
        /// <summary>
        /// Used for Client Departure
        /// </summary>
        ClientDeparture = 4
    }

    ///// <summary>
    ///// This enumeration describes all possible stati during processing of an instruction
    ///// </summary>
    //public enum InstructionStati
    //{
    //    /// <summary>
    //    /// The instruction is new and not yet being processed
    //    /// </summary>
    //    New = 1,
    //    /// <summary>
    //    /// There are pending orders found, so the rebalance has to wait.
    //    /// These ordersmight be size based sell orders due to a restructure of the modelportfolio
    //    /// </summary>
    //    PendingOrders = 2,
    //    /// <summary>
    //    /// The rebalance has been kicked off
    //    /// </summary>
    //    Rebalance = 3,
    //    /// <summary>
    //    /// There are still pending orders due to the rebalance
    //    /// </summary>
    //    PendingRebalanceOrders = 4,
    //    /// <summary>
    //    /// All cash remaining (long, short) is sucked up into the cash fund
    //    /// </summary>
    //    CashFund = 5,
    //    /// <summary>
    //    /// The cash fund order is still pending
    //    /// </summary>
    //    PendingCashFundOrders = 6,
    //    /// <summary>
    //    /// The instruction is terminated (inactived)
    //    /// </summary>
    //    Terminate = 7
    //}

    /// <summary>
    /// The possible events that can happen to an instruction
    /// </summary>
    public enum InstructionEvents
    {
        /// <summary>
        /// The event that takes an instruction to the next status
        /// </summary>
        evNext = 1,
        /// <summary>
        /// The event that cancels the instruction
        /// </summary>
        evCancel = 2
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">Instruction</see> class
    /// </summary>
    public interface IInstruction : IStateMachineClient, ITotalGiroBase<IInstruction>, IAuditable
    {
		int Key { get; }
        IAccountTypeCustomer Account { get; }
		InstructionTypes InstructionType { get; }
        bool IsTypeRebalance { get; }
        int Status { get; set; }
        bool IsEditable { get; }
        bool IsActive { get; set;}
        bool Cancelled { get; }
        DateTime ExecutionDate { get; }
        DateTime ActualExecutedDate { get; set; }
        DateTime CloseDate { get; }
        DateTime CreationDate { get; }
		DateTime LastUpdated { get; }
        string Message { get; set; }
        bool Warning { get; set; }
        string DisplayInstructionType { get; }
        string DisplayStatus { get; }
        bool DoNotChargeCommission { get; }
        bool HasOrders { get; }
        int OrdersGenerated { get; }
        IInstructionOrderCollection Orders { get; }
        IInstructionOrderCollection ActiveOrders { get; }

        IInstructionEngine Engine { get; set; }
        IList UpdateableOrders { get; set; }
        void SetInstructionMessage(int result);
    }
}
