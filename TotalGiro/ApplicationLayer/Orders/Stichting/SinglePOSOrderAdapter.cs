using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees;
using System.Collections;
using System.Web;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class SinglePOSOrderAdapter
    {
        //public static void GetTotalCashAmount(int accountId, out string cashPositionString, out string openOrderCashString)
        //{
        //    IDalSession session = NHSessionFactory.CreateSession();
        //    IAccountTypeInternal bankAccount = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);
        //    Money totalCashAmount = bankAccount.TotalPositionAmount(PositionAmountReturnValue.BothCash);
            
        //    cashPositionString = totalCashAmount.ToString();
        //    openOrderCashString = bankAccount.OpenOrderAmount().ToString();
            
        //    session.Close();
        //}

         

        public static DataSet GetCurrencies(int accountId, int instrumentId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ArrayList accounts = new ArrayList();
            ICurrency baseCurrency = null, instrumentCurrency = null;

            if (accountId > 0)
            {
                IAccountTypeCustomer bankAccount = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
                baseCurrency = bankAccount.AccountOwner.StichtingDetails.BaseCurrency;
                
                accounts.Add(bankAccount.AccountOwner.StichtingDetails.BaseCurrency);
            }

            if (instrumentId > 0)
            {
                IInstrument instrument = InstrumentMapper.GetInstrument(session, instrumentId);
                instrumentCurrency = ((ITradeableInstrument)instrument).CurrencyNominal;
                
                if (baseCurrency == null || baseCurrency.Key != instrumentCurrency.Key)
                    accounts.Add(((ITradeableInstrument)instrument).CurrencyNominal);
            }

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(accounts, "Key, Symbol");

            session.Close();

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
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
                                                bool ignoreWarnings,
                                                bool bypassValidation)
        {
            OrderValidationResult validationResult = new OrderValidationResult(OrderValidationSubType.Invalid_NotValidated, "This order has not been validated.");
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);
                IInstrument instrument = InstrumentMapper.GetInstrument(session, instrumentId);
                //FeeFactory fee = FeeFactory.GetInstance(session);
                Order order;
                decimal sign = (side == Side.Sell ? -1 : 1);

                if (!isAmountBased)
                {
                    InstrumentSize value = new InstrumentSize(sign * size, instrument);
                    order = new OrderSizeBased(account, value, false, null, true);
                }
                else
                {
                    ICurrency currency = InstrumentMapper.GetCurrency(session, currencyId);
                    if (currency == null)
                        currency = ((ITradeableInstrument)instrument).CurrencyNominal;
                    Money value = new Money(sign * amount, currency);
                    order = new OrderAmountBased(account, value, instrument, isValueInclComm, null, true);
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

        public static DataSet GetPosAccount()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList listOfCrumbles = new ArrayList();

            IEffectenGiro stichting = B4F.TotalGiro.Stichting.ManagementCompanyMapper.GetEffectenGiroCompany(session);

            listOfCrumbles.Add( stichting.CrumbleAccount);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(listOfCrumbles, "Key, Number, DisplayNumberWithName");
            return ds;
        }

        public static bool AggregatePOSOrders(out string errorMessage)
        {
            string message;
            errorMessage = "";

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IList<IOrder> unPOSAggregatedUnApprovedOrders = OrderMapper.GetPOSUnaggregatedOrders(session);

                if (unPOSAggregatedUnApprovedOrders != null && unPOSAggregatedUnApprovedOrders.Count > 0)
                {
                    foreach (IOrder order in unPOSAggregatedUnApprovedOrders) order.Approve();

                    IList<IOrder> aggregateOrders = null;

                    IList<IOrder> orders = Order.AggregateOrders(aggregateOrders, unPOSAggregatedUnApprovedOrders, out message);

                    foreach (IOrder order in orders) order.Approve();

                    if (orders != null)
                        OrderMapper.SaveAggregatedOrders(session, orders);
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            finally
            {
                session.Close();
            }
        }
    }
}
