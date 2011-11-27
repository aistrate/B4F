using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments
{
    public class ModelDetail : IModelDetail
    {
        protected ModelDetail() { }

        #region Properties

        /// <summary>
        /// Get/set identifier
        /// </summary>
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Period that a rebalance will take in working days
        /// </summary>
        public virtual short DaysDurationRebalance { get; set; }

        /// <summary>
        /// Number of working days to keep free cash around
        /// </summary>
        public virtual short DaysKeepFreeCash { get; set; }

        /// <summary>
        /// Period that it takes to free up cash in working days
        /// </summary>
        public virtual short DaysFreeUpCash { get; set; }

        /// <summary>
        /// Model Includes CashManagementFund
        /// </summary>
        public virtual bool IncludeCashManagementFund { get; set; }

        /// <summary>
        /// The several CashManagementFund presence options
        /// </summary>
        public virtual CashManagementFundOptions CashManagementFundOption { get; set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Overridden composition of name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }

        /// <summary>
        /// Overridding Hashcode composition
        /// </summary>
        /// <returns>Hashcode</returns>
		public override int GetHashCode()
		{
			return this.Key.GetHashCode();
        }

        #endregion

        #region Private Variables

        private int key;

		#endregion

    }
}
