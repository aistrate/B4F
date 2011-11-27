using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.BackOffice.Orders;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This enumeration describes all possible stati during processing of an instruction
    /// </summary>
    public enum CashWithdrawalInstructionStati
    {
        /// <summary>
        /// The instruction is new and not yet being processed
        /// </summary>
        New = 1,
        /// <summary>
        /// Cash Order (GLDSTD) has been created
        /// </summary>
        CashOrderOut = 2,
        /// <summary>
        /// The instruction is terminated (inactived)
        /// </summary>
        Terminate = 7
    }

    /// <summary>
    /// The different conditions that an instruction are tested for during its lifetime
    /// </summary>
    public enum CashWithdrawalInstructionConditions
    {
        /// <summary>
        /// Check whether an action should be performed.
        /// </summary>
        CheckAction = 1,
        /// <summary>
        /// Check whether an action should be performed.
        /// </summary>
        CheckPendingCashOrder = 2,
        /// <summary>
        /// Check whether it is possible to cancel the instruction
        /// </summary>
        CheckCancel = 5
    }

    /// <summary>
    /// The different actions that an instruction can initiate during its lifetime
    /// </summary>
    public enum CashWithdrawalInstructionActions
    {
        /// <summary>
        /// Do a rebalance to free up the cash before the withdrawal
        /// </summary>
        CreateFreeUpCashRebalanceInstruction = 1,
        /// <summary>
        /// Placing CashFund orders to free up the cash before the withdrawal
        /// </summary>
        PlaceFreeUpCashFundOrder = 2,
        /// <summary>
        /// Creation of the CashOrder (GLDSTD record) -> will give the cash to the account
        /// </summary>
        CreateCashOrder = 3,
        /// <summary>
        /// Terminate the instruction after the CashOrder was matched (by a GLDMTX) and the cash is taken of the account.
        /// </summary>
        Terminate = 4,
        /// <summary>
        /// Cancel the instruction
        /// </summary>
        Cancel = 5
    }

    public enum CashWithdrawalInstructionCheckCashReturnValues
    {
        PlaceCashOrder,
        PlaceCashFundOrder,
        CreateRebalanceInstruction,
        DoNothing,
        NotEnoughBuyingPower
    }

    public interface ICashWithdrawalInstruction : IInstruction
    {
        DateTime WithdrawalDate { get; }
        DateTime LatestPossibleRebalanceStartDate { get; }
        DateTime LatestPossibleFreeUpCashDate { get; }
        Money Amount { get; }
        bool IsPeriodic { get; }
        string DisplayRegularity { get; }
        ICounterAccount CounterAccount { get; }
        string Reference { get; }
        IWithdrawalRule Rule { get; }
        string TransferDescription { get; }
        IMoneyTransferOrder MoneyTransferOrder { get; set; }
        bool IsCancellable { get; }

        bool Validate();
        bool Edit(DateTime withdrawalDate, DateTime executionDate, Money amount, ICounterAccount counterAccount, string transferDescription, bool doNotChargeCommission);
        CashWithdrawalInstructionCheckCashReturnValues CheckAccountCash(CashWithdrawalInstructionActions action);
    }
}
