using System;
namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IExactJournal
    {
        string JournalNumber { get; set; }
        int Key { get; set; }
        ILedgerType LedgerType { get; set; }
    }
}
