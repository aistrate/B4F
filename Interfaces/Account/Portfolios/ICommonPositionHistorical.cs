using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios
{
    public interface ICommonPositionHistorical
    {
        IAccountTypeInternal Account { get; }
        Money HistoricalValue { get; }
        Money HistoricalBaseValue { get; }
        IInstrument PositionInstrument { get;  }
        int Key { get;  }
    }
}
