using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Orders.Transactions
{
    //public enum TransactionReturnClass
    //{
    //    Transaction = 0,
    //    OrderAllocation = 1,
    //    OrderExecution = 2,
    //    TransactionNTM = 3,
    //    CrumbleTransaction = 8,
    //    CorporateAction = 9
    //}

    public static class TransactionMapper
    {
        public static ITransaction GetTransaction(IDalSession session, int tradeId)
        {
            ITransaction transaction = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", tradeId));
            IList<ITransaction> list = session.GetTypedList<Transaction, ITransaction>(expressions);
            if (list != null && list.Count == 1)
                transaction = list[0];
            return transaction;
        }

        public static IList<T> GetTransactions<T>(IDalSession session, int[] tradeIds)
            where T : ITransaction
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.In("Key", tradeIds));
            return session.GetTypedList<Transaction>(expressions).Cast<T>().ToList();
        }

        public static IList<ITransactionType> GetTransactionTypes(IDalSession session)
        {
            return session.GetTypedList<TransactionType, ITransactionType>();
        }

        public static int[] GetNotarizableTransactionIds(
            IDalSession session, TransactionTypes transactionType,
            int managementCompanyId, int accountId)
        {
            Hashtable parameters = new Hashtable();

            if (transactionType != TransactionTypes.Transaction)
                parameters.Add("tradeTypeId", (int)transactionType);

            //// TODO: this is TEMPORARY, until Bonus Emission nota is created
            //if (transactionType == TransactionReturnClass.CorporateAction)
            //{
            //    parameters.Add("CorporateActionType", (int)OldCorporateActionTypes.Conversion);
            //    hql += " AND T.CorporateActionType = :CorporateActionType";
            //}

            if (managementCompanyId != 0)
                parameters.Add("managementCompanyId", managementCompanyId);

            if (accountId != 0)
                parameters.Add("accountId", accountId);

            IList<int> transactionIds = session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.Orders.Transactions.GetNotarizableTransactionIds",
                parameters);
            return transactionIds.ToArray();
        }

        public static IList<int> GetUnmigratedTransactions(IDalSession session)
        {
            string hql = @"Select T.Key
                            from Transaction T
                            where T.TempMigrationFlag = 0";

            IList<int> transactionIds = session.GetTypedListByHQL<int>(hql);
            return transactionIds;

        }


        #region CRUD

        public static bool Update(IDalSession session, ITransaction obj)
        {
            return session.InsertOrUpdate(obj);
        }

        public static bool Delete(IDalSession session, ITransaction obj)
        {
            bool retVal = false;

            if (obj.Approved)
                throw new Exception(string.Format("Trade (ID: {0}) is already approved and can not be deleted.", obj.Key.ToString()));

            try
            {
                if (obj.TransactionType == TransactionTypes.Execution)
                {
                    OrderExecution transaction = (OrderExecution)obj;
                    if (transaction.Order != null)
                    {
                        IStgOrder order = (IStgOrder)transaction.Order;
                        if (order.RemoveTransaction(transaction))
                        {
                            if (session.BeginTransaction())
                            {
                                if (session.Delete(obj))
                                {
                                    if (session.Update(order))
                                        retVal = session.CommitTransaction();
                                }
                            }
                        }
                    }
                }
                else if (obj.IsStorno)
                {
                    ITransaction stornoTx = (ITransaction)obj;
                    ITransaction stornoed = stornoTx.OriginalTransaction;
                    stornoTx.OriginalTransaction = null;
                    switch (stornoTx.TransactionType)
                    {
                        case TransactionTypes.Allocation:
                        case TransactionTypes.Execution:
                            ((ITransactionOrder)stornoTx).Order = null;
                            break;
                    }
                    if (stornoed != null)
                    {
                        stornoed.StornoTransaction = null;
                        if (session.BeginTransaction())
                        {
                            if (session.Delete(obj))
                            {
                                if (session.Update(stornoed))
                                    retVal = session.CommitTransaction();
                            }
                        }
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex.InnerException);
            }
        }

        #endregion
    }
}
