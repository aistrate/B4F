using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class MemorialJournalsAdapter
    {
        public static DataSet GetMemorialJournals()
        {
            // "Key, JournalNumber, BankAccountDescription, ManagementCompany.CompanyName, Currency.Symbol"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return JournalMapper.GetJournals(session, JournalTypes.Memorial)
                    .Select(c => new
                    {
                        c.Key,
                        c.JournalNumber,
                        c.BankAccountDescription,
                        ManagementCompany_CompanyName =
                            c.ManagementCompany.CompanyName,
                        Currency_Symbol =
                        c.Currency.Symbol,
                        OpenEntries =
                            c.OpenEntries.Count
                    })
                    .ToDataSet();
            }
        }
    }
}
