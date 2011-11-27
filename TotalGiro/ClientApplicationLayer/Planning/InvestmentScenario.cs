using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Utils.Linq;

namespace B4F.TotalGiro.ClientApplicationLayer.Planning
{
    /// <summary>
    /// Calculated properties are cached.
    /// </summary>
    public class InvestmentScenario
    {
        #region Constructors and immutable properties

        public InvestmentScenario(decimal presentValue, decimal depositPerYear, int yearsLeft,
                                  decimal expectedReturn, decimal standardDeviation)
            : this(expectedReturn, standardDeviation)
        {
            CashFlow = Enumerable.Repeat(presentValue, 1)
               .Concat(Enumerable.Repeat(depositPerYear, yearsLeft))
               .ToArray();
        }

        /// <summary>
        /// A non-zero targetValue should only be used when calculating ChanceOfMeetingTarget, because that property 
        /// is based on InternalRateOfReturn, which needs both deposits and withdrawals to calculate properly.
        /// Calculations of time sequences (e.g., the Values property) must use a targetValue equal to zero.
        /// </summary>
        public InvestmentScenario(decimal presentValue, decimal depositPerYear, int yearsLeft,
                                  decimal expectedReturn, decimal standardDeviation, decimal targetValue)
            : this(expectedReturn, standardDeviation)
        {
            CashFlow = Enumerable.Repeat(presentValue + depositPerYear / 2, 1)
               .Concat(Enumerable.Repeat(depositPerYear, yearsLeft - 1))
               .Concat(Enumerable.Repeat(-targetValue + depositPerYear / 2, 1))
               .ToArray();
        }

        public InvestmentScenario(IEnumerable<decimal> cashFlow, decimal expectedReturn, decimal standardDeviation)
            : this(expectedReturn, standardDeviation)
        {
            if (cashFlow != null)
                CashFlow = cashFlow.ToArray();
        }

        private InvestmentScenario(decimal expectedReturn, decimal standardDeviation)
        {
            ExpectedReturn = expectedReturn;
            StandardDeviation = standardDeviation;
        }
        
        public decimal[] CashFlow { get; private set; }

        public decimal ExpectedReturn { get; private set; }
        public decimal StandardDeviation { get; private set; }

        public bool AutoCorrelation { get { return true; } }
        public int DepositsPerYear { get { return 12; } }
        public int WithdrawalsPerYear { get { return 12; } }

        #endregion


        #region Time sequences

        public decimal LastValue { get { return Values.LastOrDefault(); } }

        public decimal[] Values
        {
            get
            {
                if (values == null)
                    values = calculateValues(CashFlow, ExpectedReturn);

                return values;
            }
        }
        private decimal[] values;

        private decimal[] calculateValues(decimal[] deposits, decimal expectedReturn)
        {
            decimal[] expectedReturns = Enumerable.Repeat(expectedReturn, deposits.Length)
                                                  .ToArray();
            
            return calculateValues(deposits, expectedReturns);
        }

        private decimal[] calculateValues(decimal[] deposits, decimal[] expectedReturns)
        {
            if (deposits.Length > 0)
            {
                decimal currentValue = deposits.First();

                List<decimal> valueList = new List<decimal>();
                valueList.Add(currentValue);

                foreach (var pair in deposits.Zip(expectedReturns).Skip(1))
                {
                    currentValue = nextValue(currentValue, pair.Item1, pair.Item2);
                    valueList.Add(currentValue);
                }

                return valueList.ToArray();
            }
            else
                return new decimal[] { };
        }

        private decimal nextValue(decimal currentValue, decimal nextDeposit, decimal interestRate)
        {
            if (DepositsPerYear == 1)
                return (currentValue + nextDeposit) * (1 + interestRate);
            else
                return (currentValue + 0.5m * nextDeposit) * (1 + interestRate) + 0.5m * nextDeposit;
        }

        public decimal[] YearsPassed
        {
            get
            {
                if (yearsPassed == null)
                    yearsPassed = EnumerableExtensions.Range(0, CashFlow.Length - 1)
                                                      .Select(y => (decimal)y)
                                                      .ToArray();

                return yearsPassed;
            }
        }
        private decimal[] yearsPassed;

        public decimal[] StdDevsOfTheMean
        {
            get
            {
                if (stdDevsOfTheMean == null)
                    stdDevsOfTheMean = YearsPassed.Select(y => y == 0m ? StandardDeviation :
                                                                         calcAutoCorrelation(y) * StandardDeviation / FinancialMath.Sqrt(y))
                                                  .ToArray();

                return stdDevsOfTheMean;
            }
        }
        private decimal[] stdDevsOfTheMean;

        private decimal calcAutoCorrelation(decimal yearsPassed)
        {
            if (AutoCorrelation)
                return 1m + Math.Max(0, 6m - yearsPassed) / 5m * 0.33m;
            else
                return 1m;
        }

        public decimal StdDevRangeMultiplier
        {
            get { return stdDevRangeMultiplier; }
            set
            {
                stdDevRangeMultiplier = Math.Abs(value);
                
                unfavorableReturns = null;
                favorableReturns = null;
                unfavorableValues = null;
                favorableValues = null;
            }
        }
        private decimal stdDevRangeMultiplier = 1m;

        public decimal[] UnfavorableReturns
        {
            get
            {
                if (unfavorableReturns == null)
                {
                    if (CashFlow.Length > 0)
                    {
                        decimal[] diffs = StdDevsOfTheMean.Select(v => ExpectedReturn - StdDevRangeMultiplier * v)
                                                          .ToArray();
                        decimal[] totalReturns = diffs.Zip(YearsPassed)
                                                      .Select(p => FinancialMath.Pow(1 + p.Item1, p.Item2))
                                                      .ToArray();

                        // the first value is a dummy
                        unfavorableReturns = Enumerable.Repeat(ExpectedReturn - StdDevRangeMultiplier * StandardDeviation, 1)
                                     .Concat(totalReturns.Zip(totalReturns.Skip(1))
                                                         .Select(p => p.Item2 / p.Item1 - 1))
                                     .ToArray();
                    }
                    else
                        unfavorableReturns = new decimal[] { };
                }

                return unfavorableReturns;
            }
        }
        private decimal[] unfavorableReturns;

