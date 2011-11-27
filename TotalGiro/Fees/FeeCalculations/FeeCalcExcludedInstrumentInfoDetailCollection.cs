using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    /// <summary>
    /// This class holds collection of instrument details that are excluded for some calculations.
    /// </summary>
    public class FeeCalcExcludedInstrumentInfoDetailCollection : GenericCollection<FeeCalcExcludedInstrumentInfoDetail>
    {
        internal FeeCalcExcludedInstrumentInfoDetailCollection(IList exclusions, FeeCalcExcludedInstrumentInfo parent)
            : base(exclusions)
        {
            this.parent = parent;
        }

        /// <summary>
        /// The parent calculation
        /// </summary>
        public FeeCalcExcludedInstrumentInfo Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        public override void Add(FeeCalcExcludedInstrumentInfoDetail item)
        {
            base.Add(item);
            item.Parent = Parent;
        }

        #region Private Variables

        private FeeCalcExcludedInstrumentInfo parent;

        #endregion
    }
}
