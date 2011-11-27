using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.Fees.CommCalculations
{
    public class CommCalcLineSizeBased : CommCalcLine
    {
        #region Constructor

        protected CommCalcLineSizeBased() { }

        /// <summary>
        /// Initializes a new instance of the <b>CommCalc</b> class.
        /// </summary>
        /// <param name="lowerRange">Lower limit of the interval over which this line is handling commission calculation.</param>
        /// <param name="staticCharge">(Not used)</param>
        /// <param name="tariff">Percentage on which the fee calculation for this line is based.</param>
        /// <remarks>
        /// Only the Upper range may be provided. 
        /// The Lower range must be the previous record's Upper Range for Consistency.
        /// Each Line may have a Static Charge plus a FeePercentage.
        /// </remarks>
        public CommCalcLineSizeBased(decimal lowerRange, decimal staticCharge, Money tariff)
			: base(staticCharge)
		{
            if ((tariff == null || tariff.IsZero) && staticCharge == 0m)
                throw new ApplicationException("Tariff and/or static charge are mandatory");
            
            this.Tariff = tariff;
            this.LowerRange = lowerRange;
        }

        #endregion

        #region Props

        /// <summary>
        /// Gets or sets the lower limit of the interval over which this line is handling commission calculation.
        /// </summary>
        public virtual decimal LowerRange
        {
            get { return this.lowerRange; }
            set { this.lowerRange = value; }
        }

        /// <summary>
        /// Gets the upper limit of the interval over which this line is handling commission calculation.
        /// </summary>
        public virtual decimal UpperRange
        {
            get
            {
                if (this.upperRange == 0M)
                    return decimal.MaxValue;
                else
                    return this.upperRange;
            }
            protected internal set { this.upperRange = value; }
        }

        public virtual Money Tariff { get; set; }

        /// <summary>
        /// Gets a string representation of the interval over which this line is handling commission calculation.
        /// </summary>
        public override string DisplayRange
        {
            get
            {
                string retVal = "";

                if (this.upperRange == 0M)
                    retVal = string.Format("larger than {0}", LowerRange.ToString("###,##0.00"));
                else
                    retVal = string.Format("between {0} and {1}", LowerRange.ToString("###,##0.00"), UpperRange.ToString("###,##0.00"));
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

                if (Tariff != null && Tariff.IsNotZero)
                    retVal = string.Format("tariff {0} per inst", Tariff.DisplayString);
                if (StaticCharge != 0M)
                    retVal += string.Format("{0}charge {1} €", retVal != "" ? " and " : "", StaticCharge.ToString("###,##0.00"));
                return retVal;
            }
        }

        public override decimal LowerRangeQuantity
        {
            get { return LowerRange; }
        }

        public override CommCalcLineBasedTypes LineBasedType
        {
            get { return CommCalcLineBasedTypes.SizeBased; }
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
            if (val == null)
                throw new ApplicationException("Size can not be null.");

            return (LowerRange < val.Quantity && (UpperRange == 0M) || (UpperRange != 0M && val.Quantity <= UpperRange));
        }

        /// <summary>
        /// Checks that this line's interval is under the given value.
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns><b>true</b> if line's interval is under the given value, <b>false</b> if not.</returns>
        public override bool IsUnder(InstrumentSize val)
        {
            if (val == null)
                throw new ApplicationException("Size can not be null.");
            return (UpperRange < val.Quantity && UpperRange != 0);
        }

        /// <summary>
        /// Calculates fee for a given amount.
        /// </summary>
        /// <param name="val">Amount to calculate fee for.</param>
        /// <returns>The calculated fee.</returns>
        public override Money Calculate(InstrumentSize val)
        {
            if (val == null)
                throw new ApplicationException("Size can not be null.");

            Money commission = StaticChargeAmount;
            if (Tariff != null && Tariff.IsNotZero)
                commission += (Tariff * val.Quantity);

            return commission;
        }

        /// <summary>
        /// Calculates a fee so that the fee plus the net amount (of the order) equals the gross amount.
        /// </summary>
        /// <param name="grossAmount">The gross amount.</param>
        /// <param name="side">The side of the order/transaction.</param>
        /// <param name="price">The price of the order/transaction.</param>
        /// <param name="fee">The calculated fee (<b>out</b> parameter).</param>
        /// <returns><b>true</b> if fee could be calculated (net amount fell on this line), <b>false</b> if not.</returns>
        public virtual bool CalculateBackwards(Money grossAmount, Price price, Side side, out Tuple<InstrumentSize, Money> result)
        {
            bool success = false;

            if (price == null)
                throw new ApplicationException("It is not possible to calculate the commission when there is no current price.");

            decimal sideFactor = (side == Side.Buy ? 1M : -1M);
            Money fixedSetup = Parent.FixedSetup + StaticChargeAmount;
            Money nettAmount = (grossAmount.Abs() - (fixedSetup * sideFactor)) / ((Tariff.CalculateSize(price).Quantity + sideFactor));

            Money fee = grossAmount.Abs() - (nettAmount * sideFactor);
            InstrumentSize size = nettAmount.CalculateSize(price);

            result = new Tuple<InstrumentSize, Money>(size, fee);

            // Check if within range
            if (Envelops(size.Abs()))
                success = true;
            return success;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// A string representation of this Commission Calculation Line.
        /// </summary>
        /// <returns>A string representation of this Commission Calculation Line.</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1} -> {2}%", LowerRange.ToString("###,##0.00"), UpperRange.ToString("###,##0.00"), Tariff.DisplayString);
        }

        #endregion

        #region Private Variables

        private decimal lowerRange;
        private decimal upperRange;

        #endregion

    }
}
