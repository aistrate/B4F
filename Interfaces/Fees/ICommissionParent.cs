using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Fees
{
    public enum CommissionParentTypes
    {
        Order,
        Transaction
    }
    
    public interface ICommissionParent
    {
        CommissionParentTypes Type { get; }
        IOrder Order { get; }
        //ITransactionOrder Transaction { get; }
    }
}
