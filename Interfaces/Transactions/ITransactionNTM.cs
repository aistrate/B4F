using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transfers;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ITransactionNTM: ITransaction
    {
        IPositionTransferDetail TransferDetail { get; set; }
    }
}
