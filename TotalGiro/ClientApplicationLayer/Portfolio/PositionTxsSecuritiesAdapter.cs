using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ClientApplicationLayer.Portfolio
{
    public static class PositionTxsSecuritiesAdapter
    {
        public static DataSet GetRelatedAccountFundPositions(int selectedPositionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IFundPosition selectedPosition = SecurityLayerAdapter.GetOwnedFundPosition(session, selectedPositionId);

                PositionsView view = (selectedPosition.Size.Quantity != 0m ? PositionsView.NotZero : PositionsView.Zero);

                return SecurityLayerAdapter.GetOwnedFundPositions(session, selectedPosition.Account.Key, view)
                                           .OrderBy(p => p.InstrumentOfPosition.DisplayName)
                                           .Select(p => new { p.Key, p.InstrumentDescription })
                                           .ToDataSet();
            }
        }

        public static void GetFundPositionDetails(int positionId, out string accountDescription, out string valueDisplayString)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IFundPosition position = SecurityLayerAdapter.GetOwnedFundPosition(session, positionId);

                accountDescription = position.Account.DisplayNumberWithName;
                valueDisplayString = position.CurrentBaseValue != null ? position.CurrentBaseValue.DisplayString : "";
            }
        }

        public static DataSet GetFundPositionTxs(int positionId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IFundPosition position = SecurityLayerAdapter.GetOwnedFundPosition(session, positionId);

                return ApplicationLayer.Portfolio.PositionTransactionsAdapter.GetPositionTxsSecurity(session, position, false);
            }
        }
    }
}
