using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public enum LedgerTypes
    {
        ClientTransactions = 1,     // VB
        MemorialBookings = 2,       // MM
        Mutations = 3               // BB
    }
    
    public interface ILedgerType
    {
        int Key { get; set; }
        string Type { get; set; }
        string Description { get; set; }
        LedgerTypes TypeOfLedger { get;   }
    }
}
