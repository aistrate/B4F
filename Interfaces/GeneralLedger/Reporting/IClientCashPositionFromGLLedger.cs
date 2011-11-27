using System;
namespace B4F.TotalGiro.GeneralLedger.Journal.Reporting
{
    public interface IClientCashPositionFromGLLedger
    {
        DateTime Key { get; set; }
        IClientCashPositionFromGLLedgerRecordCollection Records { get; }
    }
}
