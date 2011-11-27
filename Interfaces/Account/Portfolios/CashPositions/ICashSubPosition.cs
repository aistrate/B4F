using System;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public interface ICashSubPosition
    {
        DateTime CreationDate { get; set; }
        IGLJournalEntryLineCollection JournalLines { get; }
        int Key { get; set; }
        CashPositionSettleStatus SettledFlag { get; }
        bool IsSettled { get; }
        Money Size { get; set; }
        Money SizeInBaseCurrency { get; }
        ICashPosition ParentPosition { get; set; }
    }
}
