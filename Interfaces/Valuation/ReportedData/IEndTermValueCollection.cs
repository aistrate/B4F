using System;
using System.Collections.Generic;
namespace B4F.TotalGiro.Valuations.ReportedData
{
    public interface IEndTermValueCollection : IList<IEndTermValue>
    {
        void AddEndTermValue(IEndTermValue entry);
        IPeriodicReporting Parent { get; set; }
    }
}
