using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts;
using System.Data.SqlTypes;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class CashPositionTransactionsAdapter
    {
        public class CashPosTxHelper
        {
            public CashPosTxHelper(int key, string transactionType, 
                string searchKey, string fullDescription, decimal amountQuantity, 
                string displayAmount, DateTime transactionDate, 
                DateTime creationDate, Money saldo)
            {
                Key = key;
                TransactionType = transactionType;
                SearchKey = searchKey;
                FullDescription = fullDescription;
                AmountQuantity = amountQuantity;
                DisplayAmount = displayAmount;
                TransactionDate= transactionDate;
                CreationDate= creationDate;
                Saldo = saldo;
            }

            public int Key { get; set; }
            public string TransactionType { get; set; }
            public string SearchKey { get; set; }
            public string FullDescription { get; set; }
            public decimal AmountQuantity { get; set; }
            public string DisplayAmount { get; set; }
            public DateTime TransactionDate { get; set; }
            public DateTime CreationDate { get; set; }
            public Money Saldo { get; set; }
        }
        
        public static DataView GetCashPositionTransactions(int positionId, DateTime beginDate, DateTime endDate, string sortColumn)
        {
            if (positionId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    ICashSubPosition position = CashPositionMapper.GetSubPosition(session, positionId);

                    if (position != null)
                        return GetCashPositionTransactions(session, position, beginDate, endDate, true, sortColumn);
                    else
                        throw new ApplicationException(string.Format("Could not find cash position with ID '{0}'.", positionId));
                }
            }
            return null;
        }

        public static DataView GetCashPositionTransactions(IDalSession session, ICashSubPosition position,
                                                           DateTime beginDate, DateTime endDate,
                                                           bool retrieveNonClientDisplayable, string sortColumn)
        {
            string[] sortExprs = sortColumn.Split(' ');
            string sortDirection = "ASC";
            if (sortExprs.Length > 1)
                sortDirection = sortExprs[1];

            if (Util.IsNullDate(beginDate))
                beginDate = SqlDateTime.MinValue.Value;
            if (Util.IsNullDate(endDate))
                endDate = SqlDateTime.MaxValue.Value;

            DataSet ds = getCashPosTxDetails(session, position, retrieveNonClientDisplayable)
                .Where(x => x.TransactionDate >= beginDate && x.TransactionDate <= endDate)
                .Select(ptx => new
                {
                    ptx.Key,
                    ptx.TransactionType,
                    ptx.SearchKey,
                    ptx.FullDescription,
                    ptx.AmountQuantity,
                    ptx.DisplayAmount,
                    ptx.Saldo,
                    SaldoQuantity = ptx.Saldo.Quantity,
                    ptx.TransactionDate,
                    ptx.CreationDate
                })
                .ToDataSet();

            DataView dv = ds.Tables[0].DefaultView;
            string sortExpression = sortExprs[0] + " " + sortDirection;
            if (sortExprs[0].ToLower() == "transactiondate")
                sortExpression += ", Key " + sortDirection;
            dv.Sort = sortExpression;
            return dv;
        }

        /// <summary>
        /// Used by the client site.
        /// </summary>
        public static DataView GetCashPositionTransactions(IDalSession session, ICashSubPosition position, string sortColumn)
        {
            return GetCashPositionTransactions(session, position, SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value, false, sortColumn);
        }

        public static DataSet GetCashPositionTransactionsForExport(int positionId, DateTime beginDate, DateTime endDate)
        {
            if (positionId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    if (Util.IsNullDate(beginDate))
                        beginDate = SqlDateTime.MinValue.Value;
                    if (Util.IsNullDate(endDate))
                        endDate = SqlDateTime.MaxValue.Value; 
                    ICashSubPosition position = CashPositionMapper.GetSubPosition(session, positionId);

                    if (position != null)
                        return getCashPosTxDetails(session, position, false)
                            .Where(x => x.TransactionDate >= beginDate && x.TransactionDate <= endDate)
                            .Select(ptx => new
                            {
                                Reference = ptx.SearchKey,
                                TxType = ptx.TransactionType,
                                Description = ptx.FullDescription,
                                Amount = ptx.AmountQuantity,
                                Saldo = ptx.Saldo.Quantity,
                                ptx.TransactionDate,
                                ptx.CreationDate
                            })
                            .ToDataSet();
                    else
                        throw new ApplicationException(string.Format("Could not find cash position with ID '{0}'.", positionId));
                }
            }
            return null;
        }

        private static List<CashPosTxHelper> getCashPosTxDetails(IDalSession session, ICashSubPosition position, bool retrieveNonClientDisplayable)
        {
            Money runningTotal = position.ParentPosition.CurrentValue.ZeroedAmount();
            List<ICashMutationView> list = AccountMapper.GetCashPositionTransactions(
                session, position,
                SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value,
                retrieveNonClientDisplayable);

            return list
                .OrderBy(ptx => ptx.TransactionDate)
                .ThenBy(ptx => ptx.Key)
                .Select(ptx => new CashPosTxHelper(
                    ptx.Key,
                    ptx.TransactionType,
                    ptx.SearchKey,
                    ptx.FullDescription,
                    ptx.Amount.Quantity,
                    ptx.DisplayAmount,
                    ptx.TransactionDate,
                    ptx.CreationDate,
                    runningTotal += ptx.Amount)
                ).ToList();
        }


        public static void GetSubPositionDetails(int subPositionId,
                                      out string accountDescription, out string instrumentDescription, out string valueDisplayString)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ICashSubPosition subPosition = CashPositionMapper.GetSubPosition(session, subPositionId);
            accountDescription = string.Format("{0} ({1})", subPosition.ParentPosition.Account.Number, subPosition.ParentPosition.Account.ShortName);
            instrumentDescription = string.Format("{0} ({1}) {2}",
                ((ICurrency)subPosition.ParentPosition.Instrument).AltSymbol,
                ((ICurrency)(subPosition.ParentPosition.Instrument)).DisplayName,
                subPosition.SettledFlag == CashPositionSettleStatus.UnSettled ? "unsettled" : "" );
            valueDisplayString = (subPosition.Size != null ? subPosition.Size.DisplayString : "");

            session.Close();
        }


    }
}
