using System;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ManagementPeriodUnits;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public class AverageHolding : IAverageHolding
    {
        #region Constructor

        protected AverageHolding() { }

        #endregion

        #region Properties

        /// <summary>
        /// The Key of the holding.
        /// </summary>
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual IManagementPeriodUnitParent UnitParent { get; set; }

        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return this.account; }
            protected set { this.account = value; }
        }

        /// <summary>
        /// The relevant instrument
        /// </summary>
        public virtual IInstrument Instrument
        {
            get { return this.instrument; }
            protected set { this.instrument = value; }
        }

        /// <summary>
        /// The year + month concatenated as an integer
        /// </summary>
        public virtual int Period
        {
            get { return period; }
            protected set { period = value; }
        }

        /// <summary>
        /// The month of the period
        /// </summary>
        public virtual int Month
        {
            get { return Convert.ToInt32(Period.ToString().Substring(4)); }
        }

        /// <summary>
        /// The begin date of the holding
        /// </summary>
        public virtual DateTime BeginDate
        {
            get { return this.beginDate; }
            protected set { this.beginDate = value; }
        }

        /// <summary>
        /// The end date of the holding
        /// </summary>
        public virtual DateTime EndDate
        {
            get { return this.endDate; }
            protected set { this.endDate = value; }
        }

        /// <summary>
        /// The number of days that the holding was owned during the period
        /// </summary>
        public virtual short Days
        {
            get { return days; }
            protected set { days = value; }
        }
	

        /// <summary>
        /// This is the Average Value in base currency
        /// </summary>
        public virtual Money AverageValue
        {
            get { return this.avgValue; }
            protected set { this.avgValue = value; }
        }

        /// <summary>
        /// Link to the previous holding, in case something changed after a mutation
        /// </summary>
        public virtual IAverageHolding PreviousHolding
        {
            get { return prevHolding; }
            protected set { prevHolding = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        /// <summary>
        /// Should management fee be created for these average holdings (for old data)
        /// </summary>
        public virtual bool SkipFees
        {
            get { return skipFees; }
            set { skipFees = value; }
        }

        /// <summary>
        /// The record should be updated
        /// </summary>
        public virtual bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        /// <summary>
        /// The record is no longer valid, although it might be on a previous management fee nota
        /// </summary>
        public virtual bool IsInValid
        {
            get { return isInValid; }
            set { isInValid = value; }
        }

        public virtual string DisplayMessage
        {
            get
            {
                string message = string.Empty;
                if (IsInValid)
                    message += "This average holding is not valid.";
                else if (IsDirty)
                    message += "This average holding is dirty.";

                return message;
            }
        }
	
        /// <summary>
        /// The fees that belong to this item.
        /// </summary>
        public virtual IAverageHoldingFeeCollection FeeItems
        {
            get
            {
                if (feeItems == null)
                    this.feeItems = new AverageHoldingFeeCollection(this, bagOfFees);
                return feeItems;
            }
        }

        /// <summary>
        /// Does the Holding contains unsaved or updated fee items
        /// </summary>
        public bool ContainsNewFeeItems
        {
            get
            {
                bool isDirty = false;
                foreach (IAverageHoldingFee feeItem in FeeItems)
                {
                    if (feeItem.Key == 0 || feeItem.IsEditted)
                    {
                        isDirty = true;
                        break;
                    }
                }
                return isDirty;
            }
        }

        //public bool DeactivateFeeItems()
        //{
        //    bool retVal = false;
        //    foreach (IAverageHoldingFee feeItem in FeeItems)
        //    {
        //        if (feeItem.Deactivate())
        //            retVal = true;
        //    }
        //    return retVal;
        //}

        public bool IgnoreFeeItems()
        {
            bool retVal = false;
            foreach (IAverageHoldingFee feeItem in FeeItems)
            {
                feeItem.IsIgnored = true;
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// This method returns the amount that has been charged in the specific feetype so far
        /// </summary>
        /// <param name="feeType">The relevant feetype</param>
        /// <returns>the amount</returns>
        public Money GetPreviousCalculatedFee(FeeType feeType)
        {
            Money prevCalcFee = null;
            getPreviousCalculatedFeeRecursively(PreviousHolding, feeType, ref prevCalcFee);
            return prevCalcFee;
        }

        protected void getPreviousCalculatedFeeRecursively(IAverageHolding holding, FeeType feeType, ref Money prevCalcFee)
        {
            if (holding != null)
            {
                if (holding.PreviousHolding != null)
                    getPreviousCalculatedFeeRecursively(holding.PreviousHolding, feeType, ref prevCalcFee);

                if (holding != null && holding.FeeItems != null && holding.FeeItems.Count > 0)
                {
                    foreach (IAverageHoldingFee feeItem in holding.FeeItems)
                    {
                        if (!feeItem.IsIgnored && feeItem.FeeType.Equals(feeType))
                            prevCalcFee += feeItem.Amount;
                    }
                }
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (Instrument != null && Period > 0)
                return string.Format("{0}-{1}-{2}", Instrument.Name, Period.ToString(), AverageValue.DisplayString);
            else
                return base.ToString();
        }

        #endregion

        #region Privates

        private int key;
        private IAccountTypeInternal account;
        private IInstrument instrument;
        private int period;
        private DateTime beginDate;
        private DateTime endDate;
        private short days;
        private Money avgValue;
        private DateTime creationDate = DateTime.Now;
        private bool skipFees;
        private bool isDirty;
        private bool isInValid;
        private IAverageHolding prevHolding;
        private IList bagOfFees = new ArrayList();
        private IAverageHoldingFeeCollection feeItems;
        
        #endregion
    }
}
