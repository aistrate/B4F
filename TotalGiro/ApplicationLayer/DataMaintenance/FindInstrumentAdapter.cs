using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class FindInstrumentAdapter
    {
        public static DataSet GetDynamicProp()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList list = InstrumentMapper.GetFilteredTradeableInstruments(session, "ABN");

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
               InstrumentMapper.GetFilteredTradeableInstruments(session, "ABN"), "Key, Isin, CompanyName, Name, CurrencyNominal.BaseCurrency.Name");
            session.Close();
            return ds;
        }
    }
}
