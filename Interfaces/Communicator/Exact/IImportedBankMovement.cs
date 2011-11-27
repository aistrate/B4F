using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IImportedBankMovement
    {
        int Key { get; set; }
        IJournal BankJournal { get; set; }
        DateTime BankStatementDate { get; set; }
        DateTime CloseBalanceProcessDate { get; set; }
        string CloseBalanceProcessTime { get; set; }
        //string MovementCurrCode { get; set; }
        Money MovementAmount { get; set; }
        string Description { get; set; }
    }
}
