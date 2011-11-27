using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface IOrderExecutionChild : ITransactionOrder
    {
        IOrderExecution ParentExecution { get; set; }
    }
}
