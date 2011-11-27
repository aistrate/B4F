using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.OrderRouteMapper;

namespace B4F.TotalGiro.Orders
{
    public interface IStgAmtOrder: IAggregateAmtOrder, IStgOrder
    {

        bool IsCurrencyConverted { get; }
        Money ServiceChargeForBuy { get; }
        Money ServiceChargeForSell { get; }

        IStgAmtOrder ConvertFx(decimal exRate, Money convertedAmount);
        IStgSizeOrder ConvertBondOrder(Price price, DateTime settlementDate, IOrderRouteMapper routeMapper);
    }
}
