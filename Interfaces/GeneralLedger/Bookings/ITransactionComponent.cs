using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface ITransactionComponent : IBookingComponentParent
    {
        ITransaction ParentTransaction { get; set; }
    }
}
