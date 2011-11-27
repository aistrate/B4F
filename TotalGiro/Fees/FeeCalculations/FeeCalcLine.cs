using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    /// <summary>
    /// A calculation line belonging to a commission calculation.
    /// </summary>
    public class FeeCalcLine : IFeeCalcLine
    {
        internal FeeCalcLine() { }

        /// <summary>
        /// Initializes a new instance of the <b>FeeCalc</b> class.
        /// </summary>
        /// <param name="LowerRange">Lower limit of the interval over which this line is handling commission calculation.</param>
        /// <param name="StaticCharge">(Not used)</param>
        /// <param name="FeePercentage">Percentage on which the fee calculation for this line is based.</param>
        /// <remarks>
        /// Only the Upper range may be provided. 
        /// The Lower range must be the previous record's Upper Range for Consistency.
        /// Each Line may have a Static Charge plus a FeePercentage.
        /// </remarks>
        public FeeCalcLine(Money lowerRange, Money staticCharge, decimal feePercentage)
        {
            if ((feePercentage == 0m) && (staticCharge == null || staticCharge.IsZero))
            {
                throw new ApplicationException("FeePercentage can not be 0");
            }

            this.LowerRange = lowerRange;
            this.FeePercentage = feePercentage;
            this.StaticCharge = staticCharge;
        }

        /// <summary>
        /// The ID of the fee calculation line.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual IFeeCalcVersion Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// Gets this line's index inside the (ordered) collection of Commission Calculation Lines.
        /// </summary>
        public virtual short SerialNo
        {
            get { return this.serialNo; }
            set { this.serialNo = value; }
        }

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
                    return new Money(decimal.MaxValue, Parent.Parent.FeeCurrency);
                else
                    return this.upperRange;
            }
            set { this.upperRange = value; }
        }

        /// <summary>
        /// Gets a string representation of the Upper range.
        /// </summary>
        public virtual string DisplayUpperRange
        {
            get
            {
                string retVal = "";
                if (UpperRange != null)
                {
                    string p = "&#8734";
                    retVal = (UpperRange.Quantity == decimal.MaxValue ? p : UpperRange.Quantity.ToString());
                }
                return retVal;
            }
        }

        /// <summary>
        /// Gets a string representation of the interval over which this line is handling commission calculation.
        /// </summary>
        public virtual string DisplayRange
        {
            get
            {
                string retVal = "";

                if (this.upperRange == null)
                    retVal = string.Format("larger than {0}", LowerRange.ToString());
                else
                    retVal = string.Format("between {0} and {1}", LowerRange.ToString(), DisplayUpperRange);
                return retVal;
            }
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
        /// Gets or sets a constant amount that represents the commission for this line's interval.
        /// </summary>
        public virtual Money StaticCharge
        {
            get { return staticCharge; }
            set { staticCharge = value; }
        }

        /// <summary>
        /// Checks if the given value falls within this line's interval (<b>LowerRange</b> to <b>UpperRange</b>).
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns><b>true</b> if value falls within interval, <b>false</b> if not.</returns>
        public virtual bool Envelops(Money val)
        {
            return (LowerRange < val.Abs() && (UpperRange == null) || (UpperRange != null && val.Abs() <= UpperRange));
        }

        /// <summary>
        /// Checks that this line's interval is under the given value.
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns><b>true</b> if line's interval is under the given value, <b>false</b> if not.</returns>
        public virtual bool IsUnder(Money val)
        {
            return (UpperRange < val.Abs() && UpperRange.IsNotZero);
        }

        /// <summary>
        /// Calculates fee for a given amount for the month.
        /// </summary>
        /// <param name="val">Amount to calculate fee for.</param>
        /// <returns>The calculated fee.</returns>
        public virtual Money Calculate(Money val, decimal days)
        {
            return val * ((FeePercentage / 100M) * (days / 365M) * Parent.Parent.FeeType.FeeTypeSign) + (StaticCharge * Parent.Parent.FeeType.FeeTypeSign);
        }


        #region Overrides

        /// <summary>
        /// A string representation of this Commission Calculation Line.
        /// </summary>
        /// <returns>A string representation of this Commission Calculation Line.</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1} -> {2}%", LowerRange.ToString(), DisplayUpperRange, FeePercentage.ToString("0.00"));
        }

        /// <summary>
        /// Hash function for this type.
        /// </summary>
        /// <returns>A hash code for the current FeeCalcLine object.</returns>
        public override int GetHashCode()
        {
            return this.LowerRange.GetHashCode();
        }

        #endregion

        #region Private Variables

        private int key;
        private IFeeCalcVersion parent;
        private decimal feePercentage;
        private Money lowerRange;
        private Money upperRange;
        private short serialNo;
        private Money staticCharge;

        #endregion

    }
}