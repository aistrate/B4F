using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ExactJournal : IExactJournal
    {
        public ExactJournal() { }

        public int Key { get; set; }
        public ILedgerType LedgerType { get; set; }
        public string JournalNumber { get; set; }

    }
}
