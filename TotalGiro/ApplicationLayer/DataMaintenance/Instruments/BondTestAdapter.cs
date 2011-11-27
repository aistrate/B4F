using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.CommRules;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments
{
    public static class BondTestAdapter
    {
        /// <summary>
        /// This method tries to find a commission rule based on the parameters entered
        /// </summary>
        /// <param name="saccount">Account number</param>
        /// <param name="sinstrument">Instrument</param>
        /// <param name="sordervalue">Order value</param>
        /// <param name="samount">Amount</param>
        /// <param name="sprice">Price</param>
        public static string CalculateAccruedInterest(int instrumentId,
            DateTime settlementDate, BaseOrderTypes orderType,
            decimal amountQuantity, decimal sizeQuantity, decimal priceQuantity,
            int exchangeId)
        {
            string result = "";

            try
            {

                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    InstrumentSize size = null;
                    IBond instrument = (IBond)InstrumentMapper.GetInstrument(session, instrumentId);
                    IExchange exchange = null;
                    if (instrument == null)
                        throw new ApplicationException("select a valid bond");

                    if (exchangeId != 0 && exchangeId != int.MinValue)
                        exchange = ExchangeMapper.GetExchange(session, exchangeId);

                    ICurrency defcurrency = instrument.CurrencyNominal;
                    Price price = null;
                    if (priceQuantity != 0M)
                        price = new Price(priceQuantity, defcurrency, instrument);
                    else if (instrument.CurrentPrice != null)
                        price = instrument.CurrentPrice.Price;

                    if (!(price != null && price.IsNotZero))
                        throw new ApplicationException("There is no price");

                    if (orderType == BaseOrderTypes.AmountBased)
                    {
                        Money amount = new Money(amountQuantity, defcurrency);
                        size = instrument.CalculateSizeBackwards(amount, price, settlementDate);
                    }
                    else
                        size = new InstrumentSize(sizeQuantity, instrument);

                    AccruedInterestDetails accInt = instrument.AccruedInterest(size, settlementDate, exchange);
                    Money calcAmount = size.CalculateAmount(price);

                    if (accInt.IsRelevant)
                        result = accInt.DisplayString + string.Format("<br>Size: {0}<br>Amount: {1}<br>Settlement Date: {2}",
                        size.Quantity, calcAmount.Quantity, settlementDate.ToString("dd-MM-yyyy"));
                    else
                        result = "Accrued interest is not relevant";
                }
            }
            catch (Exception ex)
            {
                result = "Error during accrued interest test: " + ex.Message;
            }
            return result;
        }

    }
}
