using System;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts;
namespace B4F.TotalGiro.GeneralLedger.Journal
{
    public interface IClientBookYearClosures
    {
        IJournalEntry Booking { get; set; }
        IGLBookYear BookYear { get; set; }
        IAccountTypeInternal GiroAccountID { get; set; }
        int Key { get; set; }
    }
}
