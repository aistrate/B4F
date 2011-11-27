using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface IOrderAllocation : IOrderExecutionChild
    {
        void setCommission(IGLLookupRecords lookups, Money Commission);        
    }
}
