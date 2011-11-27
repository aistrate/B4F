using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.GeneralLedger.Journal.Maintenance
{
    public class ClientBookYearClosure : IClientBookYearClosure
    {
        public ClientBookYearClosure() { }
        public ClientBookYearClosure(IAccountTypeInternal giroAccountID) 
        {
            this.GiroAccountID = giroAccountID;

        }

        public int Key { get; set; }
        public IJournalEntry ClosureBooking { get; set; }
        public IAccountTypeInternal GiroAccountID { get; set; }
        public IBookYearClosure ParentClosure { get; set; }
        public bool ClosureNotRequired { get; set; }
        public DateTime CreationDate { get { return this.creationDate.HasValue ? this.creationDate.Value : DateTime.MinValue; } }

        #region privates

        private DateTime? creationDate;

        #endregion

    }
}
