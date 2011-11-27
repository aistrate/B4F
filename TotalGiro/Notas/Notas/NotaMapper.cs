using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Notas
{
    public enum NotaReturnClass
    {
        Nota,
        NotaTransaction,
        NotaTransfer,
        NotaFees,
        NotaDeposit,
        NotaDividend,
        NotaInstrumentConversion
    }

    public class NotaMapper
    {
        public static INota GetNota(IDalSession session, int notaId)
        {
            INota nota = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", notaId));
            IList list = session.GetList(typeof(Nota), expressions);
            if (list != null && list.Count == 1)
                nota = (INota)list[0];
            return nota;
        }

        public static List<INota> GetNotas(IDalSession session, int[] notaIds)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", notaIds));
            return session.GetTypedList<Nota, INota>(expressions);
        }

//        public static IList GetNotas(IDalSession session, NotaReturnClass returnClass, 
//                                     IAccountTypeInternal account, DateTime startDate, DateTime endDate)
//        {
//            Hashtable parameters = new Hashtable();
//            parameters.Add("Account", account);
//            parameters.Add("StartDate", startDate);
//            parameters.Add("EndDate", endDate);


//            return session.GetListByHQL(string.Format(@"FROM {0} N WHERE N.UnderlyingTx.AccountA = :Account 
//                                                                     AND N.UnderlyingTx.TransactionDate >= :StartDate 
//                                                                     AND N.UnderlyingTx.TransactionDate <= :EndDate
//                                                        ORDER BY N.UnderlyingTx.TransactionDate, N.NotaNumber",
//                                                      getType(returnClass).Name),
//                                        parameters);
//        }

        public static IList GetNotas(IDalSession session, NotaReturnClass returnClass)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            return session.GetList(getType(returnClass), expressions);
        }

        //public static IList GetUnprintedNotas(IDalSession session)
        //{
        //    List<ICriterion> expressions = new List<ICriterion>();
        //    expressions.Add(Expression.Eq("PrintCount", 0));
        //    return session.GetList(typeof(Nota), expressions);
        //}

//        public static List<IManagementCompany> GetMgmtCompaniesWithUnprintedNotas(IDalSession session, 
//                                                                                  NotaReturnClass returnClass, int[] managementCompanyIds)
//        {
//            string hql = string.Format(@"FROM ManagementCompany M WHERE M IN
//                                            (SELECT A.AccountOwner FROM CustomerAccount A WHERE A IN 
//                                                (SELECT N.UnderlyingTx.AccountA FROM {0} N WHERE N.PrintCount = 0))",
//                                       getType(returnClass).Name);

//            if (managementCompanyIds != null && managementCompanyIds.Length > 0)
//            {
//                string idList = "";
//                for (int i = 0; i < managementCompanyIds.Length; i++)
//                    idList += (i > 0 ? ", " : "") + managementCompanyIds[i].ToString();

//                hql += string.Format(" AND M IN ({0})", idList);
//            }

//            return NHSession.ToList<IManagementCompany>(session.GetListByHQL(hql, new Hashtable()));
//        }

        public static int[] GetAccountsIdsWithUnprintedNotas(IDalSession session, int managementCompanyId, NotaReturnClass returnClass)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("ManagementCompanyId", managementCompanyId);
            string underlyingAccObj = "";
            switch (returnClass)
            {
                case NotaReturnClass.NotaTransaction:
                case NotaReturnClass.NotaTransfer:
                case NotaReturnClass.NotaInstrumentConversion:
                    underlyingAccObj = "UnderlyingTx.AccountA";
                    break;
                default:
                    underlyingAccObj = "UnderlyingBooking.Account";
                    break;
            }

            ArrayList accountIds = (ArrayList)session.GetListByHQL(
                string.Format(@"SELECT A.Key FROM CustomerAccount A 
                                WHERE A.Key IN (SELECT N.{0}.Key FROM {1} N WHERE N.PrintCount = 0)
                                AND A.AccountOwner.Key = :ManagementCompanyId
                                ORDER BY A.Number",
                                underlyingAccObj,
                                getType(returnClass).Name),
                                parameters);
            return (int[])accountIds.ToArray(typeof(int));
        }

        public static List<INota> GetUnprintedNotasByAccount(IDalSession session, int accountId, NotaReturnClass returnClass)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("AccountId", accountId);
            string underlyingAccObj = "";
            string underlyingDateObj = "";
            switch (returnClass)
            {
                case NotaReturnClass.NotaTransaction:
                case NotaReturnClass.NotaTransfer:
                case NotaReturnClass.NotaInstrumentConversion:
                    underlyingAccObj = "UnderlyingTx.AccountA";
                    underlyingDateObj = "UnderlyingTx.TransactionDate";
                    break;
                default:
                    underlyingAccObj = "UnderlyingBooking.Account";
                    underlyingDateObj = "UnderlyingBooking.GeneralOpsJournalEntry.TransactionDate";
                    break;
            }

            return NHSession.ToList<INota>(session.GetListByHQL(
                string.Format(@"FROM {0} N WHERE N.{1}.Key = :AccountId AND N.PrintCount = 0 
                                ORDER BY N.{2}, N.NotaNumber", 
                            getType(returnClass).Name,
                            underlyingAccObj,
                            underlyingDateObj),
                            parameters));
        }

        #region CRUD

        public static bool Update(IDalSession session, INota obj)
        {
            return session.InsertOrUpdate(obj);
        }

        public static bool Update(IDalSession session, IList list)
        {
            return session.InsertOrUpdate(list);
        }

        #endregion

        #region Helpers

        private static Type getType(NotaReturnClass returnClass)
        {
            Type type;
            switch (returnClass)
            {
                case NotaReturnClass.NotaTransaction:
                    type = typeof(NotaTransaction);
                    break;
                case NotaReturnClass.NotaTransfer:
                    type = typeof(NotaTransfer);
                    break;
                case NotaReturnClass.NotaFees:
                    type = typeof(NotaFees);
                    break;
                case NotaReturnClass.NotaDeposit:
                    type = typeof(NotaDeposit);
                    break;
                case NotaReturnClass.NotaDividend:
                    type = typeof(NotaDividend);
                    break;
                case NotaReturnClass.NotaInstrumentConversion:
                    type = typeof(NotaInstrumentConversion);
                    break;
                default:
                    type = typeof(Nota);
                    break;
            }
            return type;
        }

        #endregion
    }
}
