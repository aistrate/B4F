using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using System.Data;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class HistoricalPositionAdapter
    {
        public static IPortfolioHistorical GetHistoricalPortfolio(int accountID, DateTime positionDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return GetHistoricalPortfolio(session, accountID, positionDate);
            }
        }

        public static IPortfolioHistorical GetHistoricalPortfolio(IDalSession session, int accountID, DateTime positionDate)
        {
            IList<IHistoricalPrice> prices = HistoricalPriceMapper.GetHistoricalPrices(session, positionDate);
            IList<IHistoricalExRate> rates = HistoricalExRateMapper.GetHistoricalExRates(session, positionDate);
            IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountID);

            if (prices == null || prices.Count == 0)
                throw new ApplicationException(string.Format("No prices available on {0}", positionDate.ToShortDateString()));
            return GetHistoricalPortfolio(session, account, positionDate, prices, rates);

        }
        public static IPortfolioHistorical GetHistoricalPortfolio(IDalSession session, IAccountTypeInternal account,
            DateTime positionDate, IList<IHistoricalPrice> prices, IList<IHistoricalExRate> rates)
        {
            IList<IFundPositionTx> posTx = FundPositionMapper.GetPositionTransactionsByDate(session, account, positionDate);
            IList<IJournalEntryLine> lines = JournalEntryMapper.GetSettledBookingsForAccountUntilDate(session, positionDate, account.Key);
            IPortfolioHistorical portfolio = new PortfolioHistorical(account, positionDate, posTx, prices, rates, lines);
            return portfolio;
        }

        public static IList<HistoricalPositionRowView> GetHistoricalPositions(IDalSession session, int accountID, DateTime positionDate)
        {

            if (accountID != 0 && positionDate < DateTime.Now)
            {
                IPortfolioHistorical portfolio = GetHistoricalPortfolio(session, accountID, positionDate);
                return GetHistoricalPositions(session, portfolio);
            }
            else
                return new List<HistoricalPositionRowView>();
        }

        public static IList<HistoricalPositionRowView> GetHistoricalPositions(IDalSession session, IPortfolioHistorical portfolio)
        {
            IAccountTypeInternal account = portfolio.ParentAccount;


            List<HistoricalPositionRowView> positionRowViews = portfolio.FundPortfolio
                                                                       .Select(p => new HistoricalPositionRowView(p))
                                                                       .ToList();
            positionRowViews.AddRange(portfolio.CashPortfolio
                                                .Select(c => new HistoricalPositionRowView(c))
                                                .ToList());
            decimal totalValue = 0m;
            if (positionRowViews.Count > 0)
                totalValue = positionRowViews.Where(j => (j.Value != null)).Select(v => v.Value).Sum().Quantity;
            if (totalValue != 0m)
                foreach (HistoricalPositionRowView rowView in positionRowViews.Where(pv => pv.Value != null))
                    rowView.Percentage = rowView.Value.Quantity / totalValue;

            return positionRowViews;
        }


        public static DataSet GetHistoricalPositions(int accountID, DateTime positionDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<HistoricalPositionRowView> positionRowViews = GetHistoricalPositions(session, accountID, positionDate);
                return positionRowViews.ToDataSet();
            }
        }
    }
}
