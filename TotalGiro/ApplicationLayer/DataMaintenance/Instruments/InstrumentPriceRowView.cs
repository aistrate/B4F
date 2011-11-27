using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class InstrumentPriceRowView
    {
        public InstrumentPriceRowView(IInstrumentsWithPrices instrument, decimal priceQuantity)
        {
            this.instrumentId = instrument.Key;
            this.isin = instrument.Isin;
            this.instrumentName = instrument.Name;
            this.currency = instrument.CurrencyNominal.AltSymbol;
            this.decimalPlaces = instrument.DecimalPlaces;
            this.priceQuantity = priceQuantity;
        }

        public int InstrumentId { get { return this.instrumentId; } }
        public string Isin { get { return this.isin; } }
        public string InstrumentName { get { return this.instrumentName; } }
        public string Currency { get { return this.currency; } }
        public decimal PriceQuantity { get { return this.priceQuantity; } }
        public int DecimalPlaces { get { return this.decimalPlaces; } }

        #region Privates

        private int instrumentId;
        private string isin;
        private string instrumentName;
        private string currency;
        private decimal priceQuantity;
        private int decimalPlaces;

        #endregion
    }
}
