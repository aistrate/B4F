using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.Instructions.InstructionCollection">InstructionCollection</see> class
    /// </summary>
    public interface IInstructionCollection : IGenericCollection<IInstruction>
    {
        IAccountTypeCustomer ParentAccount { get; }
        IInstruction GetActiveInstruction { get; }
    }
}
