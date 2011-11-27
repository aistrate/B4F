using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class InstrumentFinderAdapter
    {
        public static DataSet GetSecCategories()
        {
            return GetSecCategories(SecCategoryFilterOptions.All);
        }
        
        public static DataSet GetSecCategories(SecCategoryFilterOptions secCategoryFilter)
        {
            return GetSecCategories(secCategoryFilter, false);
        }

        public static DataSet GetSecCategories(SecCategoryFilterOptions secCategoryFilter, bool includeNotSupported)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = SecCategoryMapper.GetSecCategories(session, secCategoryFilter, includeNotSupported)
                    .Select(c => new
                    {
                        c.Key,
                        c.Description
                    })
                    .ToDataSet();

                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static DataSet GetExchanges()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = ExchangeMapper.GetExchanges(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.ExchangeName
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static DataSet GetCurrencies()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = InstrumentMapper.GetCurrenciesSorted(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayName
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }

        public static DataSet GetActivityFilterOptions()
        {
            return Util.GetDataSetFromEnum(typeof(ActivityReturnFilter));
        }

        public static DataSet GetTradeableInstruments(
            SecCategoryFilterOptions secCategoryFilter, string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId,
            ActivityReturnFilter activityFilter, string propertyList)
        {
            return GetTradeableInstruments(
                        secCategoryFilter, isin, instrumentName, secCategoryId,
                        exchangeId, currencyNominalId, activityFilter, true, null, propertyList);
        }

        
        public static DataSet GetTradeableInstruments(
            SecCategoryFilterOptions secCategoryFilter, string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId, 
            ActivityReturnFilter activityFilter, bool assetManagerMappedOnly, 
            string hqlWhere, string propertyList)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetTradeableInstruments(
                                session, secCategoryFilter, isin, instrumentName,
                                secCategoryId, exchangeId, currencyNominalId,
                                assetManagerMappedOnly, activityFilter, hqlWhere)
                    .ToDataSet(propertyList);
            }
        }

        public static DataSet GetTradeableInstrumentsDDL(string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId)
        {
            return GetTradeableInstrumentsDDL(isin, instrumentName, secCategoryId, exchangeId, currencyNominalId, ActivityReturnFilter.All);
        }

        public static DataSet GetTradeableInstrumentsDDL(string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId, ActivityReturnFilter activityFilter)
        {
            DataSet ds = InstrumentFinderAdapter.GetTradeableInstruments(
                SecCategoryFilterOptions.All, isin, instrumentName,
                secCategoryId, exchangeId, currencyNominalId,
                activityFilter, true, null, "Key, Isin, DisplayIsinWithName");

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetTradeableInstrumentsDDL(
            SecCategoryFilterOptions secCategoryFilter, string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId,
            ActivityReturnFilter activityFilter)
        {
            DataSet ds = InstrumentFinderAdapter.GetTradeableInstruments(
                secCategoryFilter, isin, instrumentName,
                secCategoryId, exchangeId, currencyNominalId, 
                activityFilter, true, null, "Key, Isin, DisplayIsinWithName");

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }
    }
}
