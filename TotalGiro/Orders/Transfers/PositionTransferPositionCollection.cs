using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Orders.Transfers
{
    public class PositionTransferPositionCollection: TransientDomainCollection<IPositionTransferPosition>, IPositionTransferPositionCollection
    {
        public PositionTransferPositionCollection()
            : base() { }

        public PositionTransferPositionCollection(IPositionTransferPortfolio parentPortfolio)
            : base()
        {
            ParentPortfolio = parentPortfolio;
        }

        public void AddPosition(IPositionTransferPosition position)
        {
            position.ParentPortfolio = ParentPortfolio;
            base.Add(position);

        }

        public IPositionTransferPortfolio ParentPortfolio { get; set; }

    }
}
