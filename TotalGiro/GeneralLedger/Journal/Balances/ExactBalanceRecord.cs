using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Communicator.Exact;

namespace B4F.TotalGiro.GeneralLedger.Journal.Balances
{
    public class ExactBalanceRecord : BalanceRecord
    {
        public ExactBalanceRecord(IExactAccount account, Money balance, DateTime transactionDate)
        {
            this.Account = account;
            this.Balance = balance;
            this.TransactionDate = transactionDate;
        }
        public IExactAccount Account { get; set; }
        public override string AccountNumber { get { return this.Account.AccountNumber; } }
        public override string FullDescription { get { return this.Account.FullDescription; } }
    }
}
