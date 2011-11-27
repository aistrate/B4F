using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Orders.Transfers
{
    public class TransactionNTMCollection : TransientDomainCollection<ITransactionNTM>, ITransactionNTMCollection
    {
        public TransactionNTMCollection()
            : base() { }

        public TransactionNTMCollection(IPositionTransferDetail transferDetail)
            : base()
        {
            TransferDetail = transferDetail;
        }

        public void AddTransactionNTM(ITransactionNTM transaction)
        {
            transaction.TransferDetail = TransferDetail;
            base.Add(transaction);
        }

        public IPositionTransferDetail TransferDetail { get; set; }
    }
}
