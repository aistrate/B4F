using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public class FeeCalcExcludedInstrumentInfo
    {
        /// <summary>
        /// Gets or sets the unique ID of the info
        /// </summary>
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Description of the exclusions
        /// </summary>
        public virtual string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Date/time this info was created
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
            protected internal set { this.creationDate = value; }
        }

        /// <summary>
        /// The instrument info's that are excluded from the calculation
        /// </summary>
        public virtual FeeCalcExcludedInstrumentInfoDetailCollection ExcludedInstrumentInfoDetails
        {
            get
            {
                if (this.excludedInstrumentInfoDetails == null)
                    this.excludedInstrumentInfoDetails = new FeeCalcExcludedInstrumentInfoDetailCollection(bagOfExclusions, this);
                return this.excludedInstrumentInfoDetails;
            }
        }

        public virtual bool IsExcluded(IAverageHolding holding)
        {
            if (ExcludedInstrumentInfoDetails != null)
            {
                foreach (FeeCalcExcludedInstrumentInfoDetail detail in ExcludedInstrumentInfoDetails)
                {
                    if (detail.IsExcluded(holding))
                        return true;
                }
            }
            return false;
        }

        #region Private Variables

        private int key;
        private string description;
        private DateTime creationDate = DateTime.Now;
        private IList bagOfExclusions = new ArrayList();
        private FeeCalcExcludedInstrumentInfoDetailCollection excludedInstrumentInfoDetails;

        #endregion

    }
}
