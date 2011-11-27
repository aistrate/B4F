using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.Dal.Utils;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public class ManagementPeriodUnitFee : IManagementPeriodUnitFee
    {
        /// <summary>
        /// Constructor of ManagementPeriodUnitFee object
        /// </summary>
        protected ManagementPeriodUnitFee()
        {
        }

        /// <summary>
        /// Constructor of ManagementPeriodUnitFee object
        /// </summary>
        /// <param name="calculatedAmount">The (total) value calculated</param>
        /// <param name="prevCalcAmount">The value already charged</param>
        /// <param name="feeType">The type of MgtFee</param>
        public ManagementPeriodUnitFee(IManagementPeriodUnit parent, FeeType feeType, Money amount, IFeeCalcVersion calcSource)
        {
            this.Parent = parent;
            this.FeeType = feeType;
            this.Amount = amount;

            if (calcSource != null)
                calcSourceKey = calcSource.Key; 
        }

        /// <summary>
        /// The ID
        /// </summary>
        public virtual int Key { get; set; }

        /// <summary>
        /// The unit this fee belongs to
        /// </summary>
        public virtual IManagementPeriodUnit Parent { get; internal set; }

        /// <summary>
        /// The amount of the fee calculation for the unit that will be charged to the client. (base currency)
        /// </summary>
        public virtual Money Amount { get; set; }

        /// <summary>
        /// The type of Fee.
        /// </summary>
        public virtual FeeType FeeType { get; set; }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        public void Edit(Money amount, IFeeCalcVersion calcSource)
        {
            this.Amount = amount;

            if (calcSource != null)
                calcSourceKey = calcSource.Key;
        }

        #region Override

        public override string ToString()
        {
            return String.Format("{0} {1}", FeeType.ToString(), Amount.DisplayString);
        }

        #endregion

        #region Private Variables

        private int calcSourceKey;
        private DateTime creationDate = DateTime.Now;

        #endregion

    }
}
