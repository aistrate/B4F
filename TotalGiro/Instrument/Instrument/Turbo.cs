using System;
using B4F.TotalGiro.Accounts.Portfolios;

namespace B4F.TotalGiro.Instruments
{
    public class Turbo : Derivative, ITurbo
    {
        protected Turbo()
        {
            initialize();
        }

        public Turbo(IDerivativeMaster master)
            : this()
        {
            base.Master = master;
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.Turbo;
        }

        public override bool Validate()
        {
            if (this.StopLoss == null)
                throw new ApplicationException("Stop Loss is mandatory.");
            if (!(Sign == IsLong.Long || Sign == IsLong.Short))
                throw new ApplicationException("Long/short sign is mandatory.");
            if (!(Ratio > 0))
                throw new ApplicationException("Ratio is mandatory.");
            return base.validate();
        }

        public virtual IsLong Sign { get; set; }
        public virtual short Ratio { get; set; }
        public virtual decimal Leverage { get; set; }
        public virtual decimal FinanceLevel { get; set; }
        public virtual Price StopLoss 
        {
            get { return new Price(stopLoss.Quantity, base.CurrencyNominal, this); }
            set { stopLoss = value; }
        }

        public virtual string DisplayRatio 
        {
            get { return string.Format("1:{0}", Ratio); }
        }

        #region Private Variables

        private Price stopLoss;

        #endregion

    }
}