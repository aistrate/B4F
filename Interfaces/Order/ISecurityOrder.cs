using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.SecurityOrder">SecurityOrder</see> class
    /// </summary>
    public interface ISecurityOrder : IOrder
	{
		ITradeableInstrument TradedInstrument { get; }
        Money ServiceCharge { get; set; }
        Money AccruedInterest { get; set; }
        bool IsFsSendable { get; }
        INota CreateNota();
        ISecurityOrder Convert(Price price, B4F.TotalGiro.OrderRouteMapper.IOrderRouteMapper routeMapper);
        
	}
}
