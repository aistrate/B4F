using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashPositionHistorical : ICashPositionHistorical
    {
        public CashPositionHistorical(ICashPortfolioHistorical parent, Money historicalValue, decimal rate)
        {
            this.Parent = parent;
            this.historicalValue = historicalValue;
            this.Rate = rate;
        }

        public ICashPortfolioHistorical Parent { get; set; }
        public int Key { get { return this.PositionInstrument.Key; } }
        public IInstrument PositionInstrument { get { return this.HistoricalValue.Underlying; } }
        public Money HistoricalValue
        {
            get
            {
                return new Money(this.historicalValue.Quantity, this.historicalValue.Underlying.ToCurrency, this.Rate);
            }
        }
        public Money HistoricalBaseValue
        {
            get
            {
                return HistoricalValue.BaseAmount;
            }
        }
        public Decimal Rate { get; set; }
        public IAccountTypeInternal Account
        {
            get
            {
                return this.Parent.ParentAccount;
            }
        }

        private Money historicalValue;
    }
}
