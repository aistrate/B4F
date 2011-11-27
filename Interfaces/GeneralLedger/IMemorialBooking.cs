using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface IMemorialBooking : IJournalEntry
    {
        string Description { get; set; }
    }
}
