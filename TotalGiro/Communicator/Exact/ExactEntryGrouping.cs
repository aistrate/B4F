using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ExactEntryGrouping
    {
        public ExactEntryGrouping(object[] grouping)
        {
            this.Key = (int)grouping[0];
            //this.LedgerType = (ILedgerType)grouping[1];
            this.TransactionDate = (DateTime)grouping[1];
        }
            public int Key { get; set; }
            //public ILedgerType LedgerType { get; set; }
            public DateTime TransactionDate { get; set; }
    }
}
