using System;
using System.Collections.Generic;
namespace B4F.TotalGiro.Orders.Transfers
{
    public interface IPositionTransferDetailCollection : IList<IPositionTransferDetail>
    {
        void AddPosition(IPositionTransferDetail position);
        IPositionTransfer ParentTransfer { get; set; }
    }
}
