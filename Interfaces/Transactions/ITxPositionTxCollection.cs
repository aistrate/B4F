using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ITxPositionTxCollection : IList<IFundPositionTx>
    {
        ITransaction ParentTransaction { get; set; }
        void AddPositionTx(IFundPositionTx item);

    }
}
