using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Common
{
    public static class OrderDetailsAdapter
    {
        public static DataSet GetOrderDetails(int orderId)
        {
            //@"Key, Account.Number, Account.ShortName, ActionType, AllocationDate, Amount, Approved, BaseAmount, CancelStatus, Commission,
            //CommissionInfo, CreationDate, DateClosed, Err, ErrDescription, EstimatedAmount, ExRate, FilledValue, GrossAmount, 
            //GrossAmountBase, IsAggregateOrder, IsClosed, IsCompleteFilled, IsFillable, IsMonetary, IsNetted, IsSecurity, IsStgOrder, 
            //IsUnApproveable, LastUpdated, OpenAmount, OpenValue, OrderCurrency, OrderType, PlacedValue, Price, 
            //RequestedInstrument.DisplayName, Side, Status, TopParentDisplayStatus, TopParentOrder");            
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IOrder order = OrderMapper.GetOrder(session, orderId, SecurityInfoOptions.NoFilter);
                if (order != null)
                {
                    List<IOrder> orders = new List<IOrder>();
                    orders.Add(order);
                    return orders
                    .Select(c => new
                    {
                        c.Key,
                        AccountNumber =
                            c.Account.Number,
                        AccountShortName =
                            c.Account.ShortName,
                        c.ActionType,
                        c.AllocationDate,
                        c.Amount,
                        c.Approved,
                        c.BaseAmount,
                        c.CancelStatus,
                        c.Commission,
                        c.CommissionInfo,
                        c.CreationDate,
                        c.DateClosed,
                        c.Err,
                        c.ErrDescription,
                        c.EstimatedAmount,
                        c.ExRate,
                        c.FilledValue,
                        c.GrossAmount,
                        c.GrossAmountBase,
                        c.IsAggregateOrder,
                        c.IsClosed,
                        c.IsCompleteFilled,
                        c.IsFillable,
                        c.IsMonetary,
                        c.IsNetted,
                        c.IsSecurity,
                        c.IsStgOrder,
                        c.IsUnApproveable,
                        c.LastUpdated,
                        c.OpenAmount,
                        c.OpenValue,
                        c.OrderCurrency,
                        c.OrderType,
                        c.PlacedValue,
                        c.Price,
                        RequestedInstrument =
                            c.RequestedInstrument.DisplayName,
                        c.Side,
                        c.Status,
                        c.TopParentDisplayStatus,
                        c.TopParentOrder,
                        ServiceCharge =
                            (c.IsSecurity && ((ISecurityOrder)c).ServiceCharge != null ? ((ISecurityOrder)c).ServiceCharge.DisplayString : null),
                        ParentOrder_Key = (LoginMapper.IsLoggedInAsStichting(session) ?
                            (order.ParentOrder != null ? order.ParentOrder.Key : 0) :
                            (order.ParentOrder != null && !order.ParentOrder.IsStgOrder ? order.ParentOrder.Key : 0)),
                        ClassName =
                            c.GetType().Name,
                    })
                    .ToDataSet();
                }
                else
                    return null;
            }
        }

        public static DataSet GetTradeDetails(int tradeId)
        {
            //@"Key, AccountA.DisplayNumberWithName, AccountB.DisplayNumberWithName, ValueSize.DisplayString, CounterValueSize.DisplayString,
            //Price.DisplayString,Commission.DisplayString, ServiceCharge.DisplayString, Tax.DisplayString, TaxPercentage, TransferFee.DisplayString, 
            //TradedInstrument.DisplayNameWithIsin,ExchangeRate,TransactionDate,ContractualSettlementDate, Exchange.ExchangeName, Approved, DisplayTxSide, 
            //CreationDate,IsStorno, TransactionType, Description, Order.Key"            
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ITransaction trade = TransactionMapper.GetTransaction(session, tradeId);
                List<ITransaction> trades = new List<ITransaction>();
                trades.Add(trade);
                return trades
                .Select(c => new
                {
                    c.Key,
                    AccountAName =
                        c.AccountA.DisplayNumberWithName,
                    AccountBName =
                        c.AccountB.DisplayNumberWithName,
                    ValueSize =
                        c.ValueSize.DisplayString,
                    CounterValueSize =
                        c.CounterValueSize.DisplayString,
                    Price =
                        c.Price.DisplayString,
                    ServiceCharge =
                        c.ServiceCharge.DisplayString,
                    AccruedInterest =
                        c.AccruedInterest != null ? c.AccruedInterest.DisplayString : "",
                    IsBond = c.TradedInstrument.SecCategory.Key == SecCategories.Bond,
                    TradedInstrument =
                        c.TradedInstrument.DisplayNameWithIsin,
                    c.ExchangeRate,
                    c.TransactionDate,
                    c.ContractualSettlementDate,
                    ExchangeName =
                        (c.Exchange != null ? c.Exchange.ExchangeName : ""),
                    c.Approved,
                    TxSide = 
                        c.TxSide.ToString(),
                    c.CreationDate,
                    c.IsStorno,
                    TransactionType =
                        c.TransactionType.ToString(),
                    ClassName =
                        c.GetType().Name,
                    Order_Key =
                        (c.TransactionType == TransactionTypes.TransactionOrder ? ((ITransactionOrder)c).Order.Key : 0),
                    AuditLogKey =
                        (c.MigratedTradeKey != 0 ? c.MigratedTradeKey : c.Key)
                })
                .ToDataSet();
            }
        }
    
    }
}
