using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Globalization;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Communicator.ExternalInterfaces;

namespace B4F.TotalGiro.Communicator.PearelLeven
{
    /// <summary>
    /// Helper class to expose functions
    /// </summary>
    public static class PearelLevenMapper
    {
        static private string indicISIN = "1";
        static private string maxdev = "0";
        static private string showdate = "";
        static private string price = "";
        static private string pricetype = "dag";

        static private string xmlUniServKoers = @"
                      <koerstransactie>
                        <transactie_id>{0}</transactie_id>
                        <indicatie_isincodes>{1}</indicatie_isincodes>
                        <fonds>
                          <fondsnummer>{2}</fondsnummer>
                          <maximumafwijking>{3}</maximumafwijking>
                          <koersen>
                            <koers>
                              <koersdatum>{4}</koersdatum>
                              <koersbedrag>{5}</koersbedrag>
                              <soortkoers>{6}</soortkoers>
                            </koers>
                          </koersen>
                        </fonds>
                      </koerstransactie>
                ";


        /// <summary>
        /// Creates an XML file containing the current last price of the
        /// funds Paerel Leven is interested in. 
        /// </summary>
        /// <returns>XML string containing prices</returns>
        public static string Export(DateTime priceDate, DateTime showDate, int externalInterfaceKey)
        {

            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable(1);
            parameters.Add("externalInterfaceKey", externalInterfaceKey);

            string xmlkoersinfo = "<root>";
            string transactionid = "prl" + showDate.ToString("yyyyMMdd");
            string hql = @"from InstrumentSymbol S 
                where S.ExternalInterface.Key = :externalInterfaceKey 
                order by S.ExternalSymbol";
            IList instrumentsToExport = session.GetListByHQL(hql, parameters);

            if (instrumentsToExport != null && instrumentsToExport.Count > 0)
            {
                foreach (IInstrumentSymbol instSymbol in instrumentsToExport)
                {
                    xmlkoersinfo += FundPriceXML(session, transactionid, instSymbol, priceDate, showDate);
                }
            }
            xmlkoersinfo += "</root>";

            session.Close();

            return xmlkoersinfo;
        }

        /// <summary>
        /// Latest fund price information in XML form
        /// </summary>
        /// <param name="session">NHibernate session</param>
        /// <param name="transactionid">suffix for the unique number in this export</param>
        /// <param name="instSymbol">The symbol for the instrument</param>
        /// <returns>XML string containing price information</returns>
        static private string FundPriceXML(IDalSession session, string transactionid, IInstrumentSymbol instSymbol, DateTime priceDate, DateTime showDate)
        {
            string result = "";

            ITradeableInstrument instrument = (ITradeableInstrument)instSymbol.Instrument;
            if (instSymbol != null && instrument != null)
            {
                IList<IHistoricalPrice> prices = HistoricalPriceMapper.GetHistoricalPrices(session, instrument, priceDate);
                if (prices != null && prices.Count > 0)
                {
                    IPriceDetail priceDetail = (IPriceDetail)prices[0];
                    if (priceDetail != null)
                    {
                        CultureInfo cultinfo = new CultureInfo("en-US");
                        price = priceDetail.Price.Quantity.ToString(cultinfo);
                        showdate = showDate.ToString("dd-MM-yyyy");

                        result = String.Format(
                                    xmlUniServKoers,
                                    transactionid + "_" + instSymbol.ExternalSymbol, 
                                    indicISIN, instrument.Isin, maxdev,
                                    showdate, price, pricetype);
                    }
                }
            }
            return result;
        }
    }

}
