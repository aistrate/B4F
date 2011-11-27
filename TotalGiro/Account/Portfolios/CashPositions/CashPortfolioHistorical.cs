using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashPortfolioHistorical : TransientDomainCollection<ICashPositionHistorical>, ICashPortfolioHistorical
    {
        public CashPortfolioHistorical(IPortfolioHistorical parentPortfolio,
            IList<IJournalEntryLine> lines) 
        {
            this.ParentPortfolio = parentPortfolio;
            this.lines = lines;
            fillPortfolio();
        }


        public IPortfolioHistorical ParentPortfolio { get; set; }

        public IAccountTypeInternal ParentAccount
        {
            get { return this.ParentPortfolio.ParentAccount; }
        }

        public DateTime PositionDate
        {
            get { return this.ParentPortfolio.PositionDate; }
        }

        public IList<IHistoricalExRate> Rates
        {
            get
            {
                return this.ParentPortfolio.Rates;
            }
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

        private void fillPortfolio()
        {
            var test1 = from l in lines
                        group new { Balance = l.Credit - l.Debit }
                        by new { DebitCurrencyKey = (l.Debit.Underlying.Key) } into g
                        let result = new
                        {
                            DebitCurrencyKey = g.Key,
                            TotalBalance = g.Select(v => v.Balance).Sum()
                        }
                        where result.TotalBalance.IsNotZero
                        select result;

            int baseCurrencyKey = this.ParentAccount.BaseCurrency.Key;

            foreach (var result in test1)
            {
                Decimal rate = 1m;
                if (result.DebitCurrencyKey.DebitCurrencyKey != baseCurrencyKey)
                    rate = Rates.Where(r => r.Currency.Key == result.DebitCurrencyKey.DebitCurrencyKey).ElementAt(0).Rate;
                ICashPositionHistorical newPos = new CashPositionHistorical(this, result.TotalBalance,  rate);
                this.Add(newPos);
            }


        }

        #region Privates

        IList<IJournalEntryLine> lines;

        #endregion

    }
}
