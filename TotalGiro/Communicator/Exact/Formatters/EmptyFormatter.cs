using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.Exact.Formatters
{
    internal class EmptyFormatter : ObjectFormatter
    {
        protected override string FormatNonNull(object value)
        {
            return "";
        }
    }
}
