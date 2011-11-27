using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    /// <summary>
    /// Calculates a simple, constant fee.
    /// </summary>
    public class FeeCalcVersionSimple : FeeCalcVersion, IFeeCalcVersionSimple
    {
        protected FeeCalcVersionSimple() { }

        public FeeCalcVersionSimple(IFeeCalc parent, Money fixedSetup,
            int startPeriod, string createdBy)
            : base(parent, fixedSetup, startPeriod, createdBy)
        { }

        #region Properties

        /// <summary>
        /// The type of calculation
        /// </summary>
        public override FeeCalcTypes FeeCalcType
        {
            get { return FeeCalcTypes.Simple; }
        }

        public virtual bool NoFees { get; set; }

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
                if (!NoFees)
                    isRelevant = base.IsFeeRelevant;
                return isRelevant;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        #endregion

        public override void Calculate(IManagementPeriodUnit unit)
        {
            if (!NoFees && FixedSetup != null && FixedSetup.IsNotZero)
            {
                // If Per Instrument -> loop through avgHoldings
                if (Parent.FeeType.CalcBasis == FeeCalcBasis.Instrument)
                {
                    if (unit.AverageHoldings != null && unit.AverageHoldings.Count > 0)
                    {
                        foreach (IAverageHolding holding in unit.AverageHoldings)
                        {
                            if (isHoldingIncluded(holding))
                                holding.FeeItems.AddFeeItem(Parent.FeeType, FixedSetupMonthly, unit, this, 0M);
                        }
                    }
                }
                else
                {
                    // Per Account -> do not call it per holding
                    unit.FeeItems.AddFeeItem(Parent.FeeType, FixedSetupMonthly, this);
                }
            }
        }
    }
}
