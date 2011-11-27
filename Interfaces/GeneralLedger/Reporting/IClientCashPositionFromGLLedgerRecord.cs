using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.GeneralLedger.Journal.Reporting
{
    public interface IClientCashPositionFromGLLedgerRecord
    {
        DateTime BookDate { get; set; }
        IAccountTypeInternal InternalAccount { get; set; }
        int Key { get; set; }
        int LineNumber { get; set; }
        Money Credit { get; set; }
        string CreditDisplayString { get;  }
        Money Debit { get; set; }
        string DebitDisplayString { get; }
    }
}
