using System;

namespace B4F.TotalGiro.Utils
{
    public interface IStateMachineClient
    {
        string CheckCondition(int conditionID);
        bool RunAction(int actionID);
    }
}
