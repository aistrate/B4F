using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using System.Collections;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.ApplicationLayer.BackOffice;

namespace B4F.TotalGiro.ApplicationLayer.Compliance
{
    public static class ApproveStornosAdapter
    {
        public static DataSet GetStornoTransactions()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<ITransaction> stornoes = session.GetTypedListByNamedQuery<ITransaction>(
                    "B4F.TotalGiro.Orders.Transactions.Transaction.GetUnApprovedStornoes");

                //"Key, CreatedBy, AccountA.Number, AccountA.ShortName, TxSide, ValueSize, ValueSize.DisplayString, Price, PriceShortDisplayString, CounterValueSize, CounterValueSize.DisplayString, ExchangeRate, Description, TransactionDate, CreationDate, Reason"
                return stornoes
                    .Select(c => new
                    {
                        c.Key,
                        c.CreatedBy,
                        AccountA_Key = c.AccountA.Key,
                        AccountA_Number = c.AccountA.Number,
                        AccountA_ShortName = c.AccountA.ShortName,
                        c.TxSide,
                        c.ValueSize,
                        ValueSize_DisplayString = c.ValueSize.DisplayString,
                        c.Price,
                        PriceShortDisplayString = (c.Price != null ? c.Price.ShortDisplayString : ""),
                        c.CounterValueSize,
                        CounterValueSize_DisplayString = (c.CounterValueSize != null ? c.CounterValueSize.DisplayString : ""),
                        c.ExchangeRate,
                        c.Description,
                        c.TransactionDate,
                        c.CreationDate,
                        Reason = c.StornoReason
                    })
                    .ToDataSet();
            }
        }
        
        public static Exception[] ApproveStornoTransactions(int[] stornoTxIds)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ArrayList exceptions = new ArrayList();
                IInternalEmployeeLogin employee = LoginMapper.GetCurrentEmployee(session);

                foreach (ITransaction stornoTx in TransactionMapper.GetTransactions<ITransaction>(session, stornoTxIds))
                {
                    try
                    {
                        stornoTx.Approve(employee, true);

                        BondCouponAccrualAdapter.StornoBondTransaction(session, stornoTx, employee);
                        TransactionMapper.Update(session, stornoTx);
                    }
                    catch (ApplicationException ex)
                    {
                        exceptions.Add(new ApplicationException(string.Format("Transaction {0}:", stornoTx.Key), ex));
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new ApplicationException(string.Format("Error approving transaction {0}:", stornoTx.Key), ex));
                    }
                }

                return (Exception[])exceptions.ToArray(typeof(Exception));
            }
        }

        public static void DisapproveStornoTransactions(int[] stornoTxIds)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                foreach (ITransaction stornoTx in TransactionMapper.GetTransactions<ITransaction>(session, stornoTxIds))
                {
                    try
                    {
                        TransactionMapper.Delete(session, stornoTx);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(string.Format("Error disapproving transaction {0}:", stornoTx.Key), ex);
                    }
                }
            }
        }
    }
}
