using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.ClientApplicationLayer.Charts;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Utils.Linq;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.ClientApplicationLayer.Planning
{
    public static class FinancialPlannerAdapter
    {
        public static DataSet GetContactAccounts(bool hasEmptyFirstRow, int contactId)
        {
            DataSet ds = CommonAdapter.GetContactAccounts(contactId);

            return hasEmptyFirstRow ? ds.AddEmptyFirstRow() : ds;
        }

        public static DataSet GetModelPortfolios(bool hasEmptyFirstRow)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = SecurityLayerAdapter.GetOwnedModels(session)
                                                 .Where(m => m.ExpectedReturn != 0m)
                                                 .Select(m => new { m.Key, m.ModelName })
                                                 .ToDataSet();

                return hasEmptyFirstRow ? ds.AddEmptyFirstRow() : ds;
            }
        }

        public static FinancialDataView GetFinancialDataFromAccount(int accountId, bool allowMissingExpectedReturn)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return new FinancialDataView(SecurityLayerAdapter.GetOwnedActiveAccount(session, accountId), allowMissingExpectedReturn);
            }
        }

        public static FinancialDataView GetFinancialDataFromModel(int modelId, bool allowMissingExpectedReturn)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return new FinancialDataView(SecurityLayerAdapter.GetOwnedModel(session, modelId), allowMissingExpectedReturn);
            }
        }

        public static DataSet GetMonths()
        {
            return EnumerableExtensions.Range(0, 12)
                                       .Select(m => new
                                       {
                                           Key = m, 
                                           Value = m > 0 ? (new DateTime(DateTime.Today.Year, m, 1)).ToString("MMMM") : "(maand)"
                                       })
                                       .ToDataSet();
        }

        public static DataSet GetYears()
        {
            int currentYear = DateTime.Today.Year;

            return Enumerable.Range(currentYear, 51)
                             .Select(y => new
                             {
                                 Key = y,
                                 Value = y > currentYear ? y.ToString() : "(jaar)"
                             })
                             .ToDataSet();
        }

        public static decimal GetProposedPeriodical(decimal presentValue, decimal proposedPeriodicalGuess, int yearsLeft,
                                                    decimal expectedReturn, decimal standardDeviation, decimal targetValue)
        {
            return FinancialMath.GoalSeek(x => GetChanceOfMeetingTarget(presentValue, x, yearsLeft,
                                                                        expectedReturn, standardDeviation, targetValue),
                                          0.8m, proposedPeriodicalGuess, 8);
        }

        public static decimal GetProposedInitial(decimal proposedInitialGuess, decimal depositPerYear, int yearsLeft,
                                                 decimal expectedReturn, decimal standardDeviation, decimal targetValue)
        {
            return FinancialMath.GoalSeek(x => GetChanceOfMeetingTarget(x, depositPerYear, yearsLeft,
                                                                        expectedReturn, standardDeviation, targetValue),
                                          0.8m, proposedInitialGuess, 8);
        }

        public static decimal GetChanceOfMeetingTarget(decimal presentValue, decimal depositPerYear, int yearsLeft,
                                                       decimal expectedReturn, decimal standardDeviation, decimal targetValue)
        {
            InvestmentScenario scenario = new InvestmentScenario(presentValue, depositPerYear, yearsLeft,
                                                                 expectedReturn, standardDeviation, targetValue);

            return scenario.ChanceOfMeetingTarget;
        }

        public static decimal GetIdealInterestRate(decimal presentValue, decimal depositPerYear, int yearsLeft, 
                                                   decimal idealInterestRateGuess, decimal targetValue)
        {
            return FinancialMath.GoalSeek(x => GetFutureValue(presentValue, depositPerYear, yearsLeft, x),
                                          targetValue, idealInterestRateGuess);
        }

        public static decimal GetFutureValue(decimal presentValue, decimal depositPerYear, int yearsLeft, decimal expectedReturn)
        {
            InvestmentScenario scenario = new InvestmentScenario(presentValue, depositPerYear, yearsLeft, expectedReturn, 0m);

            return scenario.LastValue;
        }

        public static string FormatCurrency(decimal quantity, string altSymbol)
        {
            return Currency.DisplayToString(quantity, altSymbol);
        }

        public static string GetBaseCurrencyAltSymbol()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetBaseCurrency(session).AltSymbol;
            }
        }

        public static decimal[] GetFutureValueSeries(decimal presentValue, decimal depositPerYear, int yearsLeft, decimal expectedReturn)
        {
            InvestmentScenario scenario = new InvestmentScenario(presentValue, depositPerYear, yearsLeft, expectedReturn, 0m);

            return scenario.Values;
        }

        public static decimal[][] GetVolatilitySeries(decimal presentValue, decimal depositPerYear, int yearsLeft,
                                                      decimal expectedReturn, decimal standardDeviation, decimal stdDevRangeMultiplier)
        {
            InvestmentScenario scenario = new InvestmentScenario(presentValue, depositPerYear, yearsLeft, expectedReturn, standardDeviation);
            scenario.StdDevRangeMultiplier = stdDevRangeMultiplier;
            
            return new decimal[][] { scenario.UnfavorableValues, scenario.FavorableValues };
        }

        public static DateTime[] GetRangeOfDates(int yearsLeft)
        {
            return EnumerableExtensions.Range(0, yearsLeft)
                                       .Select(y => DateTime.Today.AddYears(y))
                                       .ToArray();
        }

        public static DateTime[] GetValuationDates(int accountId, int yearsLeft)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICustomerAccount account = SecurityLayerAdapter.GetOwnedActiveAccount(session, accountId);

                double dateIncrement = ChartsAdapter.GetDateIncrement(account.FirstTransactionDate, DateTime.Today.AddYears(yearsLeft), 125);

                return ChartsAdapter.GenerateDates(account.FirstTransactionDate, account.LastValuationDate, dateIncrement);
            }
        }

        public static List<Tuple<DateTime, decimal>> GetValuationsTotalPortfolio(int accountId, DateTime[] dates)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedValuationsTotalPortfolio(session, accountId, dates)
                                           .Select(v => Tuple.Create(v.Date, v.TotalValue.Quantity))
                                           .ToList();
            }
        }

        public static bool SaveAccountFinancialTarget(int accountId, decimal depositPerYear, DateTime targetEndDate, decimal targetValue)
        {
            int baseCurrencyId;
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                baseCurrencyId = SecurityLayerAdapter.GetBaseCurrency(session).Key;
            }

            AccountFinancialTargetHelper helper = new AccountFinancialTargetHelper();
            helper.ParentAccountID = accountId;
            helper.CurrencyID = baseCurrencyId;
            helper.TargetAmountSize = targetValue;
            helper.DepositPerYearSize = depositPerYear;
            helper.TargetEndDate = targetEndDate;

            return AccountEditAdapter.AddAccountFinanicalTarget(helper);
        }
    }
}
