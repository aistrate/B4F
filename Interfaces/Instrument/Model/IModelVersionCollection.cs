using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments
{
    public interface IModelVersionCollection : IList<IModelVersion>
    {
        IModelBase Parent { get; set; }
        IModelVersion GetVersionByDate(DateTime date);
    }
}
