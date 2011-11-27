using System;
using System.Collections.Generic;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Orders.Transactions
{
    public interface ITransactionComponentCollection : IList<ITransactionComponent>
    {
        ITransaction Parent { get; set; }
        Money TotalValue { get; }
        Money BaseTotalValue { get; }
        Money ReturnComponentValue(BookingComponentTypes type);
        Money ReturnComponentValueInBaseCurrency(BookingComponentTypes type);
        Money ReturnComponentValue(BookingComponentTypes[] types);
        Money ReturnComponentValueInBaseCurrency(BookingComponentTypes[] types);
        Money TotalValueComponentsInSpecifiedCurrency(ICurrency currency);
    }
}
