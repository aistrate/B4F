using System;
using System.Collections.Generic;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ICorporateActionExecution: ITransaction
    {
        ITransactionNTM CounterTransaction { get; set; }
        IGeneralOperationsBooking CounterBooking { get; set; }
    }
}
