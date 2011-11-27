using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class AssetManagerInstrumentsAdapter
    {
        public static DataSet GetInstruments()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList instruments = session.GetListByHQL(getHQL(false, session), null);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                InstrumentMapper.GetTradeableInstruments(session),
                "Key, Isin, DisplayName, DisplayCurrentPrice, DisplayCurrentPriceDate");
            session.Close();

            return ds;
        }

        private static string getHQL(bool instrumentsOwnedByAM, IDalSession session)
        {
            string hql = string.Format("from TradeableInstrument I where I.Key {0} in (" +
                "select AI.Key from AssetManager as A " +
                "left outer join A.bagOfInstruments as AI " +
                "where A.Key = {1})", (instrumentsOwnedByAM ? "" : "not"), LoginMapper.GetCurrentManagmentCompany(session).Key);

            return hql;
        }

    }
}
