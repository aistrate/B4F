using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashSubPositionUnSettledCollection : TransientDomainCollection<ICashSubPositionUnSettled>, ICashSubPositionUnSettledCollection
    {
        public CashSubPositionUnSettledCollection()
            : base() { }

        public CashSubPositionUnSettledCollection(ICashPosition parentPosition)
            : base()
        {
            ParentPosition = parentPosition;
        }

        public ICashPosition ParentPosition { get; set; }

        public ICashSubPositionUnSettled DefaultSubPosition
        {
            get { return this.Find(x => x.UnSettledType == null || (x.UnSettledType != null && x.UnSettledType.IsDefault)); }
        }

        public ICashSubPositionUnSettled GetSubPosition(ICashSubPositionUnSettledType unSettledType)
        {
            return this.Find(x => (x.UnSettledType != null && x.UnSettledType.Key == unSettledType.Key) || (x.UnSettledType == null && unSettledType.IsDefault));
        }

        public void AddSubPosition(ICashSubPositionUnSettled item)
        {
            if (this.Where(x => x.UnSettledType.Key == item.UnSettledType.Key).Count() > 0)
                throw new ApplicationException("A subposition with the same properties already exists.");

            Add(item);
            item.ParentPosition = ParentPosition;
        }
    }
}
