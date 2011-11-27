using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Utils.Linq;

namespace B4F.TotalGiro.Utils
{
    public class SmallSetComparer<T> : Comparer<T>
    {
        public SmallSetComparer(IEnumerable<T> orderedSet)
            : this(orderedSet, true)
        {
        }

        public SmallSetComparer(IEnumerable<T> orderedSet, bool isMissingSmaller)
        {
            elemIndexes = orderedSet.Distinct()
                                    .Zip(EnumerableExtensions.RangeFrom(0))
                                    .ToDictionary(pair => pair.Item1, pair => pair.Item2);

            missingElemIndex = isMissingSmaller ? -1 : elemIndexes.Count;
        }

        public override int Compare(T x, T y)
        {
            return getIndex(x).CompareTo(getIndex(y));
        }

        private int getIndex(T elem)
        {
            int index;
            bool found = elemIndexes.TryGetValue(elem, out index);

            return found ? index : missingElemIndex;
        }

        private Dictionary<T, int> elemIndexes = new Dictionary<T, int>();
        private int missingElemIndex = -1;
    }
}
