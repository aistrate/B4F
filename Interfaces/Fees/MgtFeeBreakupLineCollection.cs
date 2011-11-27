using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.Fees
{
    public class MgtFeeBreakupLineCollection : GenericCollection<MgtFeeBreakupLine>
    {
        internal MgtFeeBreakupLineCollection(MgtFee parent, IList bagOfFees)
            : base(bagOfFees)
        {
            this.parent = parent;
        }

        public MgtFeeBreakupLine GetItemByType(FeeType feeType)
        {
            foreach (MgtFeeBreakupLine line in this)
            {
                if (line.MgtFeeType == feeType)
                    return line;
            }
            return null;
        }

        public MgtFee Parent
        {
            get { return parent; }
        }

        #region Override

        public override void Add(MgtFeeBreakupLine item)
        {
            base.Add(item);
            item.Parent = Parent;
        }

        public override bool Remove(MgtFeeBreakupLine item)
        {
            foreach (IAverageHoldingFee feeItem in item.FeeItems)
                feeItem.Parent.IgnoreFeeItems();
            item.FeeItems.Clear();
            return base.Remove(item);
        }

        public override string ToString()
        {
            return String.Format("{0} lines", Count.ToString());
        }

        #endregion

        #region Private Variables

        private MgtFee parent;

        #endregion
    }
}
