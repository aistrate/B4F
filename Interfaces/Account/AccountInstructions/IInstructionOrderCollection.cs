using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions
{
    public interface IInstructionOrderCollection :IList<IOrder>
    {
        IInstruction ParentInstruction { get; set; }
        IInstructionOrderCollection NewCollection(Func<IOrder, bool> criteria);
        IInstructionOrderCollection Exclude(IList<IInstrument> excludedInstruments);
    }
}
