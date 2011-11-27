using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.CommCalculations
{
    public class CommCalcLineAmountBased : CommCalcLine
    {
        #region Constructor

        protected CommCalcLineAmountBased() { }

        /// <summary>
        /// Initializes a new instance of the <b>CommCalc</b> class.
        /// </summary>
        /// <param name="lowerRange">Lower limit of the interval over which this line is handling commission calculation.</param>
        /// <param name="staticCharge">(Not used)</param>
        /// <param name="feePercentage">Percentage on which the fee calculation for this line is based.</param>
        /// <remarks>
        /// Only the Upper range may be provided. 
        /// The Lower range must be the previous record's Upper Range for Consistency.
        /// Each Line may have a Static Charge plus a FeePercentage.
        /// </remarks>
        public CommCalcLineAmountBased(Money lowerRange, decimal staticCharge, decimal feePercentage)
			: base(staticCharge)
		{
            if (feePercentage == 0m && staticCharge == 0m)
                throw new ApplicationException("FeePercentage and/or static charge are mandatory");

            this.FeePercentage = feePercentage;
            this.LowerRange = lowerRange;
        }

        #endregion

        #region Props

        /// <summary>
        /// Gets or sets the lower limit of the interval over which this line is handling commission calculation.
        /// </summary>
        public virtual Money LowerRange
        {
            get { return this.lowerRange; }
            set { this.lowerRange = value; }
        }

        /// <summary>
        /// Gets the upper limit of the interval over which this line is handling commission calculation.
        /// </summary>
        public virtual Money UpperRange
        {
            get
            {
                if (this.upperRange == null)
                    return new Money(decimal.MaxValue, Parent.CommCurrency);
                else
                    return this.upperRange;
            }
            protected internal set { this.upperRange = value; }
        }

        /// <summary>
        /// Gets or sets the percentage on which the fee (commission) calculation for this line is based.
        /// </summary>
        public virtual decimal FeePercentage
        {
            get { return this.feePercentage; }
            set { this.feePercentage = value; }
        }

        /// <summary>
        /// Gets a string representation of the interval over which this line is handling commission calculation.
        /// </summary>
        public override string DisplayRange
        {
            get
            {
                string retVal = "";

                if (this.upperRange == null)
                    retVal = string.Format("larger than {0}", LowerRange.ToString());
                else
                    retVal = string.Format("between {0} and {1}", LowerRange.ToString(), UpperRange.ToString());
                return retVal;
            }
        }

        /// <summary>
        /// Gets a string representation of the interval over which this line is handling commission calculation.
        /// </summary>
        public override string LineDistinctives
        {
            get
            {
                string retVal = "";

                if (FeePercentage != 0M)
                    retVal = string.Format("{0}%", FeePercentage.ToString("###,##0.0000"));
                if (StaticCharge != 0M)
                    retVal += string.Format("{0}charge {1} €", retVal != "" ? " and " : "", StaticCharge.ToString("###,##0.00"));
                return retVal;
            }
        }

        public override decimal LowerRangeQuantity
        {
            get { return LowerRange.Quantity; }
        }

        public override CommCalcLineBasedTypes LineBasedType
        {
            get { return CommCalcLineBasedTypes.AmountBased; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the given value falls within this line's interval (<b>LowerRange</b> to <b>UpperRange</b>).
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns><b>true</b> if value falls within interval, <b>false</b> if not.</returns>
        public override bool Envelops(InstrumentSize val)
        {
            //return (LowerRange < val && (UpperRange == null) || (UpperRange != null && val <= UpperRange));
            return (val > LowerRange && val <= UpperRange);
        }

        /// <summary>
        /// Checks that this line's interval is under the given value.
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns><b>true</b> if line's interval is under the given value, <b>false</b> if not.</returns>
        public override bool IsUnder(InstrumentSize val)
        {
            return (UpperRange < val && UpperRange.IsNotZero);
        }

        /// <summary>
        /// Calculates fee for a given amount.
        /// </summary>
        /// <param name="val">Amount to calculate fee for.</param>
        /// <returns>The calculated fee.</returns>
        public override Money Calculate(InstrumentSize val)
        {
            if (val != null && !val.IsMoney)
                throw new ApplicationException("It is not possible to calculate the calculation for non cash.");
            
            return val.GetMoney() * (FeePercentage / 100M) + StaticChargeAmount;
        }

        /// <summary>
        /// Calculates fee for a given amount which is a sell exclusive amount order.
        /// </summary>
        /// <param name="val">Amount to calculate fee for.</param>
        /// <returns>The calculated fee.</returns>
        public virtual Money CalculateExtra(Money val)
        {
            return ((val - StaticChargeAmount) * ((FeePercentage / 100M) * (1M + FeePercentage / 100M))) + StaticChargeAmount;
        }

        /// <summary>
        /// Calculates a fee so that the fee plus the net amount (of the order) equals the gross amount.
        /// </summary>
        /// <param name="grossAmount">The gross amount.</param>
        /// <param name="fee">The calculated fee (<b>out</b> parameter).</param>
        /// <returns><b>true</b> if fee could be calculated (net amount fell on this line), <b>false</b> if not.</returns>
        public virtual bool CalculateBackwards(Money grossAmount, out Money fee)
        {
            bool result = false;
            Money nettAmount;
            Money check;

            Money comAmount = grossAmount;
            if (!comAmount.Underlying.Equals(this.Parent.CommCurrency))
                comAmount = comAmount.Convert(this.Parent.CommCurrency);

            Money fixedSetup = Parent.FixedSetup + StaticChargeAmount;
            fee = (((comAmount.Abs() - fixedSetup) * (FeePercentage / 100M)) / (1M + (FeePercentage / 100M))) + fixedSetup;
            fee = fee.Round();

            check = Parent.MinValue;
            if (check != null && check.IsNotZero && fee < check)
            {
                fee = check;
            }
            else
            {
                check = Parent.MaxValue;
                if (check != null && check.IsNotZero && fee > check)
                {
                    fee = check;
                }
            }

            if (comAmount.Sign)
            {
                nettAmount = comAmount - fee;
            }
            else
            {
                nettAmount = comAmount + fee;
            }

            // Check if within range
            if (Envelops(nettAmount.Abs()))
                result = true;
            return result;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// A string representation of this Commission Calculation Line.
        /// </summary>
        /// <returns>A string representation of this Commission Calculation Line.</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1} -> {2}%", LowerRange.ToString(), UpperRange.ToString(), FeePercentage.ToString("0.00"));
        }

        #endregion

        #region Private Variables

        private Money lowerRange;
        private Money upperRange;
        private decimal feePercentage;

        #endregion

    }
}
