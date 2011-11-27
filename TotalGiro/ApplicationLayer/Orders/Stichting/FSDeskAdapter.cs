using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Communicator.FSInterface;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class FSDeskAdapter
    {
        private const string FUND_SETTLE = "FDS";
        private const string FUND_SETTLE_NAME = "FundSettle";

        public static DataSet GetRoutedOrders(string isin, string instrumentName, SecCategories secCategoryId, int currencyNominalId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {

                IExchange fundsettle = getExchange(session);
                if (fundsettle == null)
                    throw new ApplicationException("Could not find exchange FundSettle.");

                IRoute route = RouteMapper.GetRouteByExchange(session, fundsettle);
                if (route == null)
                    throw new ApplicationException("Could not find exchange FundSettle.");

                //"Key, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, PlacedValue, PlacedValue.DisplayString, OpenValue,
                //"OpenValue.DisplayString, DisplayIsSizeBased, Status, DisplayStatus, Route, CreationDate, OrderID, IsFsSendable");
                return OrderMapper.GetOrders<IStgOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.Stichting, ApprovalState.Approved,
                    SecurityInfoOptions.TradingAcctOnly, ParentalState.Null, new OrderStatusFilter(OrderStatusFilterOptions.IncludeClosedStatiToday),
                    isin, instrumentName, secCategoryId, currencyNominalId, route.Key)
                    .Select(c => new
                    {
                        c.Key,
                        TradedInstrument_DisplayName = c.RequestedInstrument.DisplayName,
                        c.DisplayTradedInstrumentIsin,
                        c.Side,
                        c.PlacedValue,
                        PlacedValue_DisplayString = c.PlacedValue.DisplayString,
                        c.OpenValue,
                        OpenValue_DisplayString = c.OpenValue.DisplayString,
                        c.DisplayIsSizeBased,
                        c.Status,
                        c.DisplayStatus,
                        c.Route,
                        c.CreationDate,
                        c.OrderID,
                        IsFsSendable =
                            c.IsSecurity ? ((ISecurityOrder)c).IsFsSendable : false
                    })
                    .ToDataSet();
            }
        }

        private static IExchange getExchange(IDalSession session)
        {
            return ExchangeMapper.GetExchangeByName(session, FUND_SETTLE_NAME);
        }

        public static ArrayList GetOrderEditData(object orderId)
        {
            ArrayList allOrderFills = new ArrayList();

            if (orderId != null)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IOrder order = OrderMapper.GetOrder(session, (int)orderId);
                allOrderFills.Add(new OrderEditView(order.OrderID,
                                                    order.PlacedValue.NumberOfDecimals,
                                                    ((SecurityOrder)order).Route.Key));
                session.Close();
            }

            return allOrderFills;
        }

        public static void SaveOrderInfo(int OrderID, int iRoute)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IOrder orderToPlace = OrderMapper.GetOrder(session, OrderID);
            IRoute route = RouteMapper.GetRoute(session, iRoute);
            ((IStgOrder)orderToPlace).ChangeRoute(route);

            OrderMapper.Update(session, orderToPlace);
            session.Close();
        }

        public static DataSet GetRoutes()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IList routes = session.GetListByHQL("from Route R where R.ResendSecurityOrdersAllowed = 1", null);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(routes, "Key, Name");

            session.Close();
            return ds;
        }

        public static DateTime GetSettlementDate(DateTime date, int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, orderId);
                ITradeableInstrument instrument = order.TradedInstrument;
                
                return instrument.GetSettlementDate(date, getExchange(session));
            }
            finally
            {
                session.Close();
            }
        }

        public static void UnApproveOrders(int[] orderIds)
        {
            bool success = false;
            IDalSession session = NHSessionFactory.CreateSession();

            IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);
            foreach (IOrder order in orders)
            {
                success = order.UnApprove();

            }
            if (success)
                OrderMapper.Update(session, orders);
            session.Close();
        }

        public static IList GetOrderFills(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return ManualDeskAdapter.GetOrderFills(orderId, getExchange(session));
        }

        public static void UpdateOrderFill(OrderFillView orderFillView)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                ICurrency orderCurrency;
                ITransaction transaction;
                IExchange exchange = null;
                Money serviceCharge = null;
                decimal serviceChargePercentage = 0m;

                ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, orderFillView.OrderId);

                IAccount counterpartyAccount = AccountMapper.GetAccountByNumber(session, FUND_SETTLE);
                if (counterpartyAccount == null)
                    throw new ApplicationException(string.Format("The counterparty account {0} can not be found in the database.", FUND_SETTLE));

                if (orderFillView.ExchangeId != 0 && orderFillView.ExchangeId != int.MinValue)
                    exchange = ExchangeMapper.GetExchange(session, orderFillView.ExchangeId);
                if (exchange == null)
                    throw new ApplicationException("Select an exchange when filling the order");

                if (order.Value.Underlying.IsCash)
                    orderCurrency = (ICurrency)order.Value.Underlying;
                else
                    orderCurrency = ((ITradeableInstrument)order.Value.Underlying).CurrencyNominal;

                Price price = new Price(orderFillView.Price, orderCurrency, ((ISecurityOrder)(order)).TradedInstrument);

                InstrumentSize size = new InstrumentSize(orderFillView.Size, order.TradedInstrument);

                ICurrency txCurrency = null;
                if (order.TradedInstrument.IsCash)
                    txCurrency = (ICurrency)order.TradedInstrument;
                else
                    txCurrency = (ICurrency)order.TradedInstrument.CurrencyNominal;
                
                decimal exRate = order.TradedInstrument.CurrencyNominal.ExchangeRate.Rate;
                if (orderFillView.ServiceChargeAmount != 0)
                {
                    serviceCharge = new Money(orderFillView.ServiceChargeAmount, txCurrency);
                    serviceChargePercentage = orderFillView.ServiceChargePercentage;
                }

                //if (orderFillView.IsCompleteFill)
                //{
                Money amount = new Money(orderFillView.Amount, txCurrency);
                //transaction = order.Fill(size, price, amount, exRate, counterpartyAccount,
                //    orderFillView.TransactionDate, orderFillView.TransactionTime, exchange,
                //    orderFillView.IsCompleteFill, serviceCharge, serviceChargePercentage);
                //}
                //else
                //    transaction = order.Fill(size, price, exRate, counterpartyAccount, orderFillView.TransactionDate, serviceCharge, serviceChargePercentage);

                //if (Util.IsNotNullDate(orderFillView.SettlementDate))
                //    transaction.ContractualSettlementDate = orderFillView.SettlementDate;
                //transaction.Approve();

                //TransactionMapper.Insert(session, transaction);

                //if (transaction.Approved)
                //{
                //    IFeeFactory fees = FeeFactory.GetInstance(session);
                //    //order.Allocate(transaction, fees);

                //    OrderMapper.Update(session, order);
                //}
            }
            finally
            {
                session.Close();
            }
        }

        public static void CancelOrder(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IOrder orderToCancel = OrderMapper.GetOrder(session, orderId);
            orderToCancel.Cancel();
            OrderMapper.Update(session, orderToCancel);
            session.Close();
        }

        public static void ResetOrder(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IStgOrder orderToReset = (IStgOrder)OrderMapper.GetOrder(session, orderId);
            if (orderToReset.Reset())
                OrderMapper.Update(session, (IOrder)orderToReset);
            session.Close();
        }

        public static void SendOrders(int[] orderIds, out string errorMessage)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            errorMessage = "";

            try
            {
                FSExportFile exportfile = new FSExportFile();
                exportfile.FilePath = (string)(System.Configuration.ConfigurationManager.AppSettings.Get("FSExportFilePath"));
                //exportfile.FilePath = (string)(System.Configuration.ConfigurationSettings.AppSettings.Get("FSExportFilePath"));

                IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);
                foreach (Order order in orders)
                {
                    exportfile.ExportedOrders.Add(order);
                    ((IStgOrder)order).Send();
                    order.ExportFile = (IFSExportFile)exportfile;

                }

                session.BeginTransaction();
                FSExportFileMapper.Update(session, exportfile);

                if (exportfile.CreateExportFile())
                    session.CommitTransaction();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                session.Close();
            }
        }
    }
}
