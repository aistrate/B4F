using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class AccountsByInstrumentAdapter
    {
        public static DataSet GetInstruments(string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId)
        {
            if (secCategoryId != SecCategories.StockDividend)
                return InstrumentFinderAdapter.GetTradeableInstrumentsDDL(isin, instrumentName, secCategoryId, exchangeId, currencyNominalId);
            else
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    Hashtable parameters = new Hashtable();

                    if (!string.IsNullOrEmpty(isin))
                        parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(isin, MatchModes.Anywhere));
                    if (!string.IsNullOrEmpty(instrumentName))
                        parameters.Add("instrumentName", Util.PrepareNamedParameterWithWildcard(instrumentName, MatchModes.Anywhere));
                    if (exchangeId > 0)
                        parameters.Add("exchangeId", exchangeId);
                    if (currencyNominalId > 0)
                        parameters.Add("currencyNominalId", currencyNominalId);
                    IList<IInstrumentCorporateAction> list = session.GetTypedListByNamedQuery<IInstrumentCorporateAction>(
                        "B4F.TotalGiro.Instruments.Instrument.GetCorporateActions",
                        parameters);

                    DataSet ds = list
                        .Select(c => new
                        {
                            c.Key,
                            c.Isin,
                            c.DisplayIsinWithName
                        })
                        .ToDataSet();
                    Utility.AddEmptyFirstRow(ds.Tables[0]);
                    return ds;
                }
            }
        }

        
        public static DataSet GetPositions(int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IInstrument instrument = InstrumentMapper.GetInstrument(session, instrumentId);
                return FundPositionMapper.GetPositions(session, instrument, PositionsView.NotZero)
                    .Select(c => new
                    {
                        c.Key,
                        AccountID = c.Account.Key,
                        Account_Number = c.Account.Number,
                        Account_ShortName = c.Account.ShortName,
                        Size_Quantity = c.Size.Quantity, 
                        c.CurrentBaseValue
                    })
                    .ToDataSet();
            }
        }

        public static void GetInstrumentDetails(int instrumentId, out decimal totalSize, out Money totalValue,
                                                out Price price, out decimal exchangeRate)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ICurrency baseCurrency = LoginMapper.GetCurrentManagmentCompany(session).BaseCurrency;

            IInstrumentsWithPrices instrument = (IInstrumentsWithPrices)InstrumentMapper.GetInstrument(session, instrumentId);
            if (instrument != null)
            {
                IList positions = FundPositionMapper.GetPositions(session, instrument, PositionsView.NotZero);

                totalSize = 0m;
                decimal totalValueQuantity = 0m;
                foreach (IFundPosition position in positions)
                {
                    totalSize += position.Size.Quantity;
                    if (position.CurrentBaseValue != null)
                        totalValueQuantity += position.CurrentBaseValue.Quantity;
                }
                totalValue = new Money(totalValueQuantity, baseCurrency);
                
                price = (instrument.CurrentPrice != null ? instrument.CurrentPrice.Price : null);
                if (instrument.CurrencyNominal.ExchangeRate != null)
                    exchangeRate = instrument.CurrencyNominal.ExchangeRate.Rate;
                else
                {
                    if (instrument.CurrencyNominal.IsBase)
                        exchangeRate = 1M;
                    else
                        exchangeRate = 0M;
                }
            }
            else
            {
                totalSize = 0m;
                totalValue = new Money(0m, baseCurrency);
                price = null;
                exchangeRate = 1m;
            }

            if (price != null)
            {
                // this forces the load of property Amount (which otherwise would be lazy); 
                // the property will be needed later, after the session is closed
                Money amount = price.Amount;
            }

            session.Close();
        }
    }
}
