using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions
{
    public enum InstructionEngineChecks
    {
        CheckSizeBaseCloseOrders
    }
    
    public enum InstructionEngineActions
    {   
        PlaceSizeBaseCloseOrders,
        RunRebalance,
        BuyModel,
        PlaceCashFundOrders,
        PlaceFreeUpCashFundOrder,
        CreateFreeUpCashRebalanceInstruction,
        CreateMoneyTransferOrder,
        LiquidatePortfolio,
        SettleAccount,
        TransferAllCash,
        TerminateAccount
    }

    public interface IInstructionEngine
    {
        bool PerformCheck(InstructionEngineChecks check, IInstruction instruction);
        void PerformAction(InstructionEngineActions action, IInstruction instruction);
    }
}
