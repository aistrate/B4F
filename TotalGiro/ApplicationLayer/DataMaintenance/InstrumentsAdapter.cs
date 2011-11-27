using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class InstrumentsAdapter
    {
        public static DataSet GetTradeableInstrument(int id)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                InstrumentMapper.GetTradeableInstruments(session, id),
                "Key, Isin, CompanyName, Name, CurrencyNominal.BaseCurrency.Name");
            session.Close();
            return ds;
        }
    }
}
