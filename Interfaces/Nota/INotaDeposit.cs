using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Notas
{
    public interface INotaDeposit : INota
    {
        string TegenrekeningNumber { get; }
        CashTransferDetailTypes CashTransferDetailType { get; }
        bool IsWithdrawalOneOff { get; }
        bool IsWithdrawalPeriodic { get; }
        bool IsWithdrawalTermination { get; }
        bool IsDeposit { get; }
        Money TransferFee { get; }
    }
}
