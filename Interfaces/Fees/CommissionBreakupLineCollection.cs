using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Fees
{
    public class CommissionBreakupLineCollection : GenericCollection<CommissionBreakupLine>
    {
        internal CommissionBreakupLineCollection(Commission parent, IList bagOfLines)
            : base(bagOfLines)
        {
            this.parent = parent;
        }

        public CommissionBreakupLine GetItemByType(CommissionBreakupTypes commissionType)
        {
            foreach (CommissionBreakupLine line in this)
            {
                if (line.CommissionType == commissionType)
                    return line;
            }
            return null;
        }

        public Commission Parent
        {
            get { return parent; }
        }

        #region Override

        public override void Add(CommissionBreakupLine item)
        {
            base.Add(item);
            item.Parent = Parent;
            Parent.Amount += item.Amount;
        }

        public override string ToString()
        {
            return String.Format("{0} lines", Count.ToString());
        }

        #endregion

        #region Private Variables

        private Commission parent;

        #endregion
    }

}
