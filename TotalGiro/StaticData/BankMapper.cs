using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.StaticData
{
    public static class BankMapper
    {
        public static IBank GetBank(IDalSession session, int bankID)
        {
            return (IBank)session.GetObjectInstance(typeof(Bank), bankID);
        }

        public static IBank GetBank(IDalSession session, string bankName)
        {
            IBank bank = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Like("Name", bankName));
            IList list = session.GetList(typeof(Bank), expressions);
            if (list != null && list.Count == 1)
                bank = (IBank)list[0];
            return bank;
        }

        public static IList GetBanks(IDalSession session)
        {
            return session.GetList(typeof(Bank));
        }
    }
}
