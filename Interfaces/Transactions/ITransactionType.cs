using System;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ITransactionType
    {
        int Key { get; set; }
        string Name { get; }
        string Description { get; }
    }
}
