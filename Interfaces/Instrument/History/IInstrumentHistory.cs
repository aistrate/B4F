using System;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.History
{
    public interface IInstrumentHistory
    {
        int Key { get; set; }
        IInstrument Instrument { get; }
        DateTime ChangeDate { get; set;  }
        CorporateActionTypes CorporateActionType { get; }
        string Description { get; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; }
        bool IsInitialised { get; set; }
        bool IsExecuted { get; set; }
        DateTime ExecutionDate { get; set; }
        //ICorporateActionCollection CorporateActions { get; }

    }
}
