using System;
using System.Collections.Generic;
namespace B4F.TotalGiro.Orders.Transfers
{
    public interface IPositionTransferPositionCollection : IList<IPositionTransferPosition> 
    {
        void AddPosition(IPositionTransferPosition position);
        IPositionTransferPortfolio ParentPortfolio { get; set; }
    }
}
