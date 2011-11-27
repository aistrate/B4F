using System;
using System.Collections.Generic;
namespace B4F.TotalGiro.Orders.Transfers.Reporting
{
    public interface IPositionTransferReportPositionCollection : IList<IPositionTransferReportPosition>
    {
        void AddPosition(IPositionTransferReportPosition position);
        IPositionTransferReportPortfolio ParentPortfolio { get; set; }
    }
}
