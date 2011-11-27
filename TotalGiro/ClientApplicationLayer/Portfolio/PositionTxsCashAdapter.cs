using System.Data;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ClientApplicationLayer.Portfolio
{
    public static class PositionTxsCashAdapter
    {
        public static void GetCashPositionDetails(int accountId, out string accountDescription, out string valueDisplayString)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICashPosition position = SecurityLayerAdapter.GetOwnedCashPosition(session, accountId);

                accountDescription = position.Account.DisplayNumberWithName;
                valueDisplayString = position.SettledSizeInBaseCurrency != null ? position.SettledSizeInBaseCurrency.DisplayString : "";
            }
        }

        public static DataView GetCashMutations(int accountId, string sortColumn)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICashSubPosition subposition = SecurityLayerAdapter.GetOwnedCashSubposition(session, accountId);

                return ApplicationLayer.Portfolio.CashPositionTransactionsAdapter.GetCashPositionTransactions(session, subposition, sortColumn);
            }
        }
    }
}
