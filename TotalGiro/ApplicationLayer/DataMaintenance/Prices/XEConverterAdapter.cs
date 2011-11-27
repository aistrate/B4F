using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using NHibernate.Linq;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Prices
{
    public static class XEConverterAdapter
    {
        public static DataSet GetCurrencies()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetCurrencies(session, false)
                    .Select(c => new
                    {
                        c.Key,
                        c.Symbol
                    })
                    .ToDataSet();
            }
        }

        public static int GetBaseCurrencyId()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetBaseCurrency(session).Key;
            }
        }

        public static DateTime GetExRateDate(int currencyId, DateTime date)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICurrency currency = InstrumentMapper.GetCurrency(session, currencyId);
                if (Util.IsNotNullDate(date))
                {
                    IExRate rate = HistoricalExRateMapper.GetHistoricalExRate(session, currency, date);
                    return rate != null ? rate.RateDate : DateTime.MinValue;
                }
                else
                    return currency.ExchangeRate != null ? currency.ExchangeRate.RateDate : DateTime.MinValue;
            }
        }

        public static string ConvertAmount(decimal quantity, int curFromId, int curToId, DateTime date)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICurrency curFrom = InstrumentMapper.GetCurrency(session, curFromId);
                ICurrency curTo = InstrumentMapper.GetCurrency(session, curToId);
                Money amount = new Money(quantity, curFrom);
                
                Money convAmount = null;
                if (Util.IsNotNullDate(date))
                    convAmount = amount.Convert(curTo, Side.Buy, date, session);
                else
                    convAmount = amount.Convert(curTo);

                return convAmount.DisplayString;
            }
        }

        public static Tuple<string, DateTime> ConvertToBaseAmount(decimal quantity, int curFromId, DateTime date, bool isCurrent)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICurrency curFrom = InstrumentMapper.GetCurrency(session, curFromId);
                
                Money convAmount = null;
                DateTime rateDate = DateTime.MinValue;
                if (isCurrent || Util.IsNullDate(date))
                {
                    Money amount = new Money(quantity, curFrom);
                    if (curFrom.ExchangeRate != null)
                        rateDate = curFrom.ExchangeRate.RateDate;
                    convAmount = amount.CurrentBaseAmount;
                }
                else
                {
                    IExRate rate = HistoricalExRateMapper.GetNearestHistoricalExRate(session, curFrom, date);
                    if (rate == null)
                        throw new ApplicationException(string.Format("No exchange rate found on {0} for {1}", date.ToString("dd-MM-yyyy"), curFrom.Symbol));
                    Money amount = new Money(quantity, curFrom, rate.Rate);
                    rateDate = rate.RateDate;
                    convAmount = amount.BaseAmount;
                }

                return new Tuple<string,DateTime>(convAmount.DisplayString, rateDate);
            }
        }
    }
}
