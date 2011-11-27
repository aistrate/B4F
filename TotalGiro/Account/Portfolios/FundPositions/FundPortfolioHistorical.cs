using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Instruments.ExRates;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public class FundPortfolioHistorical : TransientDomainCollection<IFundPositionHistorical>, IFundPortfolioHistorical
    {
        public FundPortfolioHistorical(IPortfolioHistorical parentPortfolio,
            IList<IFundPositionTx> posTx)
        {
            this.ParentPortfolio = parentPortfolio;
            this.posTx = posTx;
            fillPortfolio();
        }
        public IPortfolioHistorical ParentPortfolio { get; set; }

        public IAccountTypeInternal ParentAccount
        {
            get { return this.ParentPortfolio.ParentAccount; }
        }

        public IList<IHistoricalPrice> Prices
        {
            get
            {
                return this.ParentPortfolio.Prices;
            }
        }
        public IList<IHistoricalExRate> Rates
        {
            get
            {
                return this.ParentPortfolio.Rates;
            }
        }

        public DateTime PositionDate
        {
            get { return this.ParentPortfolio.PositionDate; }
        }


        public Money TotalPortfolioValue
        {
            get
            {
                Money returnValue = new Money(0m, this.ParentAccount.BaseCurrency);
                if (this.Count > 0)
                {
                    returnValue = this.Select(m => m.HistoricalBaseValue).Sum();
                }
                return returnValue;
            }

        }

        public Money CultureFundValue
        {
            get
            {
                Money returnValue = new Money(0m, this.ParentAccount.BaseCurrency);
                if ((this.Count > 0) && this.Exists(f => ((ISecurityInstrument) f.Size.Underlying).IsCultureFund))
                {
                    returnValue = this.Where(f => ((ISecurityInstrument) f.Size.Underlying).IsCultureFund).Select(p => p.HistoricalBaseValue).Sum();
                }
                return returnValue;
            }
        }

        public Money GreenFundValue
        {
            get
            {
                Money returnValue = new Money(0m, this.ParentAccount.BaseCurrency);
                if ((this.Count > 0) && this.Exists(f => ((ISecurityInstrument)f.Size.Underlying).IsGreenFund))
                {
                    returnValue = this.Where(f => ((ISecurityInstrument)f.Size.Underlying).IsGreenFund).Select(p => p.HistoricalBaseValue).Sum();
                }
                return returnValue;
            }
        }
        private void fillPortfolio()
        {
            var test1 = from p in posTx
                        group new { size = p.Size } by p.Instrument into g
                        let result = new
                        {
                            Instrument = g.Key,
                            TotalSize = g.Select(v => v.size).Sum()
                        }
                        where result.TotalSize.IsNotZero
                        select result;

            int baseCurrencyKey = this.ParentAccount.BaseCurrency.Key;

            foreach (var result in test1)
            {
                IHistoricalPrice hp = Prices.Where(p => p.Price.Instrument.Key == result.Instrument.Key).FirstOrDefault();
                if (hp == null)
                    throw new ApplicationException(string.Format("Could not find a price on {0} for {1}", PositionDate.ToShortDateString(), result.Instrument.DisplayNameWithIsin));
                Price price = hp.Price;
                Decimal rate = 1m;
                if (result.Instrument.CurrencyNominal.Key != baseCurrencyKey)
                    rate = Rates.Where(r => r.Currency.Key == result.Instrument.CurrencyNominal.Key).ElementAt(0).Rate;
                IFundPositionHistorical newPos = new FundPositionHistorical(this, result.TotalSize, price, rate);
                Money testf = newPos.HistoricalBaseValue;
                Money testg = newPos.HistoricalBaseValue;
                this.Add(newPos);
            }


        }

        #region Privates

        private IList<IFundPositionTx> posTx;

        #endregion
    }
}
