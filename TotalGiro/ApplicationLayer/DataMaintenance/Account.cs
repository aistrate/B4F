using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM;
using System.Web;
using B4F.TotalGiro.StaticData;
using System.Data;
using System.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class Account
    {
        public static DataSet GetCurrenciesSorted()
        {
            IDalSession session = (IDalSession)HttpContext.Current.Session["AccountIDalSession"];

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                InstrumentMapper.GetCurrenciesSorted(session), "Key, Symbol");

            Utility.AddEmptyFirstRow(ds.Tables[0], "Key", "Symbol");
            return ds;
        }
    }
}
