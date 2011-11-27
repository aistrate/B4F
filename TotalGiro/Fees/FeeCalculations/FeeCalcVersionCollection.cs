using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public class FeeCalcVersionCollection : GenericCollection<IFeeCalcVersion>, IFeeCalcVersionCollection
    {
        private FeeCalcVersionCollection() { }

        /// <summary>
        /// Initializes a new instance of the <b>FeeCalcversions</b> class.
        /// </summary>
        /// <param name="Parent">The parent <b>FeeCalc</b> object.</param>
        /// <param name="versions">A list of <b>FeeCalcVersion</b> objects to initialize this <b>FeeCalcversions</b> object with.</param>
        internal FeeCalcVersionCollection(FeeCalc parent, IList versions)
            : base(versions)
        {
            this.parent = parent;
        }

        public IFeeCalc Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the relevant version for the specified date
        /// </summary>
        public IFeeCalcVersion GetItemByPeriod(int period)
        {
            if (this.Count == 1)
                return this[0];
            else if (this.Count > 1)
            {
                FeeCalcVersionCollection sortedVersions = (FeeCalcVersionCollection)this.SortedByDefault();
                foreach (FeeCalcVersion version in sortedVersions)
                {
                    if (version.StartPeriod <= period && (version.EndPeriod == 0 || version.EndPeriod > period))
                        return version;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the relevant version for the specified date
        /// </summary>
        public IFeeCalcVersion LatestVersion()
        {
            if (this.Count > 0)
            {
                if (this.Count == 1)
                    return this[0];
                else if (this.Count > 1)
                    return this.OrderByDescending(u => u.StartPeriod).FirstOrDefault();
            }
            return null;
        }

        #region Overrides

        /// <summary>
        /// Adds a version to the collection.
        /// </summary>
        /// <param name="item">The version to add to the collection.</param>
        public override void Add(IFeeCalcVersion item)
        {
            // check if version exists with the same start period
            if (this.Where(u => u.StartPeriod == item.StartPeriod).Count() > 0)
                throw new ApplicationException("A version already exists with the same start period");

            base.Add(item);
            item.Parent = Parent;

            IFeeCalcVersion nextVersion = null;
            int versionCount = Count;
            foreach (IFeeCalcVersion v in this.OrderByDescending(u => u.StartPeriod))
            {
                if (nextVersion != null)
                    v.EndPeriod = nextVersion.StartPeriod;
                nextVersion = v;
                v.VersionNumber = --versionCount;
            }
        }

        public override bool Remove(IFeeCalcVersion item)
        {
            item.Parent = null;
            return base.Remove(item);
        }

        public override void Insert(int index, IFeeCalcVersion item)
        {
            Add(item);
        }

        public override void RemoveAt(int index)
        {
            this[index].Parent = null;
            base.RemoveAt(index);
        }

        /// <summary>
        /// Removes all versions from the collection.
        /// </summary>
        public override void Clear()
        {
            if (Count > 0)
                base.Clear();
        }

        protected override object GetDefaultSortValue(IFeeCalcVersion item)
        {
            return item.StartPeriod;
        }

        #endregion

        #region Private Variables

        private FeeCalc parent;

        #endregion
    }
}
