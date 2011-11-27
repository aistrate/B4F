using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashSubPositionSettled : CashSubPosition,  ICashSubPositionSettled
    {
        protected CashSubPositionSettled() { }
        
        public CashSubPositionSettled(ICashPosition parentPosition)
            : base(parentPosition)
        {
        }

        public override CashPositionSettleStatus SettledFlag
        {
            get { return CashPositionSettleStatus.Settled; }
        }
    }
}
