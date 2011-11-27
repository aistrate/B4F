using System;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.TaxRates
{
    public interface IHistoricalTaxRate
    {
        int Key { get; set;  }
        ICountry Country { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        decimal StandardRate { get; }
        decimal ReducedRate { get; }

    }
}
