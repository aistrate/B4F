using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IImportedBankBalance
    {
        int Key { get; set; }
        IJournal BankJournal { get; set; }
        DateTime BookBalanceDate { get; set; }
        Money BookBalance { get; set; }
    }
}
