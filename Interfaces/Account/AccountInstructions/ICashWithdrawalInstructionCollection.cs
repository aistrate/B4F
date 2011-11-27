using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.Instructions.CashWithdrawalInstructionCollection">CashWithdrawalInstructionCollection</see> class
    /// </summary>
    public interface ICashWithdrawalInstructionCollection : IGenericCollection<ICashWithdrawalInstruction>
    {
        Money TotalAmount { get; }
        Money TotalKeepCashAmount(ICashWithdrawalInstruction withdrawalInstruction);
        DateTime FirstWithdrawalDate { get; }
        bool Contains(DateTime date);
        int[] GetKeys();
    }
}
