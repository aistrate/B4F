using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using System.Collections;
using System.Web;

namespace B4F.TotalGiro.ApplicationLayer.Orders.AssetManager
{
    public static class SingleOrderAdapter
    {
        public static void GetTotalCashAmount(int accountId, out string cashPositionString, out string moneyFundPositionString, out string openOrderCashString)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
            Money totalCashAmount = account.TotalPositionAmount(PositionAmountReturnValue.Cash);
            Money moneyFundAmount = account.TotalPositionAmount(PositionAmountReturnValue.CashFund);
            Money zeroAmount = new Money(0M, account.BaseCurrency);

            if (totalCashAmount != null)
                cashPositionString = totalCashAmount.ToString();
            else
                cashPositionString = zeroAmount.ToString();
            if (moneyFundAmount != null)
                moneyFundPositionString = moneyFundAmount.ToString();
            else
                moneyFundPositionString = zeroAmount.ToString();
            if (account.OpenOrderAmount() != null)
                openOrderCashString = account.OpenOrderAmount().ToString();
            else
                openOrderCashString = zeroAmount.ToString();
            
            session.Close();
        }

        public static DataSet GetCurrencies(int accountId, int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeCustomer account = null;
                IInstrument instrument = null;

                if (accountId > 0)
                    account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
                if (instrumentId > 0)
                    instrument = InstrumentMapper.GetInstrument(session, instrumentId);

                DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    getCurrencies(account, instrument), "Key, Symbol");

                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }

        private static List<ICurrency> getCurrencies(IAccountTypeCustomer account, IInstrument instrument)
        {
            List<ICurrency> currencies = new List<ICurrency>();
            ICurrency baseCurrency = null, instrumentCurrency = null;

            if (account != null)
            {
                baseCurrency = account.AccountOwner.StichtingDetails.BaseCurrency;
                currencies.Add(baseCurrency);
            }

            if (instrument != null)
            {
                instrumentCurrency = ((ITradeableInstrument)instrument).CurrencyNominal;
                if (!instrumentCurrency.IsActive && instrumentCurrency.ParentInstrument != null)
                    instrumentCurrency = instrumentCurrency.ParentInstrument.ToCurrency;

                if (baseCurrency == null || baseCurrency.Key != instrumentCurrency.Key)
                    currencies.Add(instrumentCurrency);
            }

            return currencies;
        }

        public static OrderValidationResult PlaceOrder(
                                                int accountId, 
                                                int instrumentId, 
                                                Side side,
                                                bool isAmountBased, 
                                                decimal size,
                                                decimal amount,
                                                int currencyId,
                                                bool isValueInclComm,
                                                bool noCharges,
                                                bool ignoreWarnings,
                                                bool bypassValidation)
        {
            OrderValidationResult validationResult = new OrderValidationResult(OrderValidationSubType.Invalid_NotValidated, "This order has not been validated.");
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
                IInstrument instrument = InstrumentMapper.GetInstrument(session, instrumentId);
                FeeFactory fee = FeeFactory.GetInstance(session);
                Order order;
                decimal sign = (side == Side.Sell ? -1 : 1);

                if (!isAmountBased)
                {
                    InstrumentSize value = new InstrumentSize(sign * size, instrument);
                    order = new OrderSizeBased(account, value, false, fee, noCharges);
                }
                else
                {
                    ICurrency currency = InstrumentMapper.GetCurrency(session, currencyId);
                    if (currency == null)
                        currency = ((ITradeableInstrument)instrument).CurrencyNominal;
                    if (!getCurrencies(account, instrument).Contains(currency))
                        throw new ArgumentException(
                            string.Format("Invalid currency ({0}). Currency should be either the base currency or the instrument currency.",
                                          currency.Symbol));
                    Money value = new Money(sign * amount, currency);
                    order = new OrderAmountBased(account, value, instrument, isValueInclComm, fee, noCharges);
                }

                // Do Validation
                if (bypassValidation)
                    validationResult = new OrderValidationResult(OrderValidationSubType.Success, "");
                else
                    validationResult = order.Validate();

                if (validationResult.MainType == OrderValidationType.Success ||
                   (validationResult.MainType == OrderValidationType.Warning && ignoreWarnings))
                {
                    OrderMapper.Insert(session, order);
                    validationResult = new OrderValidationResult(OrderValidationSubType.Success, "");
                }
            }
            finally
            {
                session.Close();
            }

            return validationResult;
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, 
                                        string accountName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = AccountMapper.GetCustomerAccounts(session, assetManagerId, 0, 0, 0, modelPortfolioId,
                    accountNumber, accountName, false, true, true, false, 0, true, false)
                    .Select(c => new
                    {
                        c.Key,
                        c.Number,
                        c.DisplayNumberWithName
                    })
                    .ToDataSet();

                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }
    }
}
