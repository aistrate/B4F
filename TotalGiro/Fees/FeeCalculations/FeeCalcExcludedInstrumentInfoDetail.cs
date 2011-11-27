using System;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public class FeeCalcExcludedInstrumentInfoDetail
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
        /// The parent that the exceptions belong to.
        /// </summary>
        public virtual FeeCalcExcludedInstrumentInfo Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Gets or sets the <b>SecCategory</b> attached to this info; 
        /// </summary>
        public virtual ISecCategory SecCategory
        {
            get { return secCategory; }
            set { secCategory = value; }
        }

        /// <summary>
        /// Gets or sets the <b>Instrument</b> attached to this info; 
        /// </summary>
        public virtual IInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        public virtual SignValues SignValue
        {
            get { return signValue; }
            set { signValue = value; }
        }

        public virtual bool IsExcluded(IAverageHolding holding)
        {
            bool excluded = false;
            if ((Instrument != null && holding.Instrument.Equals(Instrument)) ||
                (SecCategory != null && holding.Instrument.SecCategory.Key.Equals(SecCategory.Key)))
            {
                if (SignValue == SignValues.All)
                    excluded = true;
                else if (SignValue == SignValues.Negative && !holding.AverageValue.Sign)
                    excluded = true;
                else if (SignValue == SignValues.Positive && holding.AverageValue.Sign)
                    excluded = true;
            }
            return excluded;
        }


        #region Private Variables

        private int key;
        private FeeCalcExcludedInstrumentInfo parent;
        private ISecCategory secCategory;
        private IInstrument instrument;
        private SignValues signValue = SignValues.All;

        #endregion

    }
}
