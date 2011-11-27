using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Communicator.Exact;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public enum JournalTypes
    {
        /// <summary>
        /// BB
        /// </summary>
        BankStatement = 1,
        /// <summary>
        /// MM
        /// </summary>
        Memorial = 2,
        /// <summary>
        /// VB
        /// </summary>
        ClientTransaction = 3,

    }

    public interface IJournal
    {
        int Key { get; set; }
        JournalTypes JournalType { get; set; }
        string JournalNumber { get; set; }
        string BankAccountNumber { get; set; }
        string BankAccountDescription { get; set; }
        IGLAccount FixedGLAccount { get; set; }
        IManagementCompany ManagementCompany { get; set; }
        ICurrency Currency { get; set; }
        IBankStatement LastBankStatement { get; }
        Money Balance { get; }
        DateTime DateLastBooked { get; }
        string FullDescription { get; }
        IExactJournal ExactJournal { get; set; }
        bool ShowManualAllowedGLAccountsOnly { get; set; }
        IList<IJournalEntry> OpenEntries { get; }
        bool IsAdminAccount { get; set; }
        bool IsSystem { get; set; }
    }
}
