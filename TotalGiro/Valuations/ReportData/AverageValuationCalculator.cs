using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Valuations
{
    public class AverageValuationCalculator
    {
        public static IList GetAverageValuations(IList valuations, Regularities regularity)
        {
            SortedList averageValuations = null;
            int period;
            int year = 0;
            foreach (IValuation valuation in valuations)
            {
                if (regularity != Regularities.Annual)
                    checkYear(valuation.Date, ref year);
                
                switch (regularity)
                {
                    case Regularities.Annual:
                        period = valuation.Date.Year;
                        break;
                    case Regularities.BiAnnual:
                        period = valuation.Date.Month <= 6 ? 1 : 2;
                        break;
                    case Regularities.Quarterly:
                        period = Util.GetQuarter(valuation.Date);
                        break;
                    case Regularities.Monthly:
                        period = valuation.Date.Month;
                        break;
                    case Regularities.Weekly:
                        period = Util.GetWeekNumber(valuation.Date);
                        break;
                    default:
                        period = 0;
                        break;
                }

                if (period != 0)
                {
                    string key = string.Format("{0}_{1}", valuation.Instrument.Key.ToString(), period.ToString());
                    if (averageValuations == null)
                        averageValuations = new SortedList();
                    if (!averageValuations.ContainsKey(key))
                        averageValuations.Add(key, new AverageValuation(valuation, period));
                    else
                        ((AverageValuation)averageValuations[key]).AddValue(valuation);
                }
            }
            if (averageValuations != null && averageValuations.Count > 0)
            {
                // first return zero items
                removeZeroItems(ref averageValuations);
                
                IAverageValuation[] list = new IAverageValuation[averageValuations.Count];
                averageValuations.Values.CopyTo(list, 0);
                return list;
            }
            else
                return null;
        }

        private static void checkYear(DateTime date, ref int year)
        {
            if (year == 0)
                year = date.Year;
            else
            {
                if (date.Year != year)
                    throw new ApplicationException("Period is not in the same year");
            }
        }

        private static void removeZeroItems(ref SortedList averageValuations)
        {
            Dictionary<IInstrument, bool> instCol = new Dictionary<IInstrument, bool>();

            foreach (AverageValuation val in averageValuations.Values)
            {
                if (!instCol.ContainsKey(val.Instrument))
                    instCol.Add(val.Instrument, val.AvgMarketValue.IsNotZero);
                else if (!instCol[val.Instrument] && val.AvgMarketValue.IsNotZero)
                    instCol[val.Instrument] = true;
            }

            foreach (IInstrument key in instCol.Keys)
            {
                if (!instCol[key])
                {
                    for (int i = averageValuations.Count; i > 0; i--)
                    {
                        AverageValuation val = (AverageValuation)averageValuations.GetByIndex(i);
                        if (val.Instrument.Equals(key))
                            averageValuations.RemoveAt(i);
                    }
                }
            }
        }
    }

    public class AverageValuation : IAverageValuation
    {
        public AverageValuation(IValuation valuation, int period)
        {
            this.instrument = valuation.Instrument;
            this.avgMarketValue = valuation.MarketValue;
            this.avgBaseMarketValue = valuation.BaseMarketValue;
            this.period = period;
            weight = 1;
        }

        public void AddValue(IValuation valuation)
        {
            Money newValue = ((AvgMarketValue * weight) + (valuation.MarketValue)) / (weight + 1);
            Money newBaseValue = ((AvgBaseMarketValue * weight) + (valuation.BaseMarketValue)) / (weight + 1);
            weight++;
            AvgMarketValue = newValue;
            AvgBaseMarketValue = newBaseValue;
        }

        public IInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        public Money AvgMarketValue
        {
            get { return avgMarketValue; }
            internal set { avgMarketValue = value; }
        }

        public Money AvgBaseMarketValue
        {
            get { return avgBaseMarketValue; }
            internal set { avgBaseMarketValue = value; }
        }

        public int Period
        {
            get { return period; }
            internal set { period = value; }
        }

        public decimal ManagementFeePercentage
        {
            get { return ((ITradeableInstrument)instrument).ManagementFeePercentage; }
        }

        public override string ToString()
        {
            if (Instrument != null && AvgMarketValue != null)
                return string.Format("Period {0} {1} {2}", Period.ToString(), Instrument.Name, AvgMarketValue.DisplayString);
            else
                return base.ToString();
        }

        #region Privates

        private IInstrument instrument;
        private Money avgMarketValue;
        private Money avgBaseMarketValue;
        private int period;
        private int weight;

        #endregion
    }
}
