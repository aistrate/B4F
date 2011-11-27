using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class TBMDataAdapter
    {
        public static DataSet GetMutualFunds()
        {
            DataSet ds = new DataSet();
            IDalSession session = NHSessionFactory.CreateSession();
            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                session.GetList(typeof(TradeableInstrument)), "Key, Isin");
            session.Close();
            return ds;
        }
    }
}
