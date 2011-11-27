using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashSubPositionUnSettled : CashSubPosition, ICashSubPositionUnSettled
    {
        protected CashSubPositionUnSettled() { }

        public CashSubPositionUnSettled(ICashPosition parentPosition, ICashSubPositionUnSettledType unSettledType)
            : base(parentPosition)
        {
            UnSettledType = unSettledType;
        }

        public override CashPositionSettleStatus SettledFlag
        {
            get { return CashPositionSettleStatus.UnSettled; }
        }

        public virtual ICashSubPositionUnSettledType UnSettledType { get; set; }
    }
}
