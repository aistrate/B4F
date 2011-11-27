using System;
using B4F.TotalGiro.Orders.Transactions;
namespace B4F.DataMigration.CleopatraMigration
{
    public interface ICleopatraTX
    {
        B4F.TotalGiro.Accounts.IAccountTypeInternal Account { get; set; }
        B4F.TotalGiro.Instruments.InstrumentSize ValueSize { get; set; }
        B4F.TotalGiro.Instruments.Money CashValue { get; set; }
        B4F.TotalGiro.Instruments.Money Commission { get; set; }
        B4F.TotalGiro.Instruments.Price Price { get; set; }
        B4F.TotalGiro.Orders.Side TxSide { get; }
        bool Migrated { get; set; }
        DateTime EntryDate { get; set; }
        DateTime SettlementDate { get; set; }
        DateTime TransactionDate { get; set; }
        DateTime ValueDate { get; set; }
        int Key { get; set; }
        ITransaction MigratedTransaction { get; set; } 
        string AccNumb { get; set; }
        string AccSub { get; set; }
        string AccType { get; set; }
        string BrokerID { get; set; }
        string Exchange { get; set; }
        string IdentCur { get; set; }
        string MemberID { get; set; }
        string NrTrade { get; set; }
        string Partij { get; set; }
        string StatCode { get; set; }
        string StdInstr { get; set; }
        string StockID { get; set; }
        string TRNSTYPE { get; set; }
    }
}
