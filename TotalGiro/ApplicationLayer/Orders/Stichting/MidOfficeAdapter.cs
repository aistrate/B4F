using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.TGTransactions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.BackOffice;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class MidOfficeAdapter
    {
        public static DataSet GetUnApprovedTrades()
        {
            //"Key, AccountB.ShortName, TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, TxSide, ValueSize, ValueSize.DisplayString, Price, Price.ShortDisplayString, CounterValueSize, CounterValueSize.DisplayString, ExchangeRate, TransactionDate, ContractualSettlementDate, ServiceCharge"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return session.GetTypedListByNamedQuery<OrderExecution>(
                    "B4F.TotalGiro.ApplicationLayer.Orders.Stichting.UnApprovedExecutions")
                    .Select(c => new
                    {
                        c.Key,
                        AccountBShortName =
                            c.AccountB.ShortName,
                        TradedInstrumentDisplayName =
                            c.TradedInstrument.DisplayName,
                        c.DisplayTradedInstrumentIsin,
                        c.TxSide,
                        c.ValueSize,
                        ValueSizeDisplayString =
                            c.ValueSize.DisplayString,
                        c.Price,
                        PriceShortDisplayString =
                            c.Price.ShortDisplayString,
                        c.CounterValueSize,
                        CounterValueSizeDisplayString =
                            c.CounterValueSize.DisplayString,
                        c.ExchangeRate,
                        c.TransactionDate,
                        c.ContractualSettlementDate,
                        c.ServiceCharge,
                        c.AccruedInterest,
                        c.Approved,
                        c.IsApproveable,
                        Allocations_Count = 
                            c.Allocations.Count,
                        ChildOrderCount_Count =
                            OrderMapper.GetChildOrderCount(session, c.Order.Key)
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetOrderFromTransaction(object tradeId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                if (tradeId != null)
                {
                    IOrderExecution trade = (IOrderExecution)TransactionMapper.GetTransaction(session, (int)tradeId);

                    if (trade != null && trade.Order != null)
                    {
                        int[] orderIds = new int[1];
                        orderIds[0] = trade.Order.OrderID;
                        IList<IOrder> orders = OrderMapper.GetOrders(session, orderIds);

                        //"Key, Side, Value, Value.DisplayString, OpenValue, OpenValue.DisplayString, DisplayIsSizeBased, Status, DisplayStatus, Route, OrderID, CreationDate");
                        ds = orders
                            .Cast<IStgOrder>()
                            .Select(c => new
                            {
                                c.Key,
                                c.Side,
                                c.Value,
                                Value_DisplayString = c.Value.DisplayString,
                                c.OpenValue,
                                OpenValue_DisplayString = c.OpenValue.DisplayString,
                                c.DisplayIsSizeBased,
                                c.Status,
                                c.DisplayStatus,
                                c.Route,
                                c.OrderID,
                                c.CreationDate
                            })
                            .ToDataSet();
                    }
                }
                return ds;
            }
        }

        public static void CancelTrade(int transactionID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ITransaction tradeToCancel = TransactionMapper.GetTransaction(session, transactionID);
            TransactionMapper.Delete(session, tradeToCancel);
            session.Close();
        }

        public static void ApproveTrades(int[] tradeIds, out string errorMessage)
        {
            IDalSession session = null;
            IFeeFactory fees = null;
            IGLLookupRecords lookupsAI = null;
            errorMessage = "";

            try
            {
                foreach (int tradeid in tradeIds)
                {
                    //Approve the Execution only
                    session = NHSessionFactory.CreateSession();
                    IInternalEmployeeLogin employee = LoginMapper.GetCurrentEmployee(session);
                    IOrderExecution trade = (IOrderExecution)TransactionMapper.GetTransaction(session, tradeid);
                    fees = FeeFactory.GetInstance(session, FeeFactoryInstanceTypes.Commission, true);
                    IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.Transaction);
                    IEnumerable<int> childOrdersToBeFilled = OrderMapper.GetChildFillableOrderKeys(session, trade.Order.Key);

                    if (!trade.Approved)
                    {
                        trade.Approve(employee);
                        if ((trade.AccountA.AccountType == AccountTypes.Customer) 
                            || (trade.AccountB.AccountType == AccountTypes.Customer)
                            || (trade.AccountB.AccountType == AccountTypes.Nostro)
                            || (trade.AccountB.AccountType == AccountTypes.Crumble))
                        {
                            //todo
                            //Client settlement date may not occur on same day.
                            // may have to split this off into seperate functionality
                            //ITradingJournalEntry execSetlementJournal = TransactionAdapter.GetNewTradingJournalEntry(session, trade.TradingJournalEntry, trade.TransactionDate);
                            trade.ClientSettle(trade.TradingJournalEntry);
                            trade.IsSettled = true;

                            // when bond -> do accrued interest stuff
                            if (trade.TradedInstrument.SecCategory.Key == SecCategories.Bond)
                            {
                                if (lookupsAI == null)
                                    lookupsAI = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.AccruedInterest);
                                if (BondCouponAccrualAdapter.ProcessBondPosition(session, trade.AccountA, trade.TradedInstrument, trade.Exchange, trade.TransactionDate, lookupsAI))
                                    session.Update(trade.AccountA);
                            }
                        }
                        TransactionMapper.Update(session, trade);
                    }

                    //For each child allocate and approve
                    foreach (int childOrderID in childOrdersToBeFilled)
                    {
                        using (IDalSession session2 = NHSessionFactory.CreateSession())
                        {
                            IOrderExecution parent = (IOrderExecution)TransactionMapper.GetTransaction(session2, tradeid);
                            ITradingJournalEntry tradingJournalEntry = TransactionAdapter.GetNewTradingJournalEntry(session2, parent.CounterValueSize.UnderlyingShortName.ToUpper(), parent.TransactionDate);

                            IOrder child = OrderMapper.GetOrder(session2, childOrderID, SecurityInfoOptions.NoFilter);
                            IOrderAllocation alloc = child.FillasAllocation(parent, tradingJournalEntry, lookups, fees);
                            alloc.Approve(employee);
                            //TransactionMapper.Update(session2, alloc);

                            //todo
                            //Client settlement date may not occur on same day.
                            // may have to split this off into seperate functionality
                            //ITradingJournalEntry clientSetlementJournal = TransactionAdapter.GetNewTradingJournalEntry(session2, tradingJournalEntry, parent.TransactionDate);
                            alloc.ClientSettle(tradingJournalEntry);
                            TransactionMapper.Update(session2, alloc);

                            // when bond -> do accrued interest stuff
                            if (alloc.TradedInstrument.SecCategory.Key == SecCategories.Bond)
                            {
                                if (lookupsAI == null)
                                    lookupsAI = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.AccruedInterest);
                                if (BondCouponAccrualAdapter.ProcessBondPosition(session2, alloc.AccountA, alloc.TradedInstrument, alloc.Exchange, alloc.TransactionDate, lookupsAI))
                                    session2.Update(alloc.AccountA);
                            }
                        }
                    }

                    //Now create the crumble
                    {
                        using (IDalSession session3 = NHSessionFactory.CreateSession())
                        {
                            trade = (IOrderExecution)TransactionMapper.GetTransaction(session3, tradeid);
                            int filledChildOrderCount = OrderMapper.GetChildOrderCount(session, trade.Order.Key);
                            if (!trade.IsAllocated && trade.TotalSizeAllocated.IsNotZero)
                            {
                                if (filledChildOrderCount != trade.Allocations.Count)
                                    throw new ApplicationException("The trade is not fully allocated, so the crumble can not be created.");
                                
                                ITradingJournalEntry crumbleJournal = TransactionAdapter.GetNewTradingJournalEntry(session3, trade.TradingJournalEntry, trade.TransactionDate);
                                ICrumbleTransaction crumble =  trade.CreateCrumble(lookups, crumbleJournal);
                                if (crumble != null)
                                {
                                    crumble.Approve(LoginMapper.GetCurrentEmployee(session));
                                    //TransactionMapper.Update(session3, trade);

                                    //ITradingJournalEntry crumbleSettleJournal = TransactionAdapter.GetNewTradingJournalEntry(session3, trade.TradingJournalEntry, trade.TransactionDate);
                                    crumble.ClientSettle(trade.TradingJournalEntry);
                                    TransactionMapper.Update(session3, trade);

                                    // when bond -> do accrued interest stuff
                                    if (crumble.TradedInstrument.SecCategory.Key == SecCategories.Bond)
                                    {
                                        if (lookupsAI == null)
                                            lookupsAI = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.AccruedInterest);
                                        if (BondCouponAccrualAdapter.ProcessBondPosition(session3, crumble.AccountA, crumble.TradedInstrument, crumble.Exchange, crumble.TransactionDate, lookupsAI))
                                            session3.Update(crumble.AccountA);
                                    }
                                }
                            }
                        }
                    }

                    //Check the flag Isallocated
                    session = NHSessionFactory.CreateSession();
                    trade = (IOrderExecution)TransactionMapper.GetTransaction(session, tradeid);
                    trade.SetIsAllocated();
                    TransactionMapper.Update(session, trade);
                    session.Close();

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                if (fees != null)
                    fees.CloseSession();
                if (session.IsOpen) session.Close();
            }
            //try
            //{
            //    IList trades = TransactionMapper.GetTransactions(session, tradeIds);
            //    fees = FeeFactory.GetInstance(session, FeeFactoryInstanceTypes.Commission, true);
            //    foreach (ITransaction trade in trades)
            //    {
            //        trade.Approve();
            //        TransactionMapper.Update(session, trade);

            //        if (trade.IsOrderExecution)
            //        {
            //            //IOrder order = ((IOrderExecution)trade).Order;

            //            //order.Allocate((IOrderExecution)trade, fees);

            //            //OrderMapper.Update(session, order);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    errorMessage = ex.Message;
            //}
            //finally
            //{
            //    if (fees != null)
            //        fees.CloseSession();
            //    session.Close();
            //}
        }
    }
}
