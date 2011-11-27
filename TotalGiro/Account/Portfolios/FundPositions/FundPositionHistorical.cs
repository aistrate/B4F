using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public class FundPositionHistorical : IFundPositionHistorical
    {
        public FundPositionHistorical(IFundPortfolioHistorical parent, InstrumentSize size, Price price, Decimal rate)
        {
            this.Parent = parent;
            this.Size = size;
            this.Price = price;
            this.rate = rate;
        }

        public IFundPortfolioHistorical Parent { get; set; }
        public int Key { get { return this.PositionInstrument.Key; } }
        public IInstrument PositionInstrument { get { return this.HistoricalBaseValue.Underlying; } }
        public IAccountTypeInternal Account
        {
            get
            {
                return this.Parent.ParentAccount;
            }
        }
        public InstrumentSize Size { get; set; }
        public Price Price { get; set; }
        public Money HistoricalValue
        {
            get
            {
                Money first = Size.CalculateAmount(Price);
                Money returnValue = new Money(first.Quantity, first.Underlying.ToCurrency, rate);
                return returnValue;
            }
        }

        public Money HistoricalBaseValue
        {
            get
            {
                return HistoricalValue.BaseAmount;
            }
        }


        private Decimal rate;
    }
}
