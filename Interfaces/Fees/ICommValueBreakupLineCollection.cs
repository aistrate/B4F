using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.Calculations
{
    public interface ICommValueBreakupLineCollection : IGenericCollection<ICommValueBreakupLine>
    {
        ICommValueDetails Parent { get; }
        ICommValueBreakupLine GetItemByType(CommValueBreakupType breakupType);
    }

}
