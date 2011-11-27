using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    abstract public class FeeCalcVersion : IFeeCalcVersion
    {
        #region Constructor

        protected FeeCalcVersion() { }

        internal FeeCalcVersion(IFeeCalc parent, Money fixedSetup, 
            int startPeriod, string createdBy)
        {
            if (parent == null)
                throw new ApplicationException("Parent calculation can not be null");
            
            Parent = parent;
            FixedSetup = fixedSetup;
            StartPeriod = startPeriod;
            CreatedBy = createdBy;

            checkCurrencies();
        }

        private void checkCurrencies()
        {
            checkCurrenciesSub(FixedSetup);
        }

        protected void checkCurrenciesSub(Money value)
        {
            if (value != null)
            {
                if (!Parent.FeeCurrency.Equals(value.Underlying))
                    throw new ApplicationException("All values should be in the same currency");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the commission calculation.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// The fee calculation this version belongs to.
        /// </summary>
	    public virtual IFeeCalc Parent
	    {
		    get { return parent;}
		    set { parent = value;}
	    }

        public abstract FeeCalcTypes FeeCalcType { get; }

	    /// <summary>
	    /// The version number of this calculation
	    /// </summary>
        public virtual int VersionNumber
	    {
		    get { return this.versionNumber;}
		    set { this.versionNumber = value;}
	    }

        /// <summary>
        /// The start date that the calculation is effective
        /// </summary>
	    public virtual int StartPeriod
	    {
		    get { return this.startPeriod;}
		    set { this.startPeriod = value;}
	    }

        /// <summary>
        /// The end date until the calculation is effective
        /// </summary>
	    public virtual int EndPeriod
	    {
		    get { return this.endPeriod;}
            set { this.endPeriod = value; }
	    }

        /// <summary>
        /// A fixed amount always added to the commission.
        /// </summary>
        public virtual Money FixedSetup
        {
            get { return this.fixedSetup; }
            set { this.fixedSetup = value; }
        }

        /// <summary>
        /// Returns the Setup charged per month including the correct sign
        /// </summary>
        public virtual Money FixedSetupMonthly
        {
            get { return this.FixedSetup/12M * Parent.FeeType.FeeTypeSign; }
        }


        /// <summary>
        /// The employee who created this calculation version
        /// </summary>
        public virtual string CreatedBy
        {
            get { return this.createdBy; }
            private set { this.createdBy = value; }
        }

        /// <summary>
        /// Date/time this version was created
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
            protected internal set { this.creationDate = value; }
        }

        /// <summary>
        /// Date/time when this order has last been updated
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        /// <summary>
        /// Holds the instruments & seccategory information of the excluded instruments
        /// </summary>
        public virtual FeeCalcExcludedInstrumentInfo ExcludedInstrumentInfo
        {
            get { return excludedInstrumentInfo; }
            set { excludedInstrumentInfo = value; }
        }

        public virtual string DisplayString
        {
            get { return this.ToString(); }
        }

        #endregion

        public abstract void Calculate(IManagementPeriodUnit unit);
        
        /// <summary>
        /// Does this calculation return any costs
        /// </summary>
        public virtual bool IsFeeRelevant 
        {
            get
            {
                bool isRelevant = false;
                if (FixedSetup!= null && FixedSetup.IsNotZero)
                    isRelevant = true;
                return isRelevant;
            }
        }

        protected bool isHoldingIncluded(IAverageHolding holding)
        {
            if (ExcludedInstrumentInfo == null)
                return true;
            else
                return !ExcludedInstrumentInfo.IsExcluded(holding);
        }

        //protected bool isPeriodRelevant(int actualPeriod, int startPeriod, int endPeriod)
        //{
        //    bool retVal = false;
        //    if (actualPeriod >= startPeriod && actualPeriod <= endPeriod)
        //        retVal = true;
        //    return retVal;
        //}

        //protected void addAmountToMgtFee(MgtFee fee, Money amount)
        //{
        //    if (amount != null && amount.IsNotZero)
        //    {
        //        MgtFeeBreakupLine breakup = fee.BreakupLines.GetItemByType(Parent.FeeType.MainFeeType);
        //        if (breakup == null)
        //        {
        //            breakup = new MgtFeeBreakupLine(Parent.FeeType.MainFeeType, amount);
        //            fee.BreakupLines.Add(breakup);
        //        }
        //        else
        //            breakup.Amount += amount;
        //    }
        //}

        #region Override

        public override string ToString()
        {
            return Parent.Name + " v." + VersionNumber.ToString("000");
        }

        /// <summary>
        /// Hash function for this type. 
        /// </summary>
        /// <returns>A hash code for the current FeeCalc object.</returns>
        public override int GetHashCode()
        {
            return this.key.GetHashCode();
        }

        #endregion

        #region Private Variables

        private int key;
        private IFeeCalc parent;
        private int versionNumber;
        private int startPeriod;
        private int endPeriod;
        private Money fixedSetup;
        private string createdBy;
        private DateTime creationDate = DateTime.Now;
        private DateTime lastUpdated;
        private FeeCalcExcludedInstrumentInfo excludedInstrumentInfo;

        #endregion

    }
}
