using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public static class CashPositionMapper
    {
        /// <summary>
        /// This method retrieves a list of CashPosition instances that are uniquely identified by one of the unique identifiers in the passed-in array. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="positionIds">An array with uniquely identifiers</param>
        /// <returns>A list of position instances</returns>
        public static List<ICashPosition> GetPositions(IDalSession session, ICollection positionIds)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", positionIds));
            return session.GetTypedList<CashPosition, ICashPosition>(expressions);
        }

        public static List<ICashPosition> GetPositions(IDalSession session, IAccountTypeInternal account, PositionsView view)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Account.Key", account.Key));
            addPositionsViewCriterion(expressions, view);
            return session.GetTypedList<CashPosition, ICashPosition>(expressions);
        }

        public static ICashPosition GetPosition(IDalSession session, int positionId)
        {
            return session.GetTypedList<CashPosition, ICashPosition>(new List<ICriterion>() { Expression.Eq("Key", positionId) })
                          .FirstOrDefault();
        }

        public static ICashPosition GetPosition(IDalSession session, int accountId, int currencyId)
        {
            return session.GetTypedList<CashPosition, ICashPosition>(new List<ICriterion>()
                                                                     {
                                                                         Expression.Eq("Account.Key", accountId),
                                                                         Expression.Eq("PositionCurrency.Key", currencyId)
                                                                     })
                          .FirstOrDefault();
        }
        
        public static ICashSubPosition GetSubPosition(IDalSession session, int subPositionId)
        {
            return session.GetTypedList<CashSubPosition, ICashSubPosition>(new List<ICriterion>() { Expression.Eq("Key", subPositionId) })
                          .FirstOrDefault();
        }

        private static void addPositionsViewCriterion(List<ICriterion> expressions, PositionsView view)
        {
            ICriterion quantityZero = Expression.Eq("SettledSize.Quantity", 0m);

            if (view == PositionsView.Zero)
                expressions.Add(quantityZero);
            else if (view == PositionsView.NotZero)
                expressions.Add(Expression.Not(quantityZero));
        }
    }
}
