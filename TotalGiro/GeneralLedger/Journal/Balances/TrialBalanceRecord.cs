using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.GeneralLedger.Journal.Balances
{
    public class TrialBalanceRecord : BalanceRecord
    {
        public TrialBalanceRecord() { }
        public IGLAccount Account { get; set; }
        public override string AccountNumber { get { return this.Account.GLAccountNumber; } }
        public override string FullDescription { get { return this.Account.FullDescription; } }

    }
}
