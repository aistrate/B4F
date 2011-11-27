using System;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.TaxRates
{
    public class HistoricalTaxRate : IHistoricalTaxRate
    {
        private HistoricalTaxRate() { }

        #region Props

        public virtual int Key { get; set; }
        public virtual ICountry Country { get; protected set; }
        public virtual DateTime StartDate { get; protected set; }
        public virtual DateTime EndDate { get; protected set; }
        public virtual decimal StandardRate { get; protected set; }
        public virtual decimal ReducedRate { get; protected set; }

        #endregion
    }
}
