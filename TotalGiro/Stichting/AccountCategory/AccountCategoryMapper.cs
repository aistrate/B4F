using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Stichting
{
    public class AccountCategoryMapper
    {
        public static IList GetAccountCategories(IDalSession session, IAssetManager am)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("AssetManager.Key", am.Key));
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Asc("CustomerType"));
            return session.GetList(typeof(AccountCategory), expressions, orderings, null, null);
        }
    }
}
