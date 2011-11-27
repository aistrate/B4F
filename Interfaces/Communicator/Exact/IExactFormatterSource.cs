using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IExactFormatterSource
    {
        ExactFieldCollection GetFields();
    }
}
