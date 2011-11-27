using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;
using NHibernate.Collection;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Fees.CommCalculations
{
	/// <summary>
    /// A list of commission calculation lines (<b>CommCalcLine</b> objects).
	/// </summary>
    public class CommCalcLineCollection : TransientDomainCollection<CommCalcLine>, ICommCalcLineCollection
	{
        public CommCalcLineCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the <b>CommCalcLines</b> class.
		/// </summary>
		/// <param name="Parent">The parent <b>CommCalc</b> object.</param>
        internal CommCalcLineCollection(CommCalc parent)
            : base()
		{
            this.Parent = parent;
		}

        public virtual CommCalc Parent { get; set; }

		#region Overrides

		/// <summary>
        /// Adds a line to the collection.
		/// </summary>
        /// <param name="item">The line to add to the collection.</param>
        public void AddCalculation(CommCalcLine item)
		{
            if (Count == 0)
			{
                if (item.LineBasedType == CommCalcLineBasedTypes.AmountBased)
                    ((CommCalcLineAmountBased)item).LowerRange = new Money(0M, this.Parent.CommCurrency);
                else
                    ((CommCalcLineSizeBased)item).LowerRange = 0M;
			}
			base.Add(item);
			item.Parent = this.Parent;
            ArrangeLines();
		}

        public bool RemoveCalculation(CommCalcLine item)
        {
            item.Parent = null;
            bool success = base.Remove(item);
            if (success && Count > 1)
                ArrangeLines();
            return success;
        }

        public void InsertCalculation(int index, CommCalcLine item)
        {
            Add(item);
        }

        public void RemoveCalculationAt(int index)
        {
            if (index >= 0 && index < this.Count)
                Remove(this[index]);
        }

        /// <summary>
        /// Orders the calcommcalclines and fills the range attributes
        /// </summary>
        public void ArrangeLines()
        {
            if (this.Count > 1)
            {
                short i = 0;
                foreach (var item in this.OrderBy(x => x.LowerRangeQuantity))
                {
                    item.SerialNo = i;
                    if (i < Count - 1)
                    {
                        if (item.LineBasedType == CommCalcLineBasedTypes.AmountBased)
                            ((CommCalcLineAmountBased)item).UpperRange = ((CommCalcLineAmountBased)this[i + 1]).LowerRange;
                        else
                            ((CommCalcLineSizeBased)item).UpperRange = ((CommCalcLineSizeBased)this[i + 1]).LowerRange;
                    }
                    i++;
                }
            }
        }

		#endregion

    }
}
