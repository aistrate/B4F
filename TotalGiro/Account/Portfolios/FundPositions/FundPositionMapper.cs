using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
using NHibernate.Criterion;
using System;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public static class FundPositionMapper
    {
        /// <summary>
        /// This method retrieves a list of Position instances that belong to a specific account. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="account">The account the positions belong to</param>
        /// <returns>A list of position instances</returns>
        public static List<IFundPosition> GetPositions(IDalSession session, IAccountTypeInternal account)
        {
            return GetPositions(session, account, PositionsView.NotZero);
        }

        /// <summary>
        /// This method retrieves a list of Position instances that belong to a specific account and meet the PositionsView criterium. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="account">The account the positions belong to</param>
        /// <param name="view">Determines which positions to show (zero side, non zero sized or all)</param>
        /// <returns>A list of position instances</returns>
        public static List<IFundPosition> GetPositions(IDalSession session, IAccountTypeInternal account, PositionsView view)
        {
            if (account != null)
                return GetPositions(session, account.Key, view);
            else
                return new List<IFundPosition>();
        }

        public static List<IFundPosition> GetPositions(IDalSession session, int accountId, PositionsView view)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Account.Key", accountId));
            addPositionsViewCriterion(expressions, view);
            return session.GetTypedList<FundPosition, IFundPosition>(expressions);
        }

        public static List<IFundPosition> GetPositions(IDalSession session, IInstrument instrument, PositionsView view)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Size.Underlying.Key", instrument.Key));
            addPositionsViewCriterion(expressions, view);
            LoginMapper.GetSecurityInfo(session, expressions, SecurityInfoOptions.NoFilter);
            return session.GetTypedList<FundPosition, IFundPosition>(expressions);
        }

        /// <summary>
        /// This method retrieves a list of Position instances that are uniquely identified by one of the unique identifiers in the passed-in array. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="positionIds">An array with uniquely identifiers</param>
        /// <returns>A list of position instances</returns>
        public static List<IFundPosition> GetPositions(IDalSession session, int[] positionIds)
        {
            return session.GetTypedList<FundPosition, IFundPosition>(new List<ICriterion>() { Expression.In("Key", positionIds) });
        }

        public static List<IFundPosition> GetPositionsByParentInstrument(IDalSession session, int accountId, int[] parentInstrumentIds, 
                                                                         PositionsView view)
        {
            Hashtable parameters = new Hashtable();
            Hashtable listParameters = new Hashtable();
            
            parameters.Add("accountId", accountId);
            
            parameters.Add("positionsViewZero", view == PositionsView.Zero ? 1 : 0);
            parameters.Add("positionsViewNotZero", view == PositionsView.NotZero ? 1 : 0);
            parameters.Add("positionsViewAll", view == PositionsView.All ? 1 : 0);

            listParameters.Add("parentInstrumentIds", parentInstrumentIds);
            return session.GetTypedListByNamedQuery<IFundPosition>(
                "B4F.TotalGiro.Accounts.Portfolios.FundPositions.GetFundPositions",
                parameters,
                listParameters);
        }

        public static IFundPosition GetPosition(IDalSession session, int positionId)
        {
            return session.GetTypedList<FundPosition, IFundPosition>(new List<ICriterion>() { Expression.Eq("Key", positionId) })
                          .FirstOrDefault();
        }

        private static void addPositionsViewCriterion(List<ICriterion> expressions, PositionsView view)
        {
            ICriterion quantityZero = Expression.Eq("Size.Quantity", 0m);

            if (view == PositionsView.Zero)
                expressions.Add(quantityZero);
            else if (view == PositionsView.NotZero)
                expressions.Add(Expression.Not(quantityZero));
        }

      
        
        #region PositionTransactions

        public static List<IFundPositionTx> GetPositionTransactions(IDalSession session, IFundPosition position)
        {
            return GetPositionTransactions(session, position, true);
        }

        public static List<IFundPositionTx> GetPositionTransactions(IDalSession session, IFundPosition position, bool retrieveNonClientDisplayable)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parameterLists = new Hashtable(1);
            parameters.Add("accountId", position.Account.Key);
            if (!retrieveNonClientDisplayable)
                parameters.Add("showStornos", false);
            parameterLists.Add("pedigree", position.InstrumentOfPosition.GetInstrumentPedigree()
                                                                        .Cast<IInstrument>().Select(i => i.Key).ToList());
            
            return session.GetTypedListByNamedQuery<IFundPositionTx>(
                "B4F.TotalGiro.Accounts.Portfolios.FundPositions.GetFundPositionTxs",
                parameters,
                parameterLists);
        }

        public static List<IFundPositionTx> GetPositionTransactions(IDalSession session, IFundPosition position, int[] positionTxIds)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ParentPosition.Key", position.Key));
            expressions.Add(Expression.In("Key", positionTxIds));
            return session.GetTypedList<FundPositionTx, IFundPositionTx>(expressions);
        }

        public static List<IFundPositionTx> GetPositionTransactions(IDalSession session, IAccount account)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("ParentPosition.Account.Key", account.Key));
            return session.GetTypedList<FundPositionTx, IFundPositionTx>(expressions);
        }

        public static List<IFundPositionTx> GetPositionTransactionsByDate(IDalSession session, IAccount account, DateTime positionDate)
        {
            Hashtable parameters = new Hashtable(2);
            parameters.Add("accountId", account.Key);

            parameters.Add("positionDate", positionDate);

            return session.GetTypedListByNamedQuery<IFundPositionTx>(
                "B4F.TotalGiro.Accounts.Portfolios.FundPositions.GetPositionTransactionsByDate",
                parameters);
        }

        public static List<IFundPositionTx> GetPositionTransactionsByDate(IDalSession session, int accountId, int instrumentId, DateTime positionDate)
        {
            Hashtable parameters = new Hashtable(3);
            parameters.Add("accountId", accountId);
            parameters.Add("instrumentId", instrumentId);
            parameters.Add("positionDate", positionDate);

            return session.GetTypedListByNamedQuery<IFundPositionTx>(
                "B4F.TotalGiro.Accounts.Portfolios.FundPositions.GetPositionTransactionsByDate",
                parameters);
        }

        #endregion
    }
}
