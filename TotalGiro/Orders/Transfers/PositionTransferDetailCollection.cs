using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Orders.Transfers
{
    public class PositionTransferDetailCollection : TransientDomainCollection<IPositionTransferDetail>, IPositionTransferDetailCollection
    {
        public PositionTransferDetailCollection()
            : base() { }

        public PositionTransferDetailCollection(IPositionTransfer parentTransfer)
            : base()
        {
            ParentTransfer = parentTransfer;
        }

        public void AddPosition(IPositionTransferDetail detail)
        {
            detail.ParentTransfer = ParentTransfer;
            base.Add(detail);

        }

        public IPositionTransfer ParentTransfer { get; set; }

    }
}
