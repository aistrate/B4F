using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    public class Option : Derivative, IOption
    {
        protected Option()
        {
            initialize();
        }

        public Option(IDerivativeMaster master)
            : this()
        {
            base.Master = master;
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.Option;
        }

        public override bool Validate()
        {
            if (this.StrikePrice == null)
                throw new ApplicationException("Strike Price is mandatory.");
            if (Util.IsNullDate(ExpiryDate))
                throw new ApplicationException("Expiry Date is mandatory.");
            return base.validate();
        }

        public virtual Price StrikePrice 
        {
            get { return new Price(strikePrice.Quantity, base.CurrencyNominal, this); }
            set { strikePrice = value; }
        }
        public virtual OptionTypes OptionType { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
        public virtual string SortOrder
        {
            get
            {
                StringBuilder sb = new StringBuilder(this.ExpiryDate.Year.ToString());
                sb.Append(this.ExpiryDate.Month.ToString("D2"));
                sb.Append((Convert.ToInt32(this.strikePrice.Quantity * 100)).ToString("D8"));
                sb.Append(OptionType == OptionTypes.Call ? "C" : "P");
                return sb.ToString();
            }
        }


        #region Private Variables

        private Price strikePrice;

        #endregion

    }
}
