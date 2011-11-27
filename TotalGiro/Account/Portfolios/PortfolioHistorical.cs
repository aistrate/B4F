using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios
{
    public class PortfolioHistorical : IPortfolioHistorical
    {
        public PortfolioHistorical(IAccountTypeInternal parentAccount, DateTime positionDate, IList<IFundPositionTx> posTx,
            IList<IHistoricalPrice> prices, IList<IHistoricalExRate> rates, IList<IJournalEntryLine> lines)
        {
            this.ParentAccount = parentAccount;
            this.PositionDate = positionDate;
            this.Rates = rates;
            this.Prices = prices;
            this.FundPortfolio = new FundPortfolioHistorical(this, posTx);
            this.CashPortfolio = new CashPortfolioHistorical(this, lines);
        }

        public IAccountTypeInternal ParentAccount { get; set; }
        public DateTime PositionDate { get; set; }
        public IList<IHistoricalPrice> Prices { get; set; }
        public IList<IHistoricalExRate> Rates { get; set; }
        public IFundPortfolioHistorical FundPortfolio { get; set; }
        public ICashPortfolioHistorical CashPortfolio { get; set; }
        public Money TotalPortfolioValue
        {
            get
            {
                return FundPortfolio.TotalPortfolioValue + CashPortfolio.TotalPortfolioValue;
            }
        }

    }
}
