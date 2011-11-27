using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.History;

namespace B4F.TotalGiro.Orders.Transactions
{

    public interface IInstrumentConversion : ICorporateAction
    {
        InstrumentSize ConvertedInstrumentSize { get; }
        IInstrumentHistory InstrumentTransformation { get; set; }
        decimal ConversionRate { get; }

    }
}
