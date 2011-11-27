using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments.Classification;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments
{
    public class AssetManagerInstrumentSelectionAdapter
    {
        public static DataSet GetUnMappedInstruments(int assetManagerId, string isin, string instrumentName, SecCategories secCategoryId, int currencyNominalId)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable();

            if (!string.IsNullOrEmpty(isin))
                parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(isin, MatchModes.Anywhere));
            if (!string.IsNullOrEmpty(instrumentName))
                parameters.Add("instrumentName", Util.PrepareNamedParameterWithWildcard(instrumentName, MatchModes.Anywhere));
            if (secCategoryId > 0)
                parameters.Add("secCategoryId", secCategoryId);
            if (currencyNominalId > 0)
                parameters.Add("currencyNominalId", currencyNominalId);
            if (assetManagerId == 0)
            {
                IManagementCompany comp = LoginMapper.GetCurrentManagmentCompany(session);
                if (comp == null || comp.IsStichting)
                    throw new ApplicationException("Not good");
                else
                    assetManagerId = comp.Key;
            }
            parameters.Add("managementCompanyID", assetManagerId);
            List<ITradeableInstrument> instruments = session.GetTypedListByNamedQuery<ITradeableInstrument>(
                "B4F.TotalGiro.Instruments.Instrument.InstrumentsNotMappedByAssetManager",
                parameters);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                instruments,
                "Key, DisplayName, DisplayIsin, SecCategory.Name, HomeExchange.ExchangeName, CurrencyNominal.Name, InActiveDate");

            session.Close();
            return ds;
        }

        public static DataSet GetMappedInstruments(int assetManagerId, ActivityReturnFilter activityFilter)
        {
            DataSet ds = null;
            IAssetManager am = null;
            IDalSession session = NHSessionFactory.CreateSession();

            if (assetManagerId == 0)
                am = (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);
            else
                am = ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

            if (am != null && am.AssetManagerInstruments != null && am.AssetManagerInstruments.Count > 0)
            {
                IList<IAssetManagerInstrument> list = am.AssetManagerInstruments.ToList();
                if (activityFilter != ActivityReturnFilter.All)
                    list = list.Where(u => u.IsActive == (activityFilter == ActivityReturnFilter.Active)).ToList();
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    list.ToList(),
                    @"Key, Instrument.DisplayName, Instrument.DisplayIsin, AssetClass.Key, AssetClass.AssetName, 
                    RegionClass.Key, RegionClass.RegionName, InstrumentsCategories.Key, InstrumentsCategories.InstrumentsCategoryName, 
                    SectorClass.Key, SectorClass.SectorName, MaxWithdrawalAmountPercentage, IsActive");
            }

            session.Close();
            return ds;
        }

        public static void CreateInstrumentMapping(int assetManagerId, int[] instrumentIds)
        {
            if (instrumentIds != null && instrumentIds.Length > 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IAssetManager am = null;
                    if (assetManagerId == 0)
                        am = (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);
                    else
                        am = ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

                    foreach (int instrumentId in instrumentIds)
                    {
                        ITradeableInstrument instrument = InstrumentMapper.GetTradeableInstrument(session, instrumentId);
                        am.AssetManagerInstruments.AddInstrument(instrument);
                    }
                    session.Update(am);
                }
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
        }

        public static void EditInstrumentMapping(bool IsActive, int assetClassId, int regionClassId, int instrumentsCategoryId, int sectorClassId, decimal maxWithdrawalAmountPercentage, int original_Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            if (original_Key != 0)
            {
                IAssetManagerInstrument ai = session.GetTypedList<AssetManagerInstrument>(original_Key).FirstOrDefault();
                if (ai != null)
                {
                    ai.AssetClass = ClassificationMapper.GetAssetClass(session, assetClassId);
                    ai.RegionClass = ClassificationMapper.GetRegionClass(session, regionClassId);
                    ai.InstrumentsCategories = ClassificationMapper.GetInstrumentsCategory(session, instrumentsCategoryId);
                    ai.SectorClass = ClassificationMapper.GetSectorClass(session, sectorClassId);
                    ai.MaxWithdrawalAmountPercentage = Convert.ToInt16(maxWithdrawalAmountPercentage);
                    ai.IsActive = IsActive;
                    session.Update(ai);
                }
            }
            session.Close();
        }

        public static void DeleteInstrumentMapping(int original_Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            if (original_Key != 0)
            {
                IAssetManagerInstrument ai = session.GetTypedList<AssetManagerInstrument>(original_Key).FirstOrDefault();
                if (ai != null)
                    session.Delete(ai);
            }
            session.Close();
        }


        public static DataSet GetAssetClasses()
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                ClassificationMapper.GetAssetClasses(session),
                "Key, AssetName");

            session.Close();
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetRegionClasses()
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                ClassificationMapper.GetRegionClasses(session),
                "Key, RegionName");

            session.Close();
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetInstrumentsCategories()
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                ClassificationMapper.GetInstrumentsCategories(session),
                "Key, InstrumentsCategoryName");

            session.Close();
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetSectorClasses()
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                ClassificationMapper.GetSectorClasses(session),
                "Key, SectorName");

            session.Close();
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }
    }
}
