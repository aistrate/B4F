using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Valuations.ReportedData;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Accounts.Portfolios;
using System.Data;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.GeneralLedger.Journal.Maintenance;
using B4F.TotalGiro.ApplicationLayer.Portfolio;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class EndYearManagementAdapter
    {
        public static void CloseAccountsInBookYear(int bookYear, int bookYearClosureID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ReportingPeriodDetail reportingPeriodDetail = new ReportingPeriodDetail(EndTermType.FourthQtr, bookYear);
            
            IList<int> listOfAccounts = AccountMapper.GetAccountKeystoCloseFinancialYear(session, reportingPeriodDetail.EndTermYear);
            session.Close();

            foreach (int iAccount in listOfAccounts)
            {
            session = NHSessionFactory.CreateSession();
            IList<IJournalEntryLine> lines = JournalEntryMapper.GetBookingsToCloseForFinancialYear(session, reportingPeriodDetail.GetEndDate(), iAccount);
            IAccountTypeInternal customer = (IAccountTypeInternal) AccountMapper.GetAccount(session, iAccount);
            IBookYearClosure closure = BookYearClosureMapper.GetBookYearClosure(session, bookYearClosureID);
            IClientBookYearClosure clientClosure = new ClientBookYearClosure(customer);
            clientClosure.ParentClosure = closure;


            if (lines.Count > 0)
            {

                //First Calculate the Totals per Currency for the account.
                IList<IGLAccount> balanceAccounts = GLAccountMapper.GetClientBalanceGLAccounts(session);
                var Total = from l in lines
                            group new { balanceTotal = l.Balance } by l.Balance.Underlying.Key into g
                            let firstchoice = new
                            {
                                currencyID = g.Key,
                                balanceTotal = g.Select(c => c.balanceTotal).Sum()
                            }
                            where (firstchoice.balanceTotal).IsNotZero
                            select firstchoice;

                foreach (var balance in Total)
                {
                    IGLAccount account = balanceAccounts.Where(a => a.DefaultCurrency.Key == balance.balanceTotal.Underlying.Key).First();
                    IJournalEntryLine newLine = new JournalEntryLine();
                    newLine.GLAccount = account;
                    newLine.GiroAccount = customer;
                    newLine.Balance = balance.balanceTotal.Negate();
                    lines.Add(newLine);
                }

                //Now summarize and total each line
                var summary = from s in lines
                              group new { Balance = s.Balance, Glaccount = s.GLAccount, GiroAccount = s.GiroAccount } by
                              new
                              {
                                  glaccount = s.GLAccount.Key,
                                  giroacct = s.GiroAccount.Key,
                                  Currency = ((ICurrency)s.Credit.Underlying).Key
                              }
                                  into g
                                  let firstchoice = new
                                  {

                                      glaccount = g.Key.glaccount,
                                      giroacct = g.Key.giroacct,
                                      Glaccount = g.First().Glaccount,
                                      GiroAccount = g.First().GiroAccount,
                                      Total = g.Select(m => m.Balance).Sum()
                                  }
                                  where (firstchoice.Total).IsNotZero
                                  select firstchoice;

                //Create new negative entries
                if (summary.Count() != 0)
                {
                    //Create a new Booking for the Next Year
                    DateTime bookDate = reportingPeriodDetail.GetEndDate().AddDays(1);
                    int journalAdmin = int.Parse(Utils.ConfigSettingsInfo.GetInfo("DefaultClientAdminJournal"));
                    int newBookingKey = (int) MemorialBookingsAdapter.CreateMemorialBooking(journalAdmin);
                    IMemorialBooking newBooking =(IMemorialBooking) JournalEntryMapper.GetJournalEntry(session, newBookingKey);
                    string description = string.Format(@"{0} - afsluiting Boekjaar {1}", customer.Number, reportingPeriodDetail.EndTermYear.ToString());
                        
                    newBooking.TransactionDate = bookDate;
                    newBooking.Description = description;                    
                    
                    foreach (var unit in summary.OrderBy(a => a.Glaccount.GLAccountNumber))
                    {
                        IJournalEntryLine newLine = new JournalEntryLine();
                        newLine.GLAccount = unit.Glaccount;
                        newLine.GiroAccount = customer;
                        newLine.Balance = unit.Total.Negate();
                        newBooking.Lines.AddJournalEntryLine(newLine);
                    }

                    newBooking.BookLines();
                    clientClosure.ClosureBooking = newBooking;
                    clientClosure.ClosureNotRequired = false;

                    session.InsertOrUpdate(newBooking);
                }
            }
        
            if (clientClosure.ClosureBooking == null) clientClosure.ClosureNotRequired = true;
            session.InsertOrUpdate(clientClosure);
            session.Close();
            }

        }

        public static IGLBookYear GetCurrentBookYear()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                B4F.TotalGiro.Stichting.IEffectenGiro giro = B4F.TotalGiro.Stichting.ManagementCompanyMapper.GetEffectenGiroCompany(session);
                return giro.CurrentBookYear;

            }

        }


        public static void StoreEndTermValues(ReportingPeriodDetail reportingPeriodDetail)
        {
            DateTime endDate = reportingPeriodDetail.GetEndDate();
            DateTime startDate = new DateTime(reportingPeriodDetail.EndTermYear, 1, 1);

            IDalSession session1 = NHSessionFactory.CreateSession();
            IInternalEmployeeLogin employee = LoginMapper.GetCurrentEmployee(session1);
            IList<int> listOfAccounts = AccountMapper.GetAccountKeysActiveinFinancialYear(session1, reportingPeriodDetail.EndTermYear);
            IList<IHistoricalPrice> prices = HistoricalPriceMapper.GetHistoricalPrices(session1, endDate);
            IList<IHistoricalExRate> rates = HistoricalExRateMapper.GetHistoricalExRates(session1, endDate);

            IList<IJournalEntryLine> dividends = null;
            if ((reportingPeriodDetail.TermType == EndTermType.FourthQtr) || (reportingPeriodDetail.TermType == EndTermType.FullYear))
                dividends = JournalEntryMapper.GetDividendBookingsForPeriod(session1, startDate, endDate);
            IPeriodicReporting reportingPeriod = getReportingPeriod(session1, reportingPeriodDetail, endDate, employee.UserName);
            session1.InsertOrUpdate(reportingPeriod);
            int reportingPeriodID = reportingPeriod.Key;



            foreach (int i in listOfAccounts)
            {
                IDalSession session2 = NHSessionFactory.CreateSession();
                IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session2, i);
                reportingPeriod = PeriodicReportingMapper.GetReportingPeriod(session2, reportingPeriodID);
                if (!account.EndTermValues.EndTermValueExists(reportingPeriodDetail))
                {
                    IPortfolioHistorical portfolio = HistoricalPositionAdapter.GetHistoricalPortfolio(session2, account, endDate, prices, rates);
                    InsertEndTermValue(session2, portfolio, dividends, reportingPeriod);
                }
                session2.Close();
            }
            session1.Close();
        }

        private static IPeriodicReporting getReportingPeriod(IDalSession session, ReportingPeriodDetail reportingPeriodDetail, DateTime endTermDate, string currentUser)
        {
            IPeriodicReporting reportingPeriod = PeriodicReportingMapper.GetReportingPeriod(session, reportingPeriodDetail);
            if (reportingPeriod == null)
                reportingPeriod = new PeriodicReporting(reportingPeriodDetail, endTermDate, currentUser);
            return reportingPeriod;
        }

        private static void InsertEndTermValue(IDalSession session, IPortfolioHistorical portfolio, IList<IJournalEntryLine> dividends, IPeriodicReporting reportingPeriod)
        {
            IAccountTypeInternal account = portfolio.ParentAccount;
            IEndTermValue etv = new EndTermValue(account, reportingPeriod);

            Money InternalDividend = new Money(0m, account.BaseCurrency);
            Money InternalDividendTax = new Money(0m, account.BaseCurrency);
            Money ExternalDividend = new Money(0m, account.BaseCurrency);
            Money ExternalDividendTax = new Money(0m, account.BaseCurrency);

            if (dividends != null)
            {
                List<IJournalEntryLine> divs = dividends.ToList();
                if (divs.Exists(d => d.GiroAccount.Key == account.Key))
                {
                    if (divs.Exists(d => d.GLAccount.IsGrossDividendInternal))
                        InternalDividend = divs.Where(d => (d.GLAccount.IsGrossDividendInternal && (d.GiroAccount.Key == account.Key))).Select(m => m.Balance.BaseAmount).Sum();
                    if (divs.Exists(d => d.GLAccount.IsDividendTaxInternal))
                        InternalDividendTax = divs.Where(d => (d.GLAccount.IsDividendTaxInternal && (d.GiroAccount.Key == account.Key))).Select(m => m.Balance.BaseAmount).Sum();
                    if (divs.Exists(d => d.GLAccount.IsGrossDividendExternal))
                        ExternalDividend = divs.Where(d => (d.GLAccount.IsGrossDividendExternal && (d.GiroAccount.Key == account.Key))).Select(m => m.Balance.BaseAmount).Sum();
                    if (divs.Exists(d => d.GLAccount.IsDividendTaxExternal))
                        ExternalDividendTax = divs.Where(d => (d.GLAccount.IsDividendTaxExternal && (d.GiroAccount.Key == account.Key))).Select(m => m.Balance.BaseAmount).Sum();
                }
            }

            etv.CashValue = portfolio.CashPortfolio.TotalPortfolioValue;
            etv.FundValue = portfolio.FundPortfolio.TotalPortfolioValue;
            etv.ClosingValue = etv.FundValue + etv.CashValue;
            etv.CultureFundValue = portfolio.FundPortfolio.CultureFundValue.Abs();
            etv.GreenFundValue = portfolio.FundPortfolio.GreenFundValue.Abs();
            etv.InternalDividend = InternalDividend.Abs();
            etv.InternalDividendTax = InternalDividendTax.Abs();
            etv.ExternalDividend = ExternalDividend.Abs();
            etv.ExternalDividendTax = ExternalDividendTax.Abs();
            session.InsertOrUpdate(etv);
        }

        public static IPeriodicReporting GetLastReportingPeriod()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return PeriodicReportingMapper.GetLastReportingPeriod(session);
            }
        }

        public static DataSet GetPeriodicReporting()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return PeriodicReportingMapper.GetReportedPeriods(session)
                    .Select(r => new
                    {
                        r.Key,
                        r.EndTermYear,
                        Description = r.ToString(),
                        EndDate = r.EndTermDate.ToLongDateString(),
                        CreatedOn = r.CreationDate.ToLongDateString(),
                        r.CreatedBy
                    }).ToDataSet();

            }

        }

        //public static 
    }
}
