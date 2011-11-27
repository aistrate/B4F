using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Valuations.ReportedData;

namespace B4F.TotalGiro.Accounts
{
    public interface IEndTermValueCollection : IList<IEndTermValue>
    {

        IAccountTypeInternal ParentAccount { get; set; }
        bool EndTermValueExists(ReportingPeriodDetail reportingPeriodDetail);
    }
}
