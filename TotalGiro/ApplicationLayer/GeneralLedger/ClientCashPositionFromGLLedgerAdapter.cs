using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.GeneralLedger.Journal.Reporting;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class ClientCashPositionFromGLLedgerAdapter
    {
        public static DataSet GetClientCashPositionFromGLLedger(DateTime transactionDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IEnumerable<IClientCashPositionFromGLLedgerRecord> records = GetClientCashPositionFromGLLedgerSource(session, transactionDate);

                if (records != null)
                {
                    ds = records.ToList()
                        .Select(c => new
                        {
                            c.Key,
                            c.LineNumber,
                            Account_Key =
                                c.InternalAccount.Key,
                            Account_AccountNumber =
                                c.InternalAccount.Number,
                            Account_FullDescription =
                                c.InternalAccount.ShortName,
                            c.Debit,
                            c.DebitDisplayString,
                            c.Credit,
                            c.CreditDisplayString
                        })
                        .ToDataSet();
                }
                return ds;
            }
        }

        private static IEnumerable<IClientCashPositionFromGLLedgerRecord> GetClientCashPositionFromGLLedgerSource(IDalSession session, DateTime transactionDate)
        {
            IEnumerable<IClientCashPositionFromGLLedgerRecord> records = null;
            ClientCashPositionFromGLLedger cashPosition = ClientCashPositionFromGLLedgerMapper.GetClientCashPositionFromGLLedger(session, transactionDate);
            if (cashPosition != null)
                records = cashPosition.Records;
            return records;
        }

        public static DataSet GetClientCashPositionFromGLLedgerDownload(DateTime transactionDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IEnumerable<IClientCashPositionFromGLLedgerRecord> records = GetClientCashPositionFromGLLedgerSource(session, transactionDate);

                if (records != null)
                {
                    ds = records.ToList()
                        .Select(c => new
                        {
                            c.LineNumber,
                            Account_AccountNumber =
                                c.InternalAccount.Number,
                            Account_FullDescription =
                                c.InternalAccount.ShortName,
                            Debit = c.Debit.Quantity,
                            Credit = c.Credit.Quantity
                        })
                        .ToDataSet();
                }
                return ds;
            }
        }


    }
}
