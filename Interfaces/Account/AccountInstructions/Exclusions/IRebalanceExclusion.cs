using System;

namespace B4F.TotalGiro.Accounts.Instructions.Exclusions
{
    public interface IRebalanceExclusion
    {
        int Key { get; set; }
        IRebalanceInstruction Parent { get; set; }
        int ComponentKey { get; }
        string ComponentName { get; }
        B4F.TotalGiro.Instruments.ModelComponentType ComponentType { get; }
        DateTime CreationDate { get; }
    }
}
