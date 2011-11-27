using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Fees.CommCalculations
{
    public enum CommCalcLineBasedTypes
    {
        AmountBased = 1,
        SizeBased = 2
    }
    
    /// <summary>
	/// A calculation line belonging to a commission calculation.
	/// </summary>
    public abstract class CommCalcLine
    {
        #region Constructor

        internal CommCalcLine() { }

        /// <summary>
        /// Initializes a new instance of the <b>CommCalc</b> class.
        /// </summary>
        /// <param name="staticCharge">(Not used)</param>
        /// <param name="feePercentage">Percentage on which the fee calculation for this line is based.</param>
        /// <remarks>
        /// Each Line may have a Static Charge plus a FeePercentage.
        /// </remarks>
        public CommCalcLine(decimal staticCharge)
		{
			this.StaticCharge = staticCharge;
        }

        #endregion

        #region Props

        public virtual int Key
        {
            get { return this.key; }
        }

        protected internal virtual CommCalc Parent
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
			protected internal set { this.serialNo = value; }
		}

        /// <summary>
        /// Gets or sets a constant amount that represents the commission for this line's interval.
        /// </summary>
        public virtual decimal StaticCharge
        {
            get { return staticCharge; }
            set { staticCharge = value; }
        }

        /// <summary>
        /// Gets or sets a constant amount that represents the commission for this line's interval.
        /// </summary>
        public virtual Money StaticChargeAmount
        {
            get { return new Money(staticCharge, Parent.CommCurrency); }
        }

        /// <summary>
        /// Gets a string representation of the interval over which this line is handling commission calculation.
        /// </summary>
        public abstract string LineDistinctives { get; }

        /// <summary>
        /// Gets a string representation of the interval over which this line is handling commission calculation.
        /// </summary>
        public abstract string DisplayRange { get; }
        public abstract decimal LowerRangeQuantity { get; }
        public abstract CommCalcLineBasedTypes LineBasedType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the given value falls within this line's interval (<b>LowerRange</b> to <b>UpperRange</b>).
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns><b>true</b> if value falls within interval, <b>false</b> if not.</returns>
        public abstract bool Envelops(InstrumentSize val);

        /// <summary>
        /// Checks that this line's interval is under the given value.
        /// </summary>
        /// <param name="val">Value to check.</param>
        /// <returns><b>true</b> if line's interval is under the given value, <b>false</b> if not.</returns>
        public abstract bool IsUnder(InstrumentSize val);

        /// <summary>
        /// Calculates fee for a given amount.
        /// </summary>
        /// <param name="val">Amount to calculate fee for.</param>
        /// <returns>The calculated fee.</returns>
        public abstract Money Calculate(InstrumentSize val);

        #endregion

		#region Private Variables

		private CommCalc parent;
		private int key;
		private short serialNo;
		private decimal staticCharge;

		#endregion

	}
}