        public decimal[] FavorableReturns
        {
            get
            {
                if (favorableReturns == null)
                    favorableReturns = UnfavorableReturns.Select(u => 2 * ExpectedReturn - u)
                                                         .ToArray();

                return favorableReturns;
            }
        }
        private decimal[] favorableReturns;

        public decimal[] UnfavorableValues
        {
            get
            {
                if (unfavorableValues == null)
                    unfavorableValues = calculateValues(CashFlow, UnfavorableReturns);

                return unfavorableValues;
            }
        }
        private decimal[] unfavorableValues;

        public decimal[] FavorableValues
        {
            get
            {
                if (favorableValues == null)
                    favorableValues = calculateValues(CashFlow, FavorableReturns);

                return favorableValues;
            }
        }
        private decimal[] favorableValues;

        public decimal[] NetPresentValues
        {
            get
            {
                if (netPresentValues == null)
                    netPresentValues = CashFlow.Zip(YearsPassed, (v, y) => v / FinancialMath.Pow(1 + ExpectedReturn, y))
                                               .ToArray();

                return netPresentValues;
            }
        }
        private decimal[] netPresentValues;

        #endregion


        #region Chance of Meeting Target

        public decimal ChanceOfMeetingTarget
        {
            get
            {
                if (chanceOfMeetingTarget == null)
                {
                    if (CashFlow.Where(v => v > 0m).Count() == 0)
                        chanceOfMeetingTarget = CashFlow.Where(v => v < 0m).Count() == 0
                                                    ? 1m
                                                    : 0m;
                    else
                        chanceOfMeetingTarget = 1m - FinancialMath.CumulativeNormalDistribution(InternalRateOfReturn,
                                                                                                ExpectedReturn, StandardDeviationOfTheMean);
                    //{
                    //    decimal internalRateOfReturn;

                    //    try
                    //    {
                    //        internalRateOfReturn = InternalRateOfReturn;
                    //    }
                    //    catch (ArgumentException)
                    //    {
                    //        //internalRateOfReturn = -1m;
                    //        return 1m;
                    //    }

                    //    chanceOfMeetingTarget = 1m - FinancialMath.CumulativeNormalDistribution(internalRateOfReturn,
                    //                                                                            ExpectedReturn, StandardDeviationOfTheMean);
                    //}
                }

                return (decimal)chanceOfMeetingTarget;
            }
        }
        private decimal? chanceOfMeetingTarget;

        public decimal InternalRateOfReturn
        {
            get
            {
                if (internalRateOfReturn == null)
                    internalRateOfReturn = FinancialMath.InternalRateOfReturn(CashFlow);

                return (decimal)internalRateOfReturn;
            }
        }
        private decimal? internalRateOfReturn;

        public decimal StandardDeviationOfTheMean
        {
            get
            {
                if (standardDeviationOfTheMean == null)
                    standardDeviationOfTheMean = (AutoCorrelation ? 4m / 3m : 1m)
                                               * StandardDeviation / FinancialMath.Sqrt(EffectivePeriod);

                return (decimal)standardDeviationOfTheMean;
            }
        }
        private decimal? standardDeviationOfTheMean;

        public decimal EffectivePeriod
        {
            get
            {
                if (effectivePeriod == null)
                {
                    if (CashFlow.Count(v => v > 0m) == 0)
                        throw new ApplicationException("Cash flow must have at least one deposit.");

                    List<decimal> cashFlowList = CashFlow.ToList();

                    int firstDepositIndex = cashFlowList.FindIndex(v => v > 0m),
                        lastDepositIndex = cashFlowList.FindLastIndex(v => v > 0m),
                        firstWithdrawalIndex = cashFlowList.FindIndex(v => v < 0m),
                        lastWithdrawalIndex = cashFlowList.FindLastIndex(v => v < 0m);

                    if (firstWithdrawalIndex >= 0)
                    {
                        if (firstDepositIndex > firstWithdrawalIndex)
                            throw new ApplicationException("Cash flow must have at least one deposit before any withdrawals.");
                        else if (lastDepositIndex > firstWithdrawalIndex)
                            throw new ApplicationException("Cash flow withdrawals must be preceded by deposits.");
                    }

                    decimal lumpsumIncreasePeriod = firstWithdrawalIndex >= 0 ? firstWithdrawalIndex - firstDepositIndex :
                                                                                cashFlowList.Count;

                    decimal periodicIncreasePeriod = 0.75m * lumpsumIncreasePeriod
                                                   - 0.1m * (decimal)Math.Min(9, DepositsPerYear)
                                                   + 0.75m;

                    decimal lumpsumPeriodicRatio = NetPresentValues[firstDepositIndex] / NetPresentValues.Where(v => v > 0m).Sum();

                    decimal increasePeriod = lumpsumPeriodicRatio * lumpsumIncreasePeriod +
                                             (1m - lumpsumPeriodicRatio) * periodicIncreasePeriod;

                    decimal decreasePeriod = firstWithdrawalIndex >= 0 ? (lastWithdrawalIndex - firstWithdrawalIndex + 1) / 2m : 0m;

                    effectivePeriod = increasePeriod + decreasePeriod;
                }

                return (decimal)effectivePeriod;
            }
        }
        private decimal? effectivePeriod;

        #endregion
    }
}
