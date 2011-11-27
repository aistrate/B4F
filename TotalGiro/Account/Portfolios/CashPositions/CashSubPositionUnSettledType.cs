using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashSubPositionUnSettledType : ICashSubPositionUnSettledType
    {
        public virtual int Key { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual bool IncludeBuyingPower { get; set; }
    }
}
