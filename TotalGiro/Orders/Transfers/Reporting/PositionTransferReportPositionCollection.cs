using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Orders.Transfers.Reporting
{
    public class PositionTransferReportPositionCollection : TransientDomainCollection<IPositionTransferReportPosition>, IPositionTransferReportPositionCollection
    {
        public PositionTransferReportPositionCollection()
            : base() { }

        public PositionTransferReportPositionCollection(IPositionTransferReportPortfolio parentPortfolio)
            : base()
        {
            ParentPortfolio = parentPortfolio;
        }

        public void AddPosition(IPositionTransferReportPosition position)
        {
            position.ParentPortfolio = ParentPortfolio;
            base.Add(position);
            position.Key = this.Count;
        }

        public IPositionTransferReportPortfolio ParentPortfolio { get; set; }

    }
}
