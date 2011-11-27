using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public interface IDerivative: ITradeableInstrument
    {
        IDerivativeMaster Master { get; set; }
    }
}
