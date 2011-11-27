using System;

namespace B4F.TotalGiro.Instruments.Classification
{
    public interface IInstrumentsCategories
    {
        int Key { get; set; }
        string InstrumentsCategoryName { get; }
        bool IsDefault { get; }
    }
}

