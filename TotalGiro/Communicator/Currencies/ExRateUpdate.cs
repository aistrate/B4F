using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Communicator.Currencies
{
    public class ExRateUpdate
    {
        public ExRateUpdate(Dictionary<DateTime, List<exRateDate>> TheDates)
        {
            this.theDates = TheDates;
        }

        public bool UpdateExRates(IDalSession DataSession)
        {
            theCurrencies = InstrumentMapper.GetCurrencies(DataSession);
            ICurrency loadedCurrency = null;
            List<exRateDate> innerRates = null;
            bool isDirty = false;
            foreach (KeyValuePair<DateTime, List<exRateDate>> de in theDates.OrderBy(x => x.Key))
            {
                innerRates = de.Value;
                foreach (exRateDate erd in innerRates)
                {
                    if (isCurrencyLoaded(erd.rateCurrency, ref loadedCurrency))
                    {
                        HistoricalExRate newRate = new HistoricalExRate(loadedCurrency, erd.rate , erd.rateDate, 0m, 0m, 1m);
                        if (!(loadedCurrency.HistoricalExRates.ContainsExRate(newRate)))
                        {
                            loadedCurrency.HistoricalExRates.AddExRate(newRate);
                            isDirty = true;
                        }
                        else if (loadedCurrency.HistoricalExRates.GetItemByDate(newRate.RateDate).Rate != newRate.Rate)
                        {
                            loadedCurrency.HistoricalExRates.GetItemByDate(newRate.RateDate).Rate = newRate.Rate;
                            isDirty = true;
                        }
                    }
                }
            }
            if (isDirty)
            {
                foreach (ICurrency currency in theCurrencies)
                {
                    InstrumentMapper.Update(DataSession, currency);
                    // Hashtable parameters = new Hashtable();
                    // parameters.Add("InstrumentID", currency.Key);
                    // DataSession.ExecuteStoredProcedure("EXEC dbo.TG_FillHistExRatesWeekendsHolidays @InstrumentID = :InstrumentID", parameters);
                }
            }
            return true;
        }


        private bool isCurrencyLoaded(string Symbol, ref ICurrency LoadedCurrency)
        {
            foreach (ICurrency c in theCurrencies)
            {
                if (c.Symbol == Symbol)
                {
                    LoadedCurrency = c;
                    return true;
                }
            }
            return false;
        }

        #region Private Variables

        private Dictionary<DateTime, List<exRateDate>> theDates;
        IList<ICurrency> theCurrencies;

        #endregion

    }
}
