using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using System.Collections;
using System.Web;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public static class EditMutualFund
    {
        public static DataSet GetExchanges(int instrumentID)
        {
            DataSet ds;
            IDalSession session = (IDalSession)HttpContext.Current.Session["MutualFundIDalSession"];
            ITradeableInstrument tradInstr = InstrumentMapper.GetTradeableInstrument(session, instrumentID);
            IInstrumentExchangeCollection collExchanges = tradInstr.InstrumentExchanges;
            ArrayList alExchanges = new ArrayList();
            foreach (IInstrumentExchange exchange in collExchanges)
            {
                alExchanges.Add(exchange.Exchange);
            }
            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                alExchanges, "Key, ExchangeName, DefaultCountry");

            return ds;
        }
    }
}
