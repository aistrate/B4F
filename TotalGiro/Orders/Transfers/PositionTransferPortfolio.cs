using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Orders.Transfers
{
    public class PositionTransferPortfolio : IPositionTransferPortfolio
    {
        public PositionTransferPortfolio() 
        {
            this.positions = new PositionTransferPositionCollection(this);
        }

        public PositionTransferPortfolio(IAccountTypeInternal account, DateTime positionDate, IPositionTransfer parentTransfer)
            : this()
        {
            this.Account = account;
            this.PositionDate = positionDate;
            this.ParentTransfer = parentTransfer;
        }



        public int Key { get; set; }
        public IPositionTransfer ParentTransfer { get; set; }
        public IAccountTypeInternal Account { get; set; }
        public DateTime PositionDate { get; set; }


        public virtual IPositionTransferPositionCollection Positions
        {
            get
            {
                IPositionTransferPositionCollection temp = (IPositionTransferPositionCollection)positions.AsList();
                if (temp.ParentPortfolio == null) temp.ParentPortfolio = this;
                return temp;
            }
        }
        private IDomainCollection<IPositionTransferPosition> positions;
    }
}
