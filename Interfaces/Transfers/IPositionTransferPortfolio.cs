using System;
using B4F.TotalGiro.Accounts;
namespace B4F.TotalGiro.Orders.Transfers
{
    public interface IPositionTransferPortfolio
    {
        int Key { get; set; }
        IPositionTransfer ParentTransfer { get; set; }
        IAccountTypeInternal Account { get; set; }
        DateTime PositionDate { get; set; }
        IPositionTransferPositionCollection Positions { get; }
    }
}
