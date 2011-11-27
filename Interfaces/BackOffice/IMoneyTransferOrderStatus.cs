using System;
namespace B4F.TotalGiro.BackOffice.Orders
{
    public interface IMoneyTransferOrderStatus
    {
        MoneyTransferOrderStati Key { get; }
        string Name { get; set; }
        bool IsOpen { get; set; }
        bool IsEditable { get; set; }
    }
}
