using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public interface IAverageHoldingCollection: IList<IAverageHolding>
    {
        IManagementPeriodUnitParent Parent { get; }
    }
}
