using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.GeneralLedger
{
    public static class JournalsAdapter
    {

        public static DataSet GetGLJournals()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return JournalMapper.GetJournals(session).Select(a => new
                {
                    a.Key,
                    a.JournalNumber,
                    a.JournalType,
                    a.BankAccountDescription,
                    a.BankAccountNumber,                    
                    FixedGLAccount = (a.FixedGLAccount != null ?   a.FixedGLAccount.GLAccountNumber : "")
                }).ToDataSet();
            }
        }

        public static int SaveGLJournalDetails(GLJournalDetails details)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IJournal journal = null;
                if (details.Key == 0)
                {
                    journal = new Journal();
                    journal.JournalType = details.JournalType;
                    journal.IsSystem = false;                    
                }
                else
                    journal = JournalMapper.GetJournal(session, details.Key);

                journal.JournalNumber = details.JournalNumber;
                journal.BankAccountDescription = details.Description;

                if (details.JournalType == JournalTypes.BankStatement)
                {
                    journal.BankAccountNumber = details.BankAccountNumber;
                    if (details.Key == 0) journal.FixedGLAccount = CreateNewFixedAccount("");
                }

                session.InsertOrUpdate(journal);

                 return journal.Key;

            }

        }

        public static IGLAccount CreateNewFixedAccount(string accountNumber)
        {
            IGLAccount newAccount = new GLAccount();
            return newAccount;

        }

        public static GLJournalDetails GetGLJournalDetails(int journalID)
        {
            GLJournalDetails details = new GLJournalDetails();
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IJournal journal = JournalMapper.GetJournal(session, journalID);

                details = new GLJournalDetails(journal);                
                
            }
            return details;
        }

        public static DataSet GetJournalTypes()
        {
            return (Enum.GetValues(typeof(JournalTypes))).Cast<JournalTypes>()
                .Select(e => new
                {
                    Key = (int)e,
                    Description = e.ToString()
                })
                .ToDataSet();
        }




        public class GLJournalDetails
        {
            public GLJournalDetails() { }
            public GLJournalDetails(IJournal journal)
            {
                this.Key = journal.Key;
                this.JournalNumber = journal.JournalNumber;
                this.BankAccountNumber = journal.BankAccountNumber;
                this.Description = journal.BankAccountDescription;
                this.JournalType = journal.JournalType;
                this.GLAccountKey = journal.FixedGLAccount != null ? journal.FixedGLAccount.Key : 0 ;
                this.FixedAccount = journal.FixedGLAccount != null ? journal.FixedGLAccount.FullDescription : "";


            }
            public int Key;
            public string JournalNumber;
            public JournalTypes JournalType;
            public string Description;
            public string BankAccountNumber;
            public int GLAccountKey;
            public string FixedAccount;
        }
    }
}

