using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.TGTransactions;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class ManualDeskAdapter
    {
        public static DataSet GetRoutedOrders(string isin, string instrumentName, SecCategories secCategoryId, int currencyNominalId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IRoute manDesk = RouteMapper.GetRouteByType(session, RouteTypes.ManualDesk);
                if (manDesk == null)
                    throw new ApplicationException("Could not find the manual desk route.");

                //"Key, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, PlacedValue, PlacedValue.DisplayString, OpenValue, 
                //OpenValue.DisplayString, PlacedValue.NumberOfDecimals, DisplayIsSizeBased, Status, DisplayStatus, Route, CreationDate, 
                //IsUnApproveable, OrderID");
                return OrderMapper.GetOrders<IStgOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.Stichting, ApprovalState.Approved,
                        SecurityInfoOptions.TradingAcctOnly, ParentalState.Null, new OrderStatusFilter(OrderStatusFilterOptions.IncludeClosedStatiToday),
                        isin, instrumentName, secCategoryId, currencyNominalId, manDesk.Key)
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
                        PlacedValue_NumberOfDecimals = c.PlacedValue.NumberOfDecimals,
                        c.DisplayIsSizeBased,
                        c.Status,
                        c.DisplayStatus,
                        c.Route,
                        c.CreationDate,
                        c.IsUnApproveable,
                        c.OrderID
                    })
                    .ToDataSet();
            }
        }

        public static IList GetOrderFills(int orderId)
        {
            return GetOrderFills(orderId, null);
        }

        public static IList GetOrderFills(int orderId, IExchange exchange)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ArrayList orderFills = new ArrayList();

            try
            {
                ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, (int)orderId);
                if (order != null)
                {
                    OrderFillView orderFillView = new OrderFillView(order.OrderID,
                                                                (order.IsSizeBased ? Math.Abs(order.OpenValue.Quantity) : 0m),
                                                                (order.IsSizeBased ? 0m : Math.Abs(order.OpenValue.Quantity)),
                                                                0m,1m,
                                                                order.IsSizeBased);

                    if (exchange != null)
                        orderFillView.ExchangeId = exchange.Key;
                    else if (order.TradedInstrument.HomeExchange != null)
                        orderFillView.ExchangeId = order.TradedInstrument.HomeExchange.Key;
                    else
                        orderFillView.ExchangeId = int.MinValue;

                    // ServiceCharge Percentage
                    if (order.TradedInstrument != null && order.TradedInstrument.InstrumentExchanges.Count > 0)
                    {
                        IInstrumentExchange instrumentExchange = order.TradedInstrument.InstrumentExchanges.GetDefault();
                        if (instrumentExchange != null)
                        {
                            // Default Counterparty
                            if (instrumentExchange.DefaultCounterParty != null)
                                orderFillView.CounterpartyAccountId = instrumentExchange.DefaultCounterParty.Key;

                            orderFillView.TickSize = instrumentExchange.TickSize;

                            // Service Charge
                            orderFillView.DoesSupportServiceCharge = instrumentExchange.DoesSupportServiceCharge;
                            if (instrumentExchange.DoesSupportServiceCharge)
                            {
                                orderFillView.ServiceChargeDisplayInfo = instrumentExchange.ServiceChargeDisplayInfo;
                                orderFillView.ServiceChargePercentage = instrumentExchange.GetServiceChargePercentageForOrder(order);
                                if (orderFillView.ServiceChargePercentage > 0 && orderFillView.IsAmountBased)
                                {
                                    orderFillView.ServiceChargeAmount =
                                        decimal.Round(orderFillView.Amount * (orderFillView.ServiceChargePercentage /
                                                        (1m + orderFillView.ServiceChargePercentage)), 2);
                                    orderFillView.Amount -= orderFillView.ServiceChargeAmount;
                                }
                            }
                        }
                    }

                    orderFillView.ShowExRate = !order.IsAmountBased && !order.TradedInstrument.CurrencyNominal.IsBase && !order.TradedInstrument.CurrencyNominal.IsObsoleteCurrency;
                    if (orderFillView.ShowExRate)
                        orderFillView.ExchangeRate = order.TradedInstrument.CurrencyNominal.ExchangeRate.Rate;

                    ICurrency currency = (order.IsSizeBased ? order.TradedInstrument.CurrencyNominal as ICurrency : order.Value.Underlying as ICurrency);
                    if (currency != null)
                    {
                        if (currency.IsObsoleteCurrency)
                            currency = currency.ParentInstrument as ICurrency;
                    }
                    orderFillView.AmountSymbol = (currency != null ? currency.AltSymbol : currency.AltSymbol);

                    orderFillView.SizeDecimals = order.TradedInstrument.DecimalPlaces;
                    orderFillView.AmountDecimals = (currency != null ? currency.DecimalPlaces : 2);

                    if (order.TradedInstrument.SecCategory.Key == SecCategories.Bond)
                    {
                        IBond bond = (IBond)order.TradedInstrument;
                        orderFillView.IsBondOrder = true;
                        if (bond.NominalValue != null)
                            orderFillView.TickSize = bond.NominalValue.Quantity;
                        if (bond.DoesPayInterest)
                        {
                            orderFillView.ShowAccruedInterest = true;
                            if (order.IsSizeBased)
                            {
                                if (order.AccruedInterest != null)
                                    orderFillView.AccruedInterestAmount = order.AccruedInterest.Quantity;
                            }
                        }
                    }

                    orderFills.Add(orderFillView);
                }
            }
            finally
            {
                session.Close();
            }

            return orderFills;
        }

        public static void UpdateOrderFill(OrderFillView orderFillView)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                ICurrency orderCurrency;
                IOrderExecution transaction;
                IAccount counterpartyAccount = null;
                IExchange exchange = null;
                Money serviceCharge = null;
                decimal serviceChargePercentage = 0m;

                //ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, orderFillView.OrderId);
                IStgOrder order = (IStgOrder)OrderMapper.GetOrder(session, orderFillView.OrderId);

                if (orderFillView.CounterpartyAccountId != 0)
                    counterpartyAccount = AccountMapper.GetAccount(session, orderFillView.CounterpartyAccountId);

                if (orderFillView.ExchangeId != 0 && orderFillView.ExchangeId != int.MinValue)
                    exchange = ExchangeMapper.GetExchange(session, orderFillView.ExchangeId);

                if (counterpartyAccount == null)
                    throw new ApplicationException("Select a counterparty account when filling the order");

                if (exchange == null)
                    throw new ApplicationException("Select an exchange when filling the order");

                if (((ISecurityOrder)order).Value.Underlying.IsCash)
                    orderCurrency = (ICurrency)(((ISecurityOrder)order).Value.Underlying);
                else
                    orderCurrency = ((ITradeableInstrument)(((ISecurityOrder)order).Value.Underlying)).CurrencyNominal;

                Price price = new Price(orderFillView.Price, ((ISecurityOrder)order).TradedInstrument.CurrencyNominal, ((ISecurityOrder)order).TradedInstrument);

                InstrumentSize size = new InstrumentSize(orderFillView.Size, ((ISecurityOrder)order).TradedInstrument);

                ICurrency txCurrency = null;
                if (((ISecurityOrder)order).TradedInstrument.IsCash)
                    txCurrency = (ICurrency)(((ISecurityOrder)order).TradedInstrument);
                else
                    txCurrency = (ICurrency)(((ISecurityOrder)order).TradedInstrument).CurrencyNominal;
                if (txCurrency.IsObsoleteCurrency)
                    txCurrency = txCurrency.ParentInstrument as ICurrency;

                decimal exRate = 0M;
                if (txCurrency.IsBase)
                    exRate = 1M;
                else if (txCurrency.ExchangeRate != null)
                    exRate = txCurrency.ExchangeRate.Rate;

                if (orderFillView.ServiceChargeAmount != 0)
                {
                    serviceCharge = new Money(orderFillView.ServiceChargeAmount, txCurrency);
                    serviceChargePercentage = orderFillView.ServiceChargePercentage;
                }

                Money amount = new Money(orderFillView.Amount, txCurrency);
                Money accruedInterest = null;
                if (orderFillView.AccruedInterestAmount != 0M)
                    accruedInterest = new Money(orderFillView.AccruedInterestAmount, txCurrency);

                ITradingJournalEntry tradingJournalEntry = TransactionAdapter.GetNewTradingJournalEntry(session, txCurrency.Symbol, orderFillView.TransactionDate);
                IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.Transaction);

                transaction = order.Fill(size, price, amount, exRate, counterpartyAccount, orderFillView.TransactionDate,
                                        orderFillView.TransactionTime, exchange, orderFillView.IsCompleteFill,
                                        serviceCharge, serviceChargePercentage, accruedInterest, tradingJournalEntry, lookups);

                transaction.ContractualSettlementDate = orderFillView.SettlementDate;

                TransactionMapper.Update(session, transaction);
            }
            finally
            {
                session.Close();
            }
        }

        public static DateTime GetSettlementDate(DateTime transactionDate, int orderId, int exchangeId, int counterpartyAccountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                DateTime settlementDate = transactionDate;
                IExchange exchange = null;

                if (exchangeId != 0)
                    exchange = ExchangeMapper.GetExchange(session, exchangeId);
                else if (counterpartyAccountId != 0)
                {
                    ICounterPartyAccount counterpartyAccount = (ICounterPartyAccount)AccountMapper.GetAccount(session, counterpartyAccountId);
                    if (counterpartyAccount != null && counterpartyAccount.DefaultExchange != null)
                        exchange = counterpartyAccount.DefaultExchange;
                }

                if (exchange != null)
                {
                    ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, orderId);
                    settlementDate = order.TradedInstrument.GetSettlementDate(transactionDate, exchange);
                }
                return settlementDate;
            }
            finally
            {
                session.Close();
            }
        }

        public static void PriceChanged(OrderFillView orderFillView)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                Price price;
                InstrumentSize size;
                Money amount;
                ITradeableInstrument tradedInstrument;

                IOrder order = OrderMapper.GetOrder(session, orderFillView.OrderId);

                if (!orderFillView.IsSizeBased)
                {
                    // Exchange rate (in base currency)
                    tradedInstrument = ((IOrderAmountBased)order).TradedInstrument;
                    price = new Price(orderFillView.Price, tradedInstrument.CurrencyNominal, tradedInstrument);
                    amount = new Money(orderFillView.Amount, (ICurrency)order.Value.Underlying);
                    if (tradedInstrument.CurrencyNominal.Key != amount.Underlying.Key)
                    {
                        if (!(tradedInstrument.CurrencyNominal.IsObsoleteCurrency && tradedInstrument.CurrencyNominal.ParentInstrument.Key == amount.Underlying.Key))
                            throw new ApplicationException("It is not possible to fill an order in a different currency.");
                    }
                    size = amount.CalculateSize(price);
                    orderFillView.Size = size.Quantity;
                }
                else
                {
                    tradedInstrument = ((IOrderSizeBased)order).TradedInstrument;
                    price = new Price(orderFillView.Price, tradedInstrument.CurrencyNominal, tradedInstrument);
                    size = new InstrumentSize(orderFillView.Size, tradedInstrument);
                    amount = size.CalculateAmount(price);
                    amount.XRate = orderFillView.ExchangeRate;
                    orderFillView.Amount = amount.Quantity;
                }

                // Check if the Price is still reliable
                IPriceDetail lastValidHistoricalPrice = HistoricalPriceMapper.GetLastValidHistoricalPrice(
                                                                                    session, tradedInstrument, orderFillView.TransactionDate);
                if (lastValidHistoricalPrice == null || lastValidHistoricalPrice.Price.IsZero)
                    orderFillView.Warning = string.Format("No price was found for {0:d}, so validation is not very reliable.",
                                                             orderFillView.TransactionDate);
                else
                {
                    // check if the price is within 1% of the last historical price
                    decimal rate = lastValidHistoricalPrice.Price.Quantity;

                    decimal diff = (price.Quantity - rate) / rate;
                    decimal diffPct = Math.Round(Math.Abs(diff), 4) * 100;
                    if (diffPct > 1)
                        orderFillView.Warning = string.Format("The price entered is {0:0.##}% {1} than the last known price for {2:d} ({3}).",
                                                              diffPct, (diff < 0 ? "lower" : "higher"), orderFillView.TransactionDate,
                                                              lastValidHistoricalPrice.Price.ShortDisplayString);

                    if (lastValidHistoricalPrice.WasOldDateBy(orderFillView.TransactionDate))
                        orderFillView.Warning += (orderFillView.Warning != string.Empty ? "\n" : "") +
                            string.Format("The last known price for {0:d} is {1} days old (last updated on {2:d}), so validation is not very reliable.",
                                          orderFillView.TransactionDate, (orderFillView.TransactionDate - lastValidHistoricalPrice.Date).Days,
                                          lastValidHistoricalPrice.Date);
                }
            }
            finally
            {
                session.Close();
            }
        }

        public static List<OrderEditView> GetOrderEditData(int orderId)
        {
            List<OrderEditView> allOrderFills = new List<OrderEditView>();

            IDalSession session = NHSessionFactory.CreateSession();
            IOrder order = OrderMapper.GetOrder(session, (int)orderId);
            OrderEditView oev = new OrderEditView(order.OrderID,
                                                order.PlacedValue.NumberOfDecimals,
                                                ((SecurityOrder)order).Route.Key);
            oev.IsSizeBased = order.IsSizeBased;
            allOrderFills.Add(oev);
            session.Close();

            return allOrderFills;
        }

        public static DataSet GetRoutes()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IList routes = session.GetListByHQL("from Route R where R.ResendSecurityOrdersAllowed = 1", null);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(routes, "Key, Name");

            session.Close();
            return ds;
        }

        public static DataSet GetOrderTransactions(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            ITransactionOrder[] transactions = null;

            IOrder order = OrderMapper.GetOrder(session, (int)orderId);

            transactions = new ITransactionOrder[order.Transactions.Count];
            order.Transactions.CopyTo(transactions, 0);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                transactions,
                @"Key, Order.OrderID, ValueSize.DisplayString, CounterValueSize.DisplayString, Price.DisplayString, ExchangeRate, 
                  TransactionDate, CreationDate");

            session.Close();

            return ds;
        }

        public static void UnApproveOrders(int[] orderIds)
        {
            bool success = false;

            if (orderIds == null || orderIds.Length == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);
                foreach (Order order in orders)
                    success = order.UnApprove();
                if (success)
                    OrderMapper.Update(session, orders);
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

        public static void SendOrder(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IOrder orderToSend = OrderMapper.GetOrder(session, orderId);
            ((IStgOrder)orderToSend).Send();
            OrderMapper.Update(session, orderToSend);
            session.Close();
        }

        public static void PlaceOrder(int orderId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IOrder orderToPlace = OrderMapper.GetOrder(session, orderId);
            ((IStgOrder)orderToPlace).Place();
            OrderMapper.Update(session, orderToPlace);
            session.Close();
        }

        public static void SaveOrderInfo(int OrderID, short PlacedValue_NumberOfDecimals, int iRoute)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            bool hasChanged = false;

            IOrder orderToPlace = OrderMapper.GetOrder(session, OrderID);
            if (orderToPlace.IsSizeBased)
                hasChanged = ((IStgSizeOrder)orderToPlace).SetNumberOfDecimals(PlacedValue_NumberOfDecimals);
            IRoute route = RouteMapper.GetRoute(session, iRoute);
            if (((IStgOrder)orderToPlace).ChangeRoute(route))
                hasChanged = true;
            if (hasChanged)
                OrderMapper.Update(session, orderToPlace);
            session.Close();
        }
    }
}
