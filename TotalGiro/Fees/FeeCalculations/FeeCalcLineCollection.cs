using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using NHibernate.Collection;
using System.Linq;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    /// <summary>
    /// A list of commission calculation lines (<b>FeeCalcLine</b> objects).
    /// </summary>
    public class FeeCalcLineCollection : GenericCollection<IFeeCalcLine>, IFeeCalcLineCollection
    {
        private FeeCalcLineCollection() { }

        /// <summary>
        /// Initializes a new instance of the <b>FeeCalcLines</b> class.
        /// </summary>
        /// <param name="Parent">The parent <b>FeeCalc</b> object.</param>
        /// <param name="Lines">A list of <b>FeeCalcLine</b> objects to initialize this <b>FeeCalcLines</b> object with.</param>
        internal FeeCalcLineCollection(IFeeCalcVersion parent, IList lines)
            : base(lines)
        {
            this.parent = parent;
        }

        public IFeeCalcVersion Parent
        {
            get { return parent; }
        }

        public IFeeCalcLine GetItemBySerialNo(int serialNo)
        {
            IFeeCalcLine line = null;
            if (Count > 0)
            {
                ArrangeLines();
                foreach (IFeeCalcLine item in this)
                {
                    if (item.SerialNo == serialNo)
                        line = item;
                }
            }
            return line;
        }

        #region Overrides

        /// <summary>
        /// Adds a line to the collection.
        /// </summary>
        /// <param name="item">The line to add to the collection.</param>
        public override void Add(IFeeCalcLine item)
        {
            if (Count == 0)
            {
                item.LowerRange = new Money(0M, Parent.Parent.FeeCurrency);
            }
            base.Add(item);
            item.Parent = Parent;
            ArrangeLines();
        }

        public override bool Remove(IFeeCalcLine item)
        {
            item.Parent = null;
            bool success = base.Remove(item);
            if (success && Count > 1)
                ArrangeLines();
            return success;
        }

        public override void Insert(int index, IFeeCalcLine item)
        {
            Add(item);
        }

        public override void RemoveAt(int index)
        {
            if (index >= 0 && index < this.Count)
                Remove(this[index]);
        }

        /// <summary>
        /// Orders the calFeeCalcLines and fills the range attributes
        /// </summary>
        public void ArrangeLines()
        {
            if (this.Count > 1)
            {
                List<IFeeCalcLine> entries = base.bagCollection.Cast<IFeeCalcLine>().ToList();
                entries.Sort(new FeeCalcLineCollection.MySorter());
                for (short i = 0; i < Count; i++)
                {
                    entries[i].SerialNo = i;
                    if (i < Count - 1)
                        entries[i].UpperRange = entries[i + 1].LowerRange;
                }
            }
        }

        /// <summary>
        /// Removes all lines from the collection.
        /// </summary>
        public override void Clear()
        {
            if (Count > 0)
                base.Clear();
        }

        protected override object GetDefaultSortValue(IFeeCalcLine item)
        {
            return item.LowerRange;
        }
        
        #endregion

        #region Private Variables

        private IFeeCalcVersion parent;

        #endregion

        private class MySorter : IComparer<IFeeCalcLine>, IComparer
        {
            public MySorter() { }
            
            public MySorter(bool descending)
            {
                this.descending = descending;
            }

            public int Compare(IFeeCalcLine x, IFeeCalcLine y)
            {
                int result = 0;
                if (x == null)
                    result = -1;
                else if (y == null)
                    result = 1;
                else
                {
                    decimal xRange = x.LowerRange == null ? 0M : x.LowerRange.Quantity;
                    decimal yRange = y.LowerRange == null ? 0M : y.LowerRange.Quantity;
                    result = descending ? yRange.CompareTo(xRange) : xRange.CompareTo(yRange);
                }
                return result;
            }

            public int Compare(object x, object y)
            {
                return Compare(x as IFeeCalcLine, y as IFeeCalcLine);
            }

            private bool descending = false;
        }
    
    }

}
