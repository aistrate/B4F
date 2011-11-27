using System;
using B4F.TotalGiro.Accounts;
namespace B4F.TotalGiro.GeneralLedger.Journal.Maintenance
{
    public interface IClientBookYearClosure
    {
        IJournalEntry ClosureBooking { get; set; }
        bool ClosureNotRequired { get; set; }
        DateTime CreationDate { get; }
        IAccountTypeInternal GiroAccountID { get; set; }
        int Key { get; set; }
        IBookYearClosure ParentClosure { get; set; }
    }
}
