using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.OrderCollection">OrderCollection</see> class
    /// </summary>
    public interface IOrderCollection : IList<IOrder>
	{
        bool Remove(IOrder item, bool unApprove);
        InstrumentSize TotalSize(IInstrument instrument);
        IOrder ParentOrder { get; set; }
        IOrderCollection Exclude(IList<IInstrument> excludedInstruments);
        IOrderCollection Filter(IInstrument tradedInstrument, OrderSideFilter sideFilter);
        IOrderCollection Filter(OrderTypes orderType, OrderSideFilter sideFilter);
        Money TotalAmount();
        Money TotalAmount(IInstrument instrument);
        Money TotalAmount(IInstrument instrument, bool useRequestedInstrument);
        Money TotalAmountInSpecifiedNominalCurrency(ICurrency currencyNominal);
        Money TotalCommission();
    }
}
