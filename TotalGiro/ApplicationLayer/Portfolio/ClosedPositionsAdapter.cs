using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class ClosedPositionsAdapter
    {
        //public static DataSet GetClosedCashPositions(int accountId)
        //{
        //    IDalSession session = NHSessionFactory.CreateSession();

        //    IAccountTypeInternal account = AccountMapper.GetAccount(session, accountId) as IAccountTypeInternal;
        //    IList positions = AccountMapper.GetPositions(session, account, PositionReturnClass.CashPosition, PositionsView.Zero);

        //    DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
        //                                    positions,
        //                                    "Key, Instrument.DisplayName, ExchangeRate.Rate");

        //    session.Close();

        //    return ds;
        //}

        public static DataSet GetClosedSecurityPositions(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeInternal account = AccountMapper.GetAccount(session, accountId) as IAccountTypeInternal;

                return GetClosedSecurityPositions(session, account);
            }
        }

        public static DataSet GetClosedSecurityPositions(IDalSession session, IAccountTypeInternal account)
        {
            return FundPositionMapper.GetPositions(session, account, PositionsView.Zero)
                                     .Select(p => new PositionRowView(p))
                                     .Select(pv => new
                                     {
                                         pv.Key,
                                         pv.Isin,
                                         pv.InstrumentName,
                                         pv.Price,
                                         PriceShortDisplayString =
                                            pv.Price.ShortDisplayString,
                                         pv.ExchangeRate
                                     })
                                     .ToDataSet();
        }

        public static string GetAccountDescription(int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IAccount account = AccountMapper.GetAccount(session, accountId);
            string accountDescription = string.Format("{0} ({1})", account.Number, account.ShortName);

            session.Close();
            return accountDescription;
        }
    }
}
