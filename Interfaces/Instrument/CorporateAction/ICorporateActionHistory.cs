using System;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public interface ICorporateActionHistory
    {
        int Key { get; set;  }
        ISecurityInstrument Instrument { get; }
        string Description { get; set;  }
        decimal TotalNumberOfUnits { get; set; }
        DateTime CreationDate { get; }
        string DisplayString { get; }
    }
}
