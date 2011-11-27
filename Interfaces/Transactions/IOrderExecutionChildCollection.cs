using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface IOrderExecutionChildCollection : IList<IOrderExecutionChild>
    { 
        bool IsFullyApproved();
        InstrumentSize TotalAllocations { get; }
        IOrderExecution ParentExecution { get; set; }
        void AddAllocation(IOrderExecutionChild allocation);
    }
}
