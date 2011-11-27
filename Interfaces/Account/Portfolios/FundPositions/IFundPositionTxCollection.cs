using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public interface IFundPositionTxCollection : IList<IFundPositionTx>
    {
        IFundPosition ParentPosition { get; set; }
        void AddPositionTX(IFundPositionTx item);
    }
}
