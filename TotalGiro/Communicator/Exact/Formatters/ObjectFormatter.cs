using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.Exact.Formatters
{
    internal abstract class ObjectFormatter
    {
        public virtual string Format(object value)
        {
            if (value == null)
                return "";
            else
                return FormatNonNull(value);
        }

        protected abstract string FormatNonNull(object value);
    }
}
