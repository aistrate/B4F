using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public class StockDividend : InstrumentCorporateAction, IStockDividend
    {
        public StockDividend()
        {
            initialize();
        }

        public StockDividend(ISecurityInstrument underlying, string isin)
            : base(underlying)
        {
            this.Name = underlying.Name + " Div.";
            if (!string.IsNullOrEmpty(isin))
                this.Isin = isin;
            initialize();
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.StockDividend;
        }

        #region Overrides

        public override bool IsStockDividend
        {
            get { return true; }
        }

        public override string Isin
        {
            get 
            { 
                if (string.IsNullOrEmpty(base.Isin))
                    return Underlying.Isin; 
                else
                    return base.Isin; 
            }
            set { base.Isin = value; }
        }

        #endregion

        #region Methods


        #endregion
    }
}
