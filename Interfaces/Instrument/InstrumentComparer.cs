using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public class InstrumentComparer : IEqualityComparer<IInstrument>
    {

        #region IEqualityComparer<IInstrument> Members

        public bool Equals(IInstrument x, IInstrument y)
        {
            // Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            // Check whether the products' properties are equal.
            return x.Key == y.Key;

        }

        public int GetHashCode(IInstrument obj)
        {
            // Check whether the object is null.
            if (Object.ReferenceEquals(obj, null)) return 0;

            // Get the hash code for the Name field if it is not null.
            int hashProductName = obj.Key == null ? 0 : obj.Key.GetHashCode();

            // Get the hash code for the Code field.
            int hashProductCode = obj.Name.GetHashCode();

            // Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;

        }

        #endregion
    }
}
