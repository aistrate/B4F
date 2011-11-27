using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.OrderCollection">OrderCollection</see> class
    /// </summary>
    public interface IAccountOrderCollection : IList<IOrder>
    {
        IAccountTypeInternal ParentAccount { get; set; }
        IAccountOrderCollection NewCollection(Func<IOrder, bool> criteria);
        IAccountOrderCollection Exclude(IList<IInstrument> excludedInstruments);
        Money TotalAmountInSpecifiedNominalCurrency(ICurrency currencyNominal);
        IAccountOrderCollection Filter(IInstrument tradedInstrument, OrderSideFilter sideFilter);
        IAccountOrderCollection Filter(OrderTypes orderType, OrderSideFilter sideFilter);
        Money TotalAmount();
        Money TotalGrossAmount();
        Money TotalAmount(IInstrument instrument);
        Money TotalAmount(IInstrument instrument, bool useRequestedInstrument);

        InstrumentSize TotalSize(IInstrument instrument);
    }
}
