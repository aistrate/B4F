using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class BankStatementJournalsAdapter
    {
        public static DataSet GetBankStatementJournals()
        {
            // @"Key, JournalNumber, BankAccountNumber, BankAccountDescription, FixedGLAccount.GLAccountNumber, 
            //  ManagementCompany.CompanyName, Currency.Symbol, DateLastBooked, Balance");
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return JournalMapper.GetJournals(session, JournalTypes.BankStatement)
                    .Select(c => new
                    {
                        c.Key,
                        c.JournalNumber, 
                        c.BankAccountNumber, 
                        c.BankAccountDescription, 
                        FixedGLAccount_GLAccountNumber = 
                            c.FixedGLAccount.GLAccountNumber, 
                        ManagementCompany_CompanyName = 
                            c.ManagementCompany.CompanyName, 
                        Currency_Symbol =
                            c.Currency.Symbol, 
                        c.DateLastBooked, 
                        c.Balance
                    })
                    .ToDataSet();
            }
        }
    }
}
