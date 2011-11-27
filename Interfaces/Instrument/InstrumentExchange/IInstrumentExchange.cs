using System;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.InstrumentExchange">InstrumentExchange</see> class
    /// </summary>
    public interface IInstrumentExchange
    {
        int Key { get; set; }
        IExchange Exchange { get; set; }
        ICounterPartyAccount DefaultCounterParty { get; set; }
        ITradeableInstrument Instrument { get; set; }
        byte NumberOfDecimals { get; set; }
        decimal TickSize { get; set; }
        bool CertificationRequired { get; set; }
        string RegisteredInNameOf { get; set; }
        string DividendPolicy { get; set; }
        string CommissionRecipientName { get; set; }
        Int16 DefaultSettlementPeriod { get; set; }
        bool DoesSupportAmountBasedBuy { get; set; }
        bool DoesSupportAmountBasedSell { get; set; }
        bool DoesSupportServiceCharge { get; set; }
        decimal ServiceChargePercentageBuy { get; set; }
        decimal ServiceChargePercentageSell { get; set; }
        decimal GetServiceChargePercentageForOrder(IOrder order);
        bool ServiceChargeBothSides { get; }
        string ServiceChargeDisplayInfo { get; }
    }
}
