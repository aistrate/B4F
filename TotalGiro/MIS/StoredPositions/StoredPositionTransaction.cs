using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.MIS.StoredPositions
{
    public class StoredPositionTransaction : IStoredPositionTransaction
    {
        public int Key { get; set; }
        public IFundPositionTx FundPositionTransaction { get; set; }
        public IAccountTypeInternal Account { get; set; }
        public InstrumentSize Size { get; set; }
        public ITransaction OriginalTransaction { get; set; }
        public IFundPosition FundPosition { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
