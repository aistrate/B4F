using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public static class GLAccountMapper
    {
        public static IList<IGLAccount> GetGLAccounts(IDalSession session)
        {
            return session.GetTypedListByNamedQuery<IGLAccount>(
                "B4F.TotalGiro.GeneralLedger.Static.GetGLAccounts");
        }

        public static IList<IGLAccount> GetGLAccounts(IDalSession session, bool isFixed)
        {
            return GetGLAccounts(session, isFixed, false);
        }

        public static IList<IGLAccount> GetGLAccounts(IDalSession session, bool isFixed, bool showAllowedManualOnly)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("isFixed", isFixed);
            if (showAllowedManualOnly)
                parameters.Add("isAllowedManual", true);
            return session.GetTypedListByNamedQuery<IGLAccount>(
                "B4F.TotalGiro.GeneralLedger.Static.GetGLAccounts",
                parameters);
        }

        public static IGLAccount GetGLAccount(IDalSession session, int glAccountId)
        {
            IGLAccount glAccount = null;

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", glAccountId));
            IList<IGLAccount> list = session.GetTypedList<GLAccount, IGLAccount>(expressions);
            if (list != null && list.Count == 1)
                glAccount = list[0];

            return glAccount;
        }

        public static IGLAccount GetDepositGLAccount(IDalSession session)
        {
            IGLAccount glAccount = null;

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("IsDefaultDeposit", true));
            IList<IGLAccount> list = session.GetTypedList<GLAccount, IGLAccount>(expressions);
            if (list != null && list.Count == 1)
                glAccount = list[0];
            else
                throw new ApplicationException("Lookup of Default Deposit GL Account produced either no account or more than one.");

            return glAccount;
        }

        public static IGLAccount GetWithdrawalGLAccount(IDalSession session)
        {
            IGLAccount glAccount = null;

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("IsDefaultWithdrawal", true));
            IList<IGLAccount> list = session.GetTypedList<GLAccount, IGLAccount>(expressions);
            if (list != null && list.Count == 1)
                glAccount = list[0];
            else
                throw new ApplicationException("Lookup of Default Withdrawal GL Account produced either no account or more than one.");

            return glAccount;
        }

        public static IList<IGLAccount> GetClientBalanceGLAccounts(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("IsClientOpenBalance", true));
            IList<IGLAccount> list = session.GetTypedList<GLAccount, IGLAccount>(expressions);
            return list;
        }


        public static IGLAccount GetSettlementDifferenceGLAccount(IDalSession session, ICurrency currency, ITradeableInstrument instrument)
        {
            IGLAccount glAccount = null;
            if (instrument.SettlementDifferenceAccount != null)
                glAccount = instrument.SettlementDifferenceAccount;
            else
            {
                List<ICriterion> expressions = new List<ICriterion>();
                expressions.Add(Expression.Eq("IsSettlementDifference", true));
                IList<IGLAccount> list = session.GetTypedList<GLAccount, IGLAccount>(expressions);
                if (list != null)
                {
                    if (list.Count == 1)
                        glAccount = list[0];
                    else if (list.Count > 1)
                        glAccount = list.Where(x => x.DefaultCurrency.Equals(currency)).FirstOrDefault();
                }
            }
            
            if (glAccount == null)
                throw new ApplicationException("Lookup of Settlement Difference GL Account produced no account.");
            else
                return glAccount;
        }
    }
}
