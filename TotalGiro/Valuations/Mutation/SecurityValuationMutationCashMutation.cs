using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Valuations
{
    public class SecurityValuationMutationCashMutation : ISecurityValuationMutationCashMutation
    {
        protected SecurityValuationMutationCashMutation () { }

        #region ValuationCash Props

        public virtual long Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual IValuationCashMutation CashMutation
        {
            get { return cashMutation; }
            protected set { cashMutation = value; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (CashMutation == null)
                return base.ToString();
            else
                return string.Format("{0} {1}", CashMutation.ValuationCashType.ToString(), CashMutation.Amount.DisplayString);
        }

        #endregion

        #region Privates

        private long key;
        private IValuationCashMutation cashMutation;

        #endregion

    }
}
