using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.GeneralLedger.Journal;
using System.Collections;
using B4F.TotalGiro.GeneralLedger.Journal.Balances;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public enum TrialBalanceType
    {
        Nett = 0,
        Full,
        Group,
        Exact
    }

    public static class TrialBalanceAdapter
    {


        public static DataSet GetTrialBalance(DateTime transactionDate, TrialBalanceType BalanceType)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<BalanceRecord> records = null;
                TrialBalance trialBalance = TrialBalanceMapper.GetTrialBalance(session, transactionDate);
                if (trialBalance != null)
                {
                        switch (BalanceType)
                        {
                            case TrialBalanceType.Nett:
                                records = trialBalance.Records.Where(n => n.Balance.IsNotZero).Cast<BalanceRecord>().ToList();
                                foreach (TrialBalanceRecord tr in records)
                                    tr.Balance = tr.Balance;
                                break;
                            case TrialBalanceType.Exact:
                                records = getExactBalance(trialBalance.Records);
                                break;
                            default:
                                records = trialBalance.Records.Cast<BalanceRecord>().ToList();
                                break;
                        }
                }
                else
                    records = new List<BalanceRecord>();

                return records.ToList()
                            .Select(c => new
                            {
                                c.Key,
                                c.LineNumber,
                                Account_GLAccountNumber =
                                    c.AccountNumber,
                                Account_FullDescription =
                                    c.FullDescription,
                                c.Debit,
                                c.DebitDisplayString,
                                c.Credit,
                                c.CreditDisplayString
                            })
                            .ToDataSet();
            }
        }

        private static IList<BalanceRecord> getExactBalance(IList<TrialBalanceRecord> records)
        {
            List<ExactBalanceRecord> returnValue = new List<ExactBalanceRecord>();
            DateTime transactionDate = records[0].TransactionDate;

            var result = from r in records
                         group new { Balance1 = r.Balance, accountinExact = r.Account.AccountinExact }
                         by new { exact = r.Account.AccountinExact.Key, currency = r.Debit.Underlying.Key } into g
                         let Balance = from pair in g select pair.Balance1
                         let Exact = from pair in g select pair.accountinExact
                         let firstChoice = new
                                               {
                                                   Exact = Exact.First(),
                                                   Balance = Balance.Sum()
                                               }

                         where (firstChoice.Balance.IsNotZero)
                         select firstChoice;
                        

            foreach (var b in result.OrderBy(r => r.Exact.AccountNumber))
            {
                ExactBalanceRecord newRecord = new ExactBalanceRecord(b.Exact, b.Balance, transactionDate);
                returnValue.Add(newRecord);
                newRecord.LineNumber = returnValue.Count;
            }


            return returnValue.Cast<BalanceRecord>().ToList();

        }
    }
}
