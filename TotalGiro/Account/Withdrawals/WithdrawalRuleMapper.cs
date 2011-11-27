using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.Withdrawals
{
    public class WithdrawalRuleMapper
    {
        public static IList<IWithdrawalRule> GetWithdrawalRules(IDalSession session, int accountID)
        {
            return GetWithdrawalRules(session, accountID, ActivityReturnFilter.Active);
        }

        public static IList<IWithdrawalRule> GetWithdrawalRules(IDalSession session, int accountID, ActivityReturnFilter activityFilter)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            IAccountTypeCustomer acc = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountID);
            if (acc != null)
            {
                expressions.Add(Expression.Eq("Account.Key", acc.Key));
                if (activityFilter != ActivityReturnFilter.All)
                    expressions.Add(Expression.Eq("IsActive", (activityFilter == ActivityReturnFilter.Active ? true : false)));
                return session.GetTypedList<WithdrawalRule, IWithdrawalRule>(expressions);
            }
            else
                return null;
        }

        public static IWithdrawalRule GetWithdrawalRule(IDalSession session, int id)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            IList rules = session.GetList(typeof(WithdrawalRule), expressions);
            if (rules != null && rules.Count == 1)
                return (IWithdrawalRule)rules[0];
            else
                return null;
        }

        public static IList<WithdrawalRuleRegularity> GetWithdrawalRuleRegularities(IDalSession session)
        {
            return session.GetTypedList<WithdrawalRuleRegularity>();
        }

        public static WithdrawalRuleRegularity GetWithdrawalRuleRegularity(IDalSession session, Regularities id)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("key", (int)id));
            IList<WithdrawalRuleRegularity> rules = session.GetTypedList<WithdrawalRuleRegularity>(expressions);
            if (rules != null && rules.Count == 1)
                return rules[0];
            else
                return null;
        }

        public static void Insert(IDalSession session, IWithdrawalRule obj)
        {
            session.InsertOrUpdate(obj);
        }

        public static void Update(IDalSession session, IWithdrawalRule obj)
        {
            session.InsertOrUpdate(obj);
        }

        public static void Delete(IDalSession session, IWithdrawalRule obj)
        {
            session.Delete(obj);
        }

    }
}
