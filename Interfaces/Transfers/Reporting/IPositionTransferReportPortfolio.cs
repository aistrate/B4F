using System;
namespace B4F.TotalGiro.Orders.Transfers.Reporting
{
    public interface IPositionTransferReportPortfolio
    {
        B4F.TotalGiro.Accounts.IAccountTypeInternal Account { get; set; }
        IPositionTransfer ParentTransfer { get; set; }
        DateTime PositionDate { get; set; }
        IPositionTransferPortfolio BeforePortfolio { get; set; }
        IPositionTransferPortfolio AfterPortfolio { get; set; }
        IPositionTransferReportPositionCollection Positions { get; }

    }
}
