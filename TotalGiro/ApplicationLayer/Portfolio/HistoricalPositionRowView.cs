using System;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public class HistoricalPositionRowView
    {
        private int key;
        private string isin;
        private string instrumentName;
        private decimal size;
        private decimal percentage;
        private decimal modelAllocation;
        private Price price;
        private DateTime priceDate;
        private decimal exchangeRate;
        private Money value;
        private decimal transferPercentage;
        private decimal transferSize;
        private Money transferValue;



        public HistoricalPositionRowView(IFundPositionHistorical position)
        {
            key = position.Key;
            instrumentName = position.Size.Underlying.DisplayName;
            PositionSize = position.Size;
            size = position.Size.Quantity;
            value = position.HistoricalBaseValue;
            price = position.Price;
            priceDate = position.Parent.PositionDate;
            exchangeRate = position.HistoricalValue.XRate;
            //modelAllocation = Math.Round(100m * position.ModelAllocation, 4);

            if (position.Size.Underlying.IsTradeable)
            {
                ITradeableInstrument instrument = (ITradeableInstrument)position.Size.Underlying;
                isin = instrument.Isin;
            }
        }

        public HistoricalPositionRowView(ICashPositionHistorical position)
        {
            key = position.Key;
            instrumentName = position.PositionInstrument.Name;
            size = position.HistoricalValue.Quantity;
            value = position.HistoricalBaseValue;
            PositionSize = position.HistoricalBaseValue;
            price = new Price(1m, position.PositionInstrument.ToCurrency, position.PositionInstrument);
            priceDate = position.Parent.PositionDate;
            exchangeRate = position.HistoricalValue.XRate;
            //modelAllocation = Math.Round(100m * position.ModelAllocation, 4);
            isin = "Cash";

        }

        public int Key { get { return this.key; } }
        public string Isin { get { return this.isin; } }
        public string InstrumentName { get { return this.instrumentName; } }
        public decimal Size { get { return this.size; } }
        public decimal Percentage { get { return this.percentage; } set { this.percentage = value; } }
        public decimal ModelAllocation { get { return this.modelAllocation; } set { this.modelAllocation = value; } }
        public Price Price { get { return this.price; } }
        public DateTime PriceDate { get { return this.priceDate; } }
        public string PriceShortDisplayString { get { return (this.price != null ? this.price.ShortDisplayString : ""); } }
        public decimal ExchangeRate { get { return this.exchangeRate; } }
        public Money Value { get { return this.value; } }
        public InstrumentSize PositionSize { get; set; }
    }
}
