using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public struct CurrencyRateRowView
    {
        public CurrencyRateRowView(Currency currency, decimal rate)
        {
            this.instrumentId = currency.Key;
            this.symbol = currency.Symbol;
            this.countryName = currency.CountryOfOrigin != null ?    currency.CountryOfOrigin.CountryName : "";
            this.altSymbol = currency.AltSymbol;
            this.rate = rate;
        }

        public int InstrumentId { get { return this.instrumentId; } }
        public string Symbol { get { return this.symbol; } }
        public string CountryName { get { return this.countryName; } }
        public string AltSymbol { get { return this.altSymbol; } }
        public decimal Rate { get { return this.rate; } }

        #region Privates

        private int instrumentId;
        private string symbol;
        private string countryName;
        private string altSymbol;
        private decimal rate;
        
        #endregion
    }
}
