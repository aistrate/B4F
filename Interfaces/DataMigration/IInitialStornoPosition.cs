using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.DataMigration.InitialPositions
{
    public interface IInitialStornoPosition
    {
        int Key { get; set; }
        ITransactionOrder TradeToBeStornoed { get; set; }
        IAccountTypeInternal AccountB { get; set; }
        IInternalEmployeeLogin CreatedBy { get; set; }
        string Reason { get; set; }
        bool IsMigrated { get; set; }
        string Currency { get; set; }
    }
}
