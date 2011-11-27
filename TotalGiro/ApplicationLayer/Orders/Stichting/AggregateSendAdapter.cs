using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.OrderRouteMapper;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class AggregateSendAdapter
    {
        public static DataSet GetStichtingUnAggregatedChildOrders()
        {
            //"Key, Account.Number, Account.ShortName, DisplayTradedInstrumentIsin, TradedInstrument.DisplayName, Side, Value, Value.DisplayString, " +
            //"DisplayIsSizeBased, CreationDate"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return OrderMapper.GetOrders<ISecurityOrder>(session, OrderReturnClass.SecurityOrder, 
                    OrderAggregationLevel.AssetManager, ApprovalState.Approved, 
                    SecurityInfoOptions.ManagedsAcctsOnly, ParentalState.Null, 
                    new OrderStatusFilter(OrderStati.New))
                    .Select(c => new
                    {
                        c.Key,
                        Account_Number = c.Account.Number,
                        Account_ShortName = c.Account.ShortName,
                        c.DisplayTradedInstrumentIsin,
                        TradedInstrument_DisplayName = c.TradedInstrument.DisplayName,
                        c.Side,
                        c.Value,
                        Value_DisplayString = c.Value.DisplayString,
                        c.DisplayIsSizeBased,
                        c.CreationDate
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetAggregatedStgOrders()
        {
            //"Key, ParentOrder, ChildOrders.Count, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, RequestedInstrument, PlacedValue.Quantity, PlacedValue.DisplayString, DisplayIsSizeBased, CreationDate, OrderID, IsSendable, Route.Name, NeedsCurrencyConversion"
            //Route.Name
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return OrderMapper.GetOrders<IStgOrder>(session, OrderReturnClass.SecurityOrder, 
                    OrderAggregationLevel.Stichting, ApprovalState.UnApproved, 
                    SecurityInfoOptions.TradingAcctOnly, ParentalState.Null, 
                    new OrderStatusFilter(OrderStati.New))
                    .Select(c => new
                    {
                        c.Key,
                        c.ParentOrder,
                        ChildOrders_Count = c.ChildOrders.Count,
                        TradedInstrument_DisplayName = c.RequestedInstrument.DisplayName,
                        c.DisplayTradedInstrumentIsin,
                        c.Side,
                        c.RequestedInstrument,
                        PlacedValue_Quantity = c.PlacedValue.Quantity,
                        PlacedValue_DisplayString = c.PlacedValue.DisplayString, 
                        c.DisplayIsSizeBased, 
                        c.CreationDate, 
                        c.OrderID, 
                        c.IsSendable,
                        Route_Name = c.Route.Name, 
                        c.NeedsCurrencyConversion
                    })
                    .ToDataSet();
            }
        }

        public static void AggregateOrders(int[] orderIds, out string errorMessage)
        {
            AggregateOrders(orderIds, out errorMessage, false, null);
        }

        public static void AggregateOrders(int[] orderIds, out string errorMessage, bool isAggregateSpecial, int[] parentOrderIds)
        {
            string message;
            errorMessage = "";

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IList<IOrder> unStgAggregatedApprovedOrders = OrderMapper.GetOrders(session, orderIds);
                if (unStgAggregatedApprovedOrders != null && unStgAggregatedApprovedOrders.Count > 0)
                {
                    OrderRouteMapper.OrderRouteMapper mapper = OrderRouteMapper.OrderRouteMapper.GetInstance(session);

                    // If parentOrderIds NOT NULL -> Aggregate Special
                    IList<IOrder> aggregateUnApprovedStgOrders = null;
                    if (isAggregateSpecial)
                    {
                        if (parentOrderIds != null && parentOrderIds.Length > 0)
                            aggregateUnApprovedStgOrders = OrderMapper.GetOrders(session, parentOrderIds);
                    }
                    else
                    {
                        aggregateUnApprovedStgOrders = OrderMapper.GetOrders<IOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.Stichting,
                            ApprovalState.UnApproved, SecurityInfoOptions.TradingAcctOnly, ParentalState.Null, new OrderStatusFilter(OrderStati.New));
                    }
                    IList<IOrder> orders = Order.AggregateOrdersForStichting(aggregateUnApprovedStgOrders, unStgAggregatedApprovedOrders, mapper, out message);
                    if (orders != null)
                        OrderMapper.SaveAggregatedOrders(session, orders);
                }
                else
                    throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
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

        public static void DeAggregateOrder(int orderID)
        {
            bool unApproveChildren = false;
            IDalSession session = NHSessionFactory.CreateSession();

            Order order = (Order)OrderMapper.GetOrder(session, orderID);
            unApproveChildren = order.IsNetted || (order.IsStgOrder && ((IStgOrder)order).IsTypeConverted);

            if (order.DeAggregate(unApproveChildren))
                OrderMapper.Update(session, order);
            session.Close();
        }

        public static bool UnApproveOrders(int[] orderIds)
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
            return success;
        }

        public static void SendOrders(int[] orderIds)
        {
            if (orderIds == null || orderIds.Length == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);
                foreach (Order order in orders)
                {
                    order.Approve();
                }
                OrderMapper.Update(session, orders);
            }
        }

        public static void NettOrders(int[] orderIds, out string errorMessage)
        {
            errorMessage = "";
            if (orderIds == null || orderIds.Length == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            try
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IList<IOrder> notNettedStgOrders = OrderMapper.GetOrders(session, orderIds);
                    IList<IOrder> nettedUnApprovedOrders = OrderMapper.GetOrders<IOrder>(session, OrderReturnClass.SecurityOrder, OrderAggregationLevel.StichtingNetted,
                        ApprovalState.UnApproved, SecurityInfoOptions.TradingAcctOnly, ParentalState.Null, new OrderStatusFilter(OrderStati.New), null);

                    IList<IOrder> orders = Order.Nett(nettedUnApprovedOrders, notNettedStgOrders, out errorMessage);
                    if (orders != null && orders.Count > 0)
                        OrderMapper.SaveAggregatedOrders(session, orders);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public static void ConvertOrder(decimal price, int OrderID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, OrderID);
                Price p = new Price(price, order.TradedInstrument.CurrencyNominal, order.TradedInstrument);

                IOrderRouteMapper routeMapper = OrderRouteMapper.OrderRouteMapper.GetInstance(session);
                ISecurityOrder newOrder = order.Convert(p, routeMapper);
                if (newOrder != null)
                    OrderMapper.Insert(session, newOrder);
            }
        }

        public static void ConvertBondOrder(decimal price, int OrderID, DateTime settlementDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, OrderID);
                if (order.TradedInstrument.SecCategory.Key != SecCategories.Bond)
                    throw new ApplicationException("This is not a bond order.");
                
                if (order.IsSizeBased)
                    throw new ApplicationException("Totalgiro only supports conversion of amount based bond orders to a size based order.");

                IStgAmtOrder amtOrder = (IStgAmtOrder)order;

                Price p = new Price(price, order.TradedInstrument.CurrencyNominal, order.TradedInstrument);

                IOrderRouteMapper routeMapper = OrderRouteMapper.OrderRouteMapper.GetInstance(session);
                IStgSizeOrder newOrder = amtOrder.ConvertBondOrder(p, settlementDate, routeMapper);
                if (newOrder != null)
                    OrderMapper.Insert(session, newOrder);
            }
        }


        public static void ConvertFx(decimal exRate, decimal convertedAmount, int OrderID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IStgAmtOrder order = (IStgAmtOrder)OrderMapper.GetOrder(session, OrderID);

                decimal testAmt = order.Amount.Convert(exRate, order.TradedInstrument.CurrencyNominal).Abs().Quantity;
                if ((convertedAmount - testAmt) > 0.01M)
                    throw new ApplicationException("The converted amount does not match the exchange rate.");

                decimal sign = order.Amount.Sign ? 1M : -1M;
                Money convAmt = new Money(convertedAmount * sign,
                    order.TradedInstrument.CurrencyNominal,
                    order.TradedInstrument.CurrencyNominal.BaseCurrency,
                    exRate);

                IStgAmtOrder newOrder = order.ConvertFx(exRate, convAmt);
                if (newOrder != null)
                    OrderMapper.Insert(session, newOrder);
            }
        }

        public static ArrayList GetOrderEditData(object orderId)
        {
            ArrayList allOrderFills = new ArrayList();

            if (orderId != null)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, (int)orderId);
                    
                    string amountSymbol = "";
                    if (order.TradedInstrument.PriceType == PricingTypes.Direct)
                        amountSymbol = order.TradedInstrument.CurrencyNominal.AltSymbol;
                    else
                        amountSymbol = "%";
                    Price latestPrice = order.TradedInstrument.CurrentPrice.Price;
                    int numberofdecimals = order.RequestedInstrument.DecimalPlaces;
                    bool isBondOrder = order.TradedInstrument.SecCategory.Key == SecCategories.Bond;
                    DateTime expectedSettlementDate = DateTime.MinValue;
                    if (isBondOrder)
                    {
                        IExchange exchange = order.TradedInstrument.DefaultExchange ?? order.TradedInstrument.HomeExchange;
                        if (exchange != null)
                            expectedSettlementDate = order.TradedInstrument.GetSettlementDate(DateTime.Today, exchange);
                        else
                            expectedSettlementDate = Util.DateAdd(DateInterval.BusinessDay, 3, DateTime.Today, DateIntervalOptions.ExcludeWeekends, null);
                    }
                    allOrderFills.Add(new OrderEditView(order.OrderID, amountSymbol, latestPrice.Quantity, numberofdecimals, isBondOrder, expectedSettlementDate));
                }
            }
            return allOrderFills;
        }

        public static DateTime GetSettlementDate(object orderId, DateTime transactionDate)
        {
            DateTime settlementDate = DateTime.MinValue;
            if (orderId != null)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, (int)orderId);
                    IExchange exchange = order.TradedInstrument.DefaultExchange ?? order.TradedInstrument.HomeExchange;
                    if (exchange != null)
                        settlementDate = order.TradedInstrument.GetSettlementDate(transactionDate, exchange);
                    else
                        settlementDate = Util.DateAdd(DateInterval.BusinessDay, 3, transactionDate, DateIntervalOptions.ExcludeWeekends, null);
                }
            }
            return settlementDate;
        }


        public static List<OrderEditView> GetOrderFxConvertData(int orderId)
        {
            List<OrderEditView> allOrderFills = new List<OrderEditView>();
            if (orderId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, (int)orderId);
                    ITradeableInstrument instrument = order.TradedInstrument;
                    decimal exRate = instrument.CurrencyNominal.ExchangeRate.Rate;
                    decimal originalAmount = order.Amount.Quantity;
                    decimal convAmount = order.Amount.Convert(exRate, instrument.CurrencyNominal).Quantity;

                    OrderEditView oev = new OrderEditView(order.OrderID,
                                                        exRate,
                                                        convAmount, originalAmount,
                                                        instrument.CurrencyNominal.AltSymbol);
                    allOrderFills.Add(oev);
                }
            }
            return allOrderFills;
        }

        public static OrderEditView CheckPrice(OrderEditView orderEditView)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, orderEditView.OrderId);
                ITradeableInstrument tradedInstrument = order.TradedInstrument;
                Price price = new Price(orderEditView.Price, order.TradedInstrument.CurrencyNominal, order.TradedInstrument);

                // Check if the Price is still reliable
                IPriceDetail instrumentPrice = tradedInstrument.CurrentPrice;
                if (instrumentPrice == null || instrumentPrice.Price.IsZero)
                    orderEditView.Warning = string.Format("No price was found for {0} so the validation is not very reliable.", tradedInstrument.DisplayName);
                else
                {
                    // check if the price is within 1% of the last exRate
                    decimal rate = instrumentPrice.Price.Quantity;

                    decimal diff = (price.Quantity - rate) / rate;
                    decimal diffPct = Math.Round(Math.Abs(diff), 4) * 100;
                    if (diffPct > 1)
                        orderEditView.Warning = string.Format("The price you entered is {0}% {1} than the last known price ({2}).", diffPct.ToString("0.##"), (diff < 0 ? "smaller" : "higher"), instrumentPrice.Price.ShortDisplayString);

                    if (instrumentPrice.IsOldDate)
                        orderEditView.Warning += (orderEditView.Warning != string.Empty ? Environment.NewLine : "") + string.Format("The price found for {0} is old ({1}) so the validation is not very reliable.", tradedInstrument.DisplayName, instrumentPrice.Date.ToShortDateString());
                }
                return orderEditView;
            }
        }

        //public static OrderEditView GetLatestPrice(OrderEditView orderEditView)
        //{
        //    IDalSession session = NHSessionFactory.CreateSession();

        //    ISecurityOrder order = (ISecurityOrder)OrderMapper.GetOrder(session, orderEditView.OrderId);
        //    ITradeableInstrument tradedInstrument = order.TradedInstrument;
        //    Price price = tradedInstrument.CurrentPrice;

        //    return orderEditView;
        //}
    }
}
