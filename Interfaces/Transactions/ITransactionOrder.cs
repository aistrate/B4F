using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ITransactionOrder : ITransactionTrading
    {
        decimal FillRatio { get; set; }
        IOrder Order { get; set; }          
    }
}
