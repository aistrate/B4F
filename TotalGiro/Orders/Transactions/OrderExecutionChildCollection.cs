using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class OrderExecutionChildCollection : TransientDomainCollection<IOrderExecutionChild>, IOrderExecutionChildCollection
    {
        public OrderExecutionChildCollection()
            : base() { }

        public OrderExecutionChildCollection(IOrderExecution parentExecution)
            : base()
        {
            ParentExecution = parentExecution;
        }

        public void AddAllocation(IOrderExecutionChild allocation)
        {
            allocation.ParentExecution = ParentExecution;
            base.Add(allocation);
        }

        public InstrumentSize TotalAllocations
        {
            get
            {
                var total = this.Where(x => x.ValueSize != null).Select(v => v.ValueSize).Sum();
                return total;
            }
        }

        public bool IsFullyApproved()
        {
            bool retVal = false;
            if ((this.Count > 0) && (!this.Any(x => !x.Approved)))
                retVal = true;
            return retVal;
        }

        public ICrumbleTransaction Crumble
        {
            get
            {
                if (this.Any(x => x.TransactionType == TransactionTypes.Crumble))
                {
                    return (ICrumbleTransaction) this.Where(x => x.TransactionType == TransactionTypes.Crumble).ElementAt(0);
                }
                else
                    return null;
            }
        }

        public IOrderExecution ParentExecution { get; set; }
    }
}
