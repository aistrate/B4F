using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class TxPositionTxCollection : TransientDomainCollection<IFundPositionTx>, ITxPositionTxCollection
    {
        public TxPositionTxCollection()
            : base() { }

        public TxPositionTxCollection(ITransaction parentTransaction)
            : base()
        {
            ParentTransaction = parentTransaction;
        }


        public void AddPositionTx(IFundPositionTx item)
        {
            item.ParentTransaction = ParentTransaction;
            base.Add(item);
        }

        public ITransaction ParentTransaction { get; set; }


    }
}
