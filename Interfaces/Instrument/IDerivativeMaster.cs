using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments
{
    public interface IDerivativeMaster
    {
        int Key { get; set; }
        IExchange Exchange { get; set; }
        string Name { get; set; }
        SecCategories SecCategory { get; set; }
        ITradeableInstrument Underlying { get; set; }
        SecCategories UnderlyingSecCategory { get; set; }
        short DecimalPlaces { get; set; }
        int ContractSize { get; set; }
        ICurrency CurrencyNominal { get; set; }
        string DerivativeSymbol { get; set; }
        List<IDerivative> Series { get; }
    }
}
