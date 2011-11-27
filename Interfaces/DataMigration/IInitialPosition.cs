using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
namespace B4F.DataMigration.InitialPositions
{
    public interface IInitialPosition
    {     
        bool IsMigrated { get; set; }
        IAccountTypeInternal AccountA { get; set; }
        IAccount AccountB { get; set; }
        InstrumentSize InitialSize { get; set; }
        int Key { get; set; }
        int MigratedKey { get; set; }
        DateTime TransactionDate { get; set; }
        string Description { get; set; }
        Price InitialPrice { get; set; }        
        B4F.TotalGiro.Orders.Side TxSide { get; }
    }
}
