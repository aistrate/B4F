using System;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public class PositionRowView
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

        public PositionRowView(IFundPosition position)
        {
            key = position.Key;
            instrumentName = position.Size.Underlying.DisplayName;
            size = position.Size.Quantity;
            value = position.CurrentBaseValue;
            modelAllocation = Math.Round(100m * position.ModelAllocation, 4);

            if (position.Size.Underlying.IsWithPrice)
            {
                IInstrumentsWithPrices instrument = (IInstrumentsWithPrices)position.Size.Underlying;
                isin = instrument.Isin;
                IsCloseable = instrument.IsTradeable;
                if (instrument.CurrentPrice != null)
                {
                    price = instrument.CurrentPrice.Price;
                    priceDate = instrument.CurrentPrice.Date;
                }
                if (instrument.CurrencyNominal.ExchangeRate != null)
                    exchangeRate = instrument.CurrencyNominal.ExchangeRate.Rate;
                else
                {
                    if (instrument.CurrencyNominal.IsBase)
                        exchangeRate = 1M;
                }
                if (instrument.SecCategory.Key == SecCategories.Bond)
                {
                    IBond bond = (IBond)instrument;
                    if (bond != null && bond.DoesPayInterest)
                    {
                        AccruedInterest = position.
                            Get(v => v.BondCouponPayments).
                            Get(w => w.ActivePayment).
                            Get(x => x.DailyCalculations.
                            Get(y => y.LastCalculation).
                            Get(z => z.CalculatedAccruedInterestUpToDate));

                        ShowAccruedInterest = (AccruedInterest != null && AccruedInterest.IsNotZero);
                    }
                }
            }
        }

        public int Key { get { return this.key; } }
        public string Isin { get { return this.isin; } }
        public string InstrumentName { get { return this.instrumentName; } }
        public decimal Size { get { return this.size; } }
        public decimal Percentage { get { return this.percentage; } set { this.percentage = value; } }
        public decimal ModelAllocation { get { return this.modelAllocation; } set { this.modelAllocation = value; } }
        public Price Price { get { return this.price; } }
        public DateTime PriceDate { get { return this.priceDate; } }
        public string PriceShortDisplayString { get { return (this.price != null ? this.price.ShortDisplayString : "" ); } }
        public decimal ExchangeRate { get { return this.exchangeRate; } }
        public Money Value { get { return this.value; } }
        public Money AccruedInterest { get; set; }
        public bool ShowAccruedInterest { get; set; }
        public bool IsCloseable { get; set; }
    }
}
