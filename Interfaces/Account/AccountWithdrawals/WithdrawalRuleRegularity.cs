using System;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Withdrawals
{
    public class WithdrawalRuleRegularity
    {
        public virtual Regularities Key
        {
            get { return (Regularities)this.key; }
            internal set { this.key = (int)value; }
        }

        public virtual string Regularity { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTimeUnit DateUnitInclCashFund { get; set; }
        public virtual DateTimeUnit DateUnitExclCashFund { get; set; }
        public virtual DateTimeUnit DateUnitCashFundOnly { get; set; }

        #region Private Variables

        internal int key;

        #endregion
    }
}
