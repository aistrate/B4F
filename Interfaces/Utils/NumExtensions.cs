using System;

namespace B4F.TotalGiro.Utils
{
    public static class NumExtensions
    {
        public static int BoundedBy(this int number, int minBound, int maxBound)
        {
            if (minBound > maxBound)
                throw new ApplicationException("Minimum bound cannot be greater than maximum bound.");

            return Math.Min(maxBound, Math.Max(minBound, number));
        }
    }
}
