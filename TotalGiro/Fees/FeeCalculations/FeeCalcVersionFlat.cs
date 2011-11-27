using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    /// <summary>
    /// Calculates a flat fee.
    /// </summary>
    public class FeeCalcVersionFlat : FeeCalcVersion, IFeeCalcVersionFlat
    {
        #region Constructor

        protected FeeCalcVersionFlat() { }

        public FeeCalcVersionFlat(IFeeCalc parent, Money fixedSetup,
            Money minValue, Money maxValue, int startPeriod, string createdBy)
            : base(parent, fixedSetup, startPeriod, createdBy) 
        {
            MinValue = minValue;
            MaxValue = maxValue;

            checkCurrencies();
        }

        protected virtual void checkCurrencies()
        {
            checkCurrenciesSub(MinValue);
            checkCurrenciesSub(MaxValue);

            if (MinValue != null && MinValue.IsNotZero && MaxValue != null && MaxValue.IsNotZero)
            {
                if (MinValue > MaxValue)
                    throw new ApplicationException("Minimum Value can not be more than Maximum Value");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of calculation
        /// </summary>
        public override FeeCalcTypes FeeCalcType
        {
            get { return FeeCalcTypes.Flat; }
        }

        /// <summary>
        /// The minimum value of the calculation.
        /// </summary>
        public virtual Money MinValue
        {
            get { return this.minValue; }
            set { this.minValue = value; }
        }

        /// <summary>
        /// The maximum value of the calculation.
        /// </summary>
        public virtual Money MaxValue
        {
            get { return this.maxValue; }
            set { this.maxValue = value; }
        }

        /// <summary>
        /// The minimum value of the calculation per month in the correct sign.
        /// </summary>
        public virtual Money MinValueMonthly
        {
            get { return MinValue / 12M * Parent.FeeType.FeeTypeSign; }
        }

        /// <summary>
        /// The maximum value of the calculation per month in the correct sign.
        /// </summary>
        public virtual Money MaxValueMonthly
        {
            get { return MaxValue / 12M * Parent.FeeType.FeeTypeSign;  }
        }

        /// <summary>
        /// A collection of child <b>FeeCalcLines</b> objects belonging to the Commission Calculation.
        /// </summary>
        public virtual IFeeCalcLineCollection FeeLines
        {
            get
            {
                if (this.feeLines == null)
                {
                    this.feeLines = new FeeCalcLineCollection(this, lines);
                    this.feeLines.ArrangeLines();
                }
                return feeLines;
            }
        }

        #endregion

        #region Methods

        public override void Calculate(IManagementPeriodUnit unit)
        {
            // If Per Instrument -> loop through avgHoldings
            if (Parent.FeeType.CalcBasis == FeeCalcBasis.Instrument)
            {
                if (unit.AverageHoldings != null && unit.AverageHoldings.Count > 0)
                {
                    foreach (IAverageHolding holding in unit.AverageHoldings)
                    {
                        if (isHoldingIncluded(holding))
                        {
                            Money calcFee = null;
                            Money avgAmount = holding.AverageValue;
                            decimal feePercentageUsed = 0M;

                            if (avgAmount != null && avgAmount.IsNotZero)
                            {
                                if (!avgAmount.Underlying.Equals(Parent.FeeCurrency))
                                    throw new ApplicationException("The average value is not in the same currency as the fee currency");

                                FeeLines.ArrangeLines();

                                foreach (FeeCalcLine line in FeeLines)
                                {
                                    if (line.Envelops(avgAmount))
                                    {
                                        feePercentageUsed = line.FeePercentage;
                                        calcFee = addFixMinMax(line.Calculate(avgAmount, unit.Days));
                                        break;
                                    }
                                }
                            }

                            if (calcFee != null && calcFee.IsNotZero)
                                holding.FeeItems.AddFeeItem(Parent.FeeType, calcFee, unit, this, feePercentageUsed);
                        }
                    }
                }
            }
            else
            {
                // Per Account -> do not call it per holding
                Money fee = addFixMinMax(null);
                if (fee != null && fee.IsNotZero)
                    unit.FeeItems.AddFeeItem(Parent.FeeType, fee, this);
            }
        }

        private Money addFixMinMax(Money fee)
        {
            // Add Fixed setup
            if (FixedSetup != null && FixedSetup.IsNotZero)
            {
                fee += FixedSetupMonthly;
            }

            // Check Minimum Value
            if (MinValue != null && MinValue.IsNotZero && fee.Abs() < MinValueMonthly.Abs())
                fee = MinValueMonthly;

            // Check Maximum Value
            if (MaxValue != null && MaxValue.IsNotZero && fee.Abs() > MaxValueMonthly.Abs())
                fee = MaxValueMonthly;
            return fee;
        }

        #endregion

        #region Override

        /// <summary>
        /// Does this calculation return any costs
        /// </summary>
        public override bool IsFeeRelevant
        {
            get
            {
                bool isRelevant = false;
                if (FeeLines != null && FeeLines.Count > 0 && FeeLines[0].FeePercentage != 0M)
                    isRelevant = true;
                else
                    isRelevant = base.IsFeeRelevant;
                return isRelevant;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        #endregion

        #region Private Variables

        private Money minValue;
        private Money maxValue;
        private IList lines = new ArrayList();
        private IFeeCalcLineCollection feeLines;

        #endregion
    
    }
}
