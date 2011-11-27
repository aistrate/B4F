using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Instruments.History
{
    public interface IInstrumentsHistoryConversion : IInstrumentHistory
    {
        IInstrument NewInstrument { get; }
        decimal OldChildRatio { get; set;  }
        byte NewParentRatio { get; set; }
        bool IsSpinOff { get; set; }
        decimal ConversionRate { get; }
        IInstrumentConversionCollection Conversions { get; }
    }
}
