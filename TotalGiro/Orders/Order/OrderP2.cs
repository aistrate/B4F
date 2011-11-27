using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.OrderRouteMapper;
using B4F.TotalGiro.Routes;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Abstract class describing the general functionality for an order object
    /// </summary>
    abstract public partial class Order : IOrder
    {
        #region Validation

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual OrderValidationResult Validate()
        {
            // Buy  -> Check Cash (Buying Power)
            // Sell -> Check the Positions

            string message = "";
            OrderValidationResult retVal = new OrderValidationResult(OrderValidationSubType.Success, "");

            //Check if an instruction exist - Invalid_InstructionExists
            retVal = checkInstructionExists();
            if (retVal.Type != OrderValidationSubType.Success)
                return retVal;

            if (Side == Side.Buy)
            {
                if (!Account.AccountOwner.IsStichting)
                {
                    // check the Cash -> Buying Power check
                    // BP = All Cash (incl cash management fund) - Open Orders - Open Trades (Not Approved)
                    Money currentOrderAmount;
                    Money totalCashAmount;
                    string oldPriceWarning = "";

                    if (!IsMonetary && ((ISecurityOrder)this).TradedInstrument.SecCategory.Key == SecCategories.CashManagementFund)
                        totalCashAmount = Account.TotalPositionAmount(PositionAmountReturnValue.Cash);
                    else
                        totalCashAmount = Account.TotalPositionAmount(PositionAmountReturnValue.BothCash);

                    if (totalCashAmount == null || totalCashAmount.IsZero)
                    {
                        message = string.Format("This order ({0}) can not be entered since there is no cash.", Value.ToString());
                        return new OrderValidationResult(OrderValidationSubType.Invalid_NoCash, message);
                    }

                    if (IsSizeBased)
                    {
                        // if closure -> check not another buy order already exists.
                        OrderValidationResult buyCloseCheck = checkBuyCloseOrders();
                        if (buyCloseCheck.Type != OrderValidationSubType.Success)
                            return buyCloseCheck;

                        // Get the Price of the Instrument
                        IPriceDetail priceDetail = RequestedInstrument.CurrentPrice;

                        if (priceDetail == null)
                        {
                            message = string.Format("The order validation is not very accurate since no current price is available for {0}.", Value.Underlying.Name);
                            return new OrderValidationResult(OrderValidationSubType.Warning_NoCurrentPrice, message);
                        }
                        else
                        {
                            Money accruedInterest = ((ISecurityOrder)this).AccruedInterest;
                            currentOrderAmount = ((Money)(Value.CalculateAmount(priceDetail.Price) + Commission + accruedInterest)).CurrentBaseAmount;
                            if (priceDetail.IsOldDate)
                            {
                                oldPriceWarning = string.Format("The order validation is not very accurate since the last available price ({0}) is from {1}.", priceDetail.Price.ToString(), priceDetail.Date.ToShortDateString());
                            }
                        }
                    }
                    else
                    {
                        currentOrderAmount = GrossAmountBase;

                        //// Check when Foreign currency amount -> if foreign currency is available
                        if (!Value.Underlying.ToCurrency.IsBase)
                        {
                            ICashPosition posFx = Account.Portfolio.PortfolioCashGL.GetPosition(Value.Underlying.ToCurrency);
                            Money orderAmtFx = Account.OpenOrdersForAccount.TotalAmountInSpecifiedNominalCurrency((ICurrency)Value.Underlying);
                            if (posFx == null)
                            {
                                message = string.Format("This order ({0}) can not be placed since the account doesn't have a {1} position.", GrossAmount.DisplayString, Value.Underlying.Name);
                                retVal = new OrderValidationResult(OrderValidationSubType.Invalid_NoCash, message);
                            }
                            else if (!((Money)(posFx.SettledSize - orderAmtFx - GrossAmount)).Sign)
                            {
                                string messageOpenOrders = "";
                                if (orderAmtFx != null && orderAmtFx.IsNotZero)
                                    messageOpenOrders = string.Format(" minus the open order amount ({0}) in {1}", orderAmtFx.ToString(), Value.Underlying.Name);
                                message = string.Format("This order ({0}) exceeds the current {1} cash amount ({2}){3}.", GrossAmount.DisplayString, Value.Underlying.Name, posFx.SettledSize.ToString(), messageOpenOrders);
                                retVal = new OrderValidationResult(OrderValidationSubType.Invalid_NotEnoughCash, message);
                            }
                            if (retVal.Type != OrderValidationSubType.Success)
                            {
                                retVal.Message += Environment.NewLine + string.Format("Place a {0} order instead.", Account.AccountOwner.StichtingDetails.BaseCurrency.ToString());
                                return retVal;
                            }
                        }
                    }

                    // Consider both Buy & Sell orders
                    Money openOrderAmount = Account.OpenOrderAmount(OpenOrderAmountReturnValue.Gross);
                    if (!((Money)(totalCashAmount - openOrderAmount - currentOrderAmount)).Sign)
                    {
                        if (openOrderAmount != null && openOrderAmount.IsNotZero)
                            message = string.Format(" minus the open order amount ({0})", openOrderAmount.DisplayString);
                        message = string.Format("This order ({0}) exceeds the total cash amount ({1}){2}.", currentOrderAmount.DisplayString, totalCashAmount.DisplayString, message);
                        retVal = new OrderValidationResult(OrderValidationSubType.Invalid_NotEnoughCash, message);
                    }

                    retVal.Message += oldPriceWarning;
                    if (oldPriceWarning != string.Empty && retVal.Type == OrderValidationSubType.Success)
                    {
                        retVal.Type = OrderValidationSubType.Warning_OldPrice;
                    }
                }
            }
            else
            {
                // Get the current Position size
                InstrumentSize positionSize = null;
                IFundPosition position = Account.Portfolio.PortfolioInstrument.GetPosition((ITradeableInstrument) RequestedInstrument);
                if (position != null)
                    positionSize = position.Size;

                if (positionSize == null || positionSize.IsZero)
                {
                    message = string.Format("You tried to sell {0} but there are no positions found.", RequestedInstrument.Name);
                    return new OrderValidationResult(OrderValidationSubType.Invalid_NoPosition, message);
                }


                // Get the positions size of the order
                PredictedSize predSize;
                if (!IsSizeBased)
                {
                    // Amount based -> Get the Price/ExRate
                    predSize = RequestedInstrument.PredictSize(Amount);

                    // If there is no Price/ExRate found -> warning
                    if (predSize.Status == PredictedSizeReturnValue.NoRate)
                    {
                        message = string.Format("The order validation is not very accurate since no current {0} is available for {1}.", (RequestedInstrument.IsCash ? "exchangerate" : "price"), Value.Underlying.Name);
                        return new OrderValidationResult(OrderValidationSubType.Warning_NoCurrentPrice, message);
                    }
                }
                else
                {
                    predSize = new PredictedSize(Value, DateTime.Now);
                }

                // Get the open order size
                InstrumentSize openOrderSize = null;
                if (Account.OpenOrdersForAccount != null && Account.OpenOrdersForAccount.Count > 0)
                    openOrderSize = Account.OpenOrdersForAccount.TotalSize(RequestedInstrument);

                if (!((InstrumentSize)(positionSize + (predSize.Size + openOrderSize))).Sign)
                {
                    message = string.Format("You tried to sell {0} {1} but in the portfolio only {2} positions were found", predSize.Size.Abs().ToString(), RequestedInstrument.Name, positionSize.ToString());
                    if (openOrderSize != null && openOrderSize.IsNotZero)
                        message += Environment.NewLine + string.Format(" and order(s) existed for {0}.", openOrderSize.ToString());
                    else
                        message += ".";
                    retVal = new OrderValidationResult(OrderValidationSubType.Invalid_NotEnoughPosition, message);
                }

                if (predSize.Status == PredictedSizeReturnValue.OldRateDate)
                {
                    retVal.Message += (retVal.Message == string.Empty ? "" : Environment.NewLine) + string.Format("The order validation is not very accurate since the last available {0} ({1}) is from {2}.", (RequestedInstrument.IsCash ? "exchangerate" : "price"), predSize.Rate, predSize.RateDate.ToShortDateString());
                    if (retVal.Type == OrderValidationSubType.Success)
                    {
                        retVal.Type = OrderValidationSubType.Warning_OldPrice;
                    }
                }
            }

            // Check opposite orders
            OrderValidationResult sideCheck = checkSideOrders();
            if (sideCheck.Type != OrderValidationSubType.Success)
            {
                retVal.Message += (retVal.Message == string.Empty ? "" : Environment.NewLine) + sideCheck.Message;
                if (retVal.Type == OrderValidationSubType.Success)
                {
                    retVal.Type = sideCheck.Type;
                }
            }

            return retVal;
        }

        private OrderValidationResult checkSideOrders()
        {
            string message = "";
            OrderValidationResult retVal = new OrderValidationResult(OrderValidationSubType.Success, "");
            OrderSideFilter opSideFilter = (Side == Side.Buy ? OrderSideFilter.Sell : OrderSideFilter.Buy);

            if (Account.OpenOrdersForAccount != null && Account.OpenOrdersForAccount.Count > 0)
            {
                IAccountOrderCollection col = Account.OpenOrdersForAccount.Filter(RequestedInstrument, opSideFilter);
                if (col != null && col.Count > 0)
                {
                    message = string.Format("Warning: a {0} order already exists for {1}.", opSideFilter.ToString(), RequestedInstrument.Name);
                    retVal = new OrderValidationResult(OrderValidationSubType.Warning_OppositeSideOrder, message);
                }
            }
            return retVal;
        }

        /// <summary>
        /// if closure -> check not another buy order already exists.
        /// </summary>
        /// <returns></returns>
        private OrderValidationResult checkBuyCloseOrders()
        {
            string message = "";
            OrderValidationResult retVal = new OrderValidationResult(OrderValidationSubType.Success, "");

            if (((IOrderSizeBased)this).IsClosure && Side == Side.Buy && Account.OpenOrdersForAccount != null && Account.OpenOrdersForAccount.Count > 0)
            {
                OrderSideFilter sideFilter = OrderSideFilter.Buy;
                IAccountOrderCollection col = Account.OpenOrdersForAccount.Filter(RequestedInstrument, sideFilter);
                if (col != null && col.Count > 0)
                {
                    message = string.Format("Warning: {0} buy order(s) with a total size of {1} already exists for {2}.", 
                        col.Count, col.TotalSize(RequestedInstrument), RequestedInstrument.Name);
                    retVal = new OrderValidationResult(OrderValidationSubType.Warning_BuyCloseOrderAlreadyExists, message);
                }
            }
            return retVal;
        }

        private OrderValidationResult checkInstructionExists()
        {
            string message = "";
            OrderValidationResult retVal = new OrderValidationResult(OrderValidationSubType.Success, "");

            if (Side == Side.Sell)
            {
                IInstructionCollection instructions = ((IAccountTypeCustomer)Account).ActiveRebalanceInstructions;
                if (instructions != null && instructions.Count > 0)
                {
                    foreach (IInstruction instruction in instructions)
                    {
                        if (instruction.IsActive)
                        {
                            message = string.Format("This order can not be entered since the account {0} is in a rebalance instruction (status {1}).", Account.ShortName, instruction.DisplayStatus);
                            retVal = new OrderValidationResult(OrderValidationSubType.Invalid_InstructionExists, message);
                            break;
                        }
                    }
                }
            }
            return retVal;
        }

        #endregion

        /// <summary>
        /// Approves an order. The approves attribute is set to true and a check on the corresponding money order is done.
        /// </summary>
        /// <returns>True if approved else false</returns>
        virtual public bool Approve()
        {
            this.Approved = true;
            this.ApprovalDate = DateTime.Now;
            return true;
        }

        /// <summary>
        /// UnApproves an order when allowed
        /// </summary>
        /// <returns>True if UnApprove was successfull</returns>
        public virtual bool UnApprove()
        {
            bool retVal = false;

            if (Approved)
            {
                // Only allow unapprove when ParentOrder is null
                if (ParentOrder != null)
                    throw new ApplicationException("It is not allowed to unapprove an order with a parent order attached");

                // If Stichting Order then only allow when still open & editable
                if (IsStgOrder)
                {
                    OrderStatus status = OrderStateMachine.GetOrderStatus(Status);
                    if (!(status != null && status.IsOpen && status.IsEditable))
                        throw new ApplicationException(string.Format("It is not allowed to unapprove order #{0} because it is no longer editable", this.Key.ToString()));
                }

                this.Approved = false;
                this.ApprovalDate = DateTime.MinValue;
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Is this order Cancellable
        /// </summary>
        public virtual bool IsCancellable
        {
            get 
            {
                bool retVal = true;

                if (Approved)
                {
                    // Only allow unapprove when ParentOrder is null
                    if (ParentOrder != null)
                        retVal = false;

                    // If Stichting Order then only allow when still open & editable
                    if (IsStgOrder)
                    {
                        OrderStatus status = OrderStateMachine.GetOrderStatus(Status);
                        if (!(status != null && status.IsOpen && status.IsEditable))
                            retVal = false;
                    }

                }
                return retVal;
            } 
        }

        /// <summary>
        /// Sets the order status to New. Changes to the status are checked by the OrderStateMachine
        /// </summary>
        /// <returns>True if successful</returns>
        virtual public bool SetNew()
        {
            bool retVal = false;
            retVal = OrderStateMachine.SetNewStatus(this, OrderStateEvents.New);
            return retVal;
        }

        /// <summary>
        /// Sets the order status to Routed. Changes to the status are checked by the OrderStateMachine
        /// </summary>
        /// <returns>True if successful</returns>
        protected virtual bool Send()
        {
            bool retVal = false;
            retVal = OrderStateMachine.SetNewStatus(this, OrderStateEvents.Send);
            return retVal;
        }

        /// <summary>
        /// Sets the order status to Placed. Changes to the status are checked by the OrderStateMachine
        /// </summary>
        /// <returns>True if successful</returns>
        protected virtual bool Place()
        {
            bool retVal = false;
            retVal = OrderStateMachine.SetNewStatus(this, OrderStateEvents.Place);
            return retVal;
        }

        /// <summary>
        /// Sets the order status to Cancelled. Changes to the status are checked by the OrderStateMachine
        /// </summary>
        /// <returns>True if successful</returns>
        virtual public bool Cancel()
        {
            return Cancel(false);
        }

        /// <summary>
        /// Sets the order status to Cancelled. Changes to the status are checked by the OrderStateMachine
        /// </summary>
        /// <param name="recursive">Propagate cancel to child orders</param>
        /// <returns>True if successful</returns>
        virtual public bool Cancel(bool recursive)
        {
            IOrder parent = (IOrder)this.ParentOrder;

            // Check if this is a MoneyOrder attached to a AmountBased order
            if (IsMonetary)
            {
                if (((IMonetaryOrder)this).MoneyParent != null)
                    throw new ApplicationException(string.Format("It is not possibe to Cancel order #{0} because it is attached to a amount based order (#{1}). Cancel the parent instead.", this.Key.ToString(), ((IMonetaryOrder)this).MoneyParent.OrderID.ToString()));
            }

            // Edit the parent
            if (parent != null && !(recursive))
            {
                OrderStatus status = OrderStateMachine.GetOrderStatus(parent.Status);
                if (status.IsOpen)
                {
                    if (!(status.IsEditable))
                    {
                        throw new ApplicationException(string.Format("It is not possibe to Cancel order #{0} because it's parent is no longer editable", this.Key.ToString()));
                    }
                    else
                    {
                        ((Order)parent).ChildOrders.Remove(this);
                    }
                }
            }

            // Set the status
            OrderStateMachine.SetNewCancelStatus(this, OrderCancelStateEvents.Cancel);

            // If not a converted order
            // Cancel the Children
            if (!this.IsTypeConverted)
            {
                if (this.IsAggregateOrder)
                {
                    foreach (IOrder child in ((Order)this).ChildOrders)
                    {
                        child.Cancel();
                    }
                }
            }
            else
            {
                // Orphan the child
                if (((IOrder)this).ChildOrders.Count == 1)
                    ((IOrder)this).ChildOrders[0].SetParentOrder(null);
            }

            return true;
        }

        /// <summary>
        /// Change the order route. This can only be done on stichting level aggregated orders which have the new status
        /// </summary>
        /// <returns>True if successful</returns>
        protected bool changeRoute(IRoute newroute)
        {
            bool retVal = false;

            if (this.IsAggregateOrder && this.IsStgOrder && this.Status == OrderStati.New)
            {
                if (newroute != null && !this.Route.Equals(newroute))
                {
                    this.Route = newroute;
                    retVal = true;
                }
            }
            return retVal;
        }

        #region Aggregation

        public static IList<IOrder> AggregateOrders(IList<IOrder> aggregateUnApprovedOrders, IList<IOrder> unAggregatedApprovedOrders, out string message)
        {
            aggregatedOrders = new Dictionary<OrderSearchKey, IOrder>();
            aggregatedOrderKeys = new List<OrderSearchKey>();
            IList<IOrder> result = new List<IOrder>();
            OrderSearchKey SearchKey = null;
            //IList unAggregatedApprovedOrders = new Order.DataAccessLayer().GetUnAggregatedOrders(true);
            int count = 0;

            if (unAggregatedApprovedOrders == null || unAggregatedApprovedOrders.Count == 0)
            {
                message = "";
                return null;
            }

            checkAggregateOrders(aggregateUnApprovedOrders, false);

            foreach (Order newOrder in unAggregatedApprovedOrders)
            {
                if (newOrder.parentOrder == null)
                {
                    if (newOrder.Approved)
                    {
                        SearchKey = addToList(newOrder, null, false, false);
                        ((IOrder)aggregatedOrders[SearchKey]).ChildOrders.Add(newOrder);
                        // Check for MoneyOrder
                        SearchKey.Updated = true;
                    }
                    else
                        count++;
                }
            }

            message = "";
            if (count > 0)
                message = string.Format("{0} orders could not be aggregated since they were not approved.", count.ToString());
            //saveAggregatedOrders();

            IAggregatedOrder orderToSave;
            for (int i = 0; i < aggregatedOrderKeys.Count; i++)
            {
                OrderSearchKey key = (OrderSearchKey)aggregatedOrderKeys[i];
                if (key.Updated)
                {
                    orderToSave = (IAggregatedOrder)aggregatedOrders[key];
                    result.Add((IOrder)orderToSave);
                }
            }
            return result;
        }

        public static IList<IOrder> AggregateOrdersForStichting(IList<IOrder> aggregateUnApprovedStgOrders, IList<IOrder> unStgAggregatedApprovedOrders, IOrderRouteMapper routeMapper, out string message)
        {
            aggregatedOrders = new Dictionary<OrderSearchKey, IOrder>();
            aggregatedOrderKeys = new List<OrderSearchKey>();
            IList<IOrder> result = new List<IOrder>();
            OrderSearchKey SearchKey = null;
            //IList unStgAggregatedApprovedOrders = new Order.DataAccessLayer().GetUnAggregatedOrders(true);
            int count = 0;

            if (unStgAggregatedApprovedOrders == null || unStgAggregatedApprovedOrders.Count == 0)
            {
                message = "";
                return null;
            }

            checkAggregateOrders(aggregateUnApprovedStgOrders, false);

            foreach (Order newOrder in unStgAggregatedApprovedOrders)
            {
                if (newOrder.parentOrder == null)
                {
                    if (newOrder.Approved)
                    {
                        SearchKey = addToList(newOrder, routeMapper, true, false);
                        ((IOrder)aggregatedOrders[SearchKey]).ChildOrders.Add(newOrder);
                        SearchKey.Updated = true;
                    }
                    else
                        count++;
                }
            }

            message = "";
            if (count > 0)
                message = string.Format("{0} orders could not be aggregated since they were not approved.", count.ToString());
            //saveAggregatedOrders();

            IStgOrder orderToSave;
            for (int i = 0; i < aggregatedOrderKeys.Count; i++)
            {
                OrderSearchKey key = (OrderSearchKey)aggregatedOrderKeys[i];
                if (key.Updated)
                {
                    orderToSave = (IStgOrder)aggregatedOrders[key];
                    // Calculate Service Charge
                    if (((IOrder)orderToSave).OrderType == OrderTypes.AmountBased)
                    {
                        StgAmtOrder o = (StgAmtOrder)orderToSave;
                        o.setServiceChargeForAmountBasedOrder();
                    }
                    ((IOrder)orderToSave).Validate();
                    result.Add((IOrder)orderToSave);
                }
            }
            return result;
        }

        private static void checkAggregateOrders(IList<IOrder> orderList, bool netting)
        {
            int childCount = 0;
            int wrongStatusCount = 0;
            string err = "";

            if (orderList != null)
            {
                foreach (Order order in orderList)
                {
                    if (order.IsAggregateOrder)
                    {
                        // Only add to Aggregated orders that are not yet approved and not aggregated themselves
                        if (order.Status == OrderStati.New && order.parentOrder == null)
                        {
                            OrderSearchKey SearchKey = new OrderSearchKey(order, netting);
                            if (!aggregatedOrders.ContainsKey(SearchKey))
                            {
                                aggregatedOrders.Add(SearchKey, order);
                                aggregatedOrderKeys.Add(SearchKey);
                                // how many children?
                                if (netting)
                                {
                                    SearchKey.Count = order.ChildOrders.Count;
                                }
                            }
                            else
                            {
                                //TODO -> Delete aggregate order and orphan the children
                            }
                        }
                        else
                            wrongStatusCount++;
                    }
                    else
                    {
                        childCount++;
                    }
                }

                if (childCount > 0)
                {
                    err = string.Format("{0} child orders were found.", childCount.ToString());
                }

                if (wrongStatusCount > 0)
                {
                    err = err + (err != "" ? "\n" : "") + string.Format("{0} child orders were found.", childCount.ToString());

                }
                if (err != "")
                {
                    throw new ApplicationException("Can not do aggregation:/n" + err);
                }
            }
        }

        private static OrderSearchKey addToList(Order newOrder, IOrderRouteMapper routeMapper, bool aggrOnStg, bool netting)
        {
            int index;
            OrderSearchKey SearchKey = new OrderSearchKey(newOrder, netting);
            if (!getKeyFromAggregatedOrders(ref SearchKey))
            {
                if (!(newOrder.parentOrder == null))
                {
                    aggregatedOrders.Add(SearchKey, newOrder.parentOrder);
                }
                else
                {
                    switch (newOrder.OrderType)
                    {
                        case OrderTypes.Monetary:
                            if (aggrOnStg)
                            {
                                if (netting)
                                    aggregatedOrders.Add(SearchKey, new StgMonetaryOrder((IStgMonetaryOrder)newOrder));
                                else
                                    aggregatedOrders.Add(SearchKey, new StgMonetaryOrder(getClearingAccount(newOrder), (IAggregateMonetaryOrder)newOrder, routeMapper));
                            }
                            else
                                aggregatedOrders.Add(SearchKey, new AggregateMonetaryOrder(getClearingAccount(newOrder), (IMonetaryOrder)newOrder));
                            break;
                        case OrderTypes.SizeBased:
                            if (aggrOnStg)
                            {
                                if (netting)
                                    aggregatedOrders.Add(SearchKey, new StgSizeOrder((IStgSizeOrder)newOrder));
                                else
                                    aggregatedOrders.Add(SearchKey, new StgSizeOrder(getClearingAccount(newOrder), (IAggregateSizeOrder)newOrder, routeMapper));
                            }
                            else
                                aggregatedOrders.Add(SearchKey, new AggregateSizeOrder(getClearingAccount(newOrder), (IOrderSizeBased)newOrder));
                            break;
                        default:
                            //OrderTypes.AmountBased:
                            if (aggrOnStg)
                            {
                                if (netting)
                                    aggregatedOrders.Add(SearchKey, new StgAmtOrder((IStgAmtOrder)newOrder));
                                else
                                    aggregatedOrders.Add(SearchKey, new StgAmtOrder(getClearingAccount(newOrder), (IAggregateAmtOrder)newOrder, routeMapper));
                            }
                            else
                                aggregatedOrders.Add(SearchKey, new AggregateAmtOrder(getClearingAccount(newOrder), (IOrderAmountBased)newOrder));
                            break;
                    }
                }
                aggregatedOrderKeys.Add(SearchKey);
            }
            else
            {
                // return same Key -> stored in collection
                int i = aggregatedOrderKeys.IndexOf(SearchKey);
                aggregatedOrderKeys[i] = SearchKey;
            }
            return SearchKey;
        }

        private static bool getKeyFromAggregatedOrders(ref OrderSearchKey searchKey)
        {
            bool success = false;
            foreach (var item in aggregatedOrders)
            {
                if (item.Key.Equals(searchKey))
                {
                    searchKey = item.Key;
                    success = true;
                    break;
                }
            }
            return success;
        }

        private static ITradingAccount getClearingAccount(IOrder order)
        {
            return order.Account.AccountforAggregation;
        }


        #endregion

        #region Netting

        public static IList<IOrder> Nett(IList<IOrder> nettedUnApprovedOrders, IList<IOrder> notNettedStgOrders, out string message)
        {
            aggregatedOrders = new Dictionary<OrderSearchKey, IOrder>();
            aggregatedOrderKeys = new List<OrderSearchKey>();
            IList<IOrder> result = new List<IOrder>();
            OrderSearchKey SearchKey = null;
            int count = 0;
            message = "";

            if (notNettedStgOrders == null || notNettedStgOrders.Count == 0)
                return null;

            // Check for the status of the notNettedStgOrders (Approved but not sent)
            checkUnNettedApprovedOrders(notNettedStgOrders);

            // Check if the nettOrders are ok (status)
            checkAggregateOrders(nettedUnApprovedOrders, true);

            foreach (Order newOrder in notNettedStgOrders)
            {
                if (newOrder.parentOrder == null)
                {
                    // Check the Instrument of the order allows Netting
                    if (checkInstrumentAllowsNetting(newOrder))
                    {
                        //Set the PlacedValue back -> Rounding on the Netted Order
                        if (newOrder.IsSizeBased && !newOrder.IsMonetary)
                            ((IStgSizeOrder)newOrder).ResetPlacedValue();
                        SearchKey = addToList(newOrder, null, true, true);
                        ((IOrder)aggregatedOrders[SearchKey]).ChildOrders.Add(newOrder);
                        SearchKey.Updated = true;
                        SearchKey.Count++;
                    }
                    else
                        count++;
                }
            }

            if (count > 0)
                message = string.Format("{0} orders could not be netted since the instrument does not allow inhouse-netting.", count.ToString());
            //saveAggregatedOrders();

            IStgOrder orderToSave;
            for (int i = 0; i < aggregatedOrderKeys.Count; i++)
            {
                OrderSearchKey key = (OrderSearchKey)aggregatedOrderKeys[i];
                if (key.Updated && key.Count > 1)
                {
                    orderToSave = (IStgOrder)aggregatedOrders[key];
                    adjustValueNettedOrderWithServiceCharge((IStgOrder)orderToSave);
                    ((IOrder)orderToSave).Validate();
                    foreach (IOrder child in ((IOrder)orderToSave).ChildOrders)
                    {
                        child.Approve();
                    }

                    result.Add((IOrder)orderToSave);
                }
            }
            return result;
        }

        private static bool checkInstrumentAllowsNetting(IOrder order)
        {
            IInstrument instrument;

            // Check if the Instrument allows Netting
            instrument = order.RequestedInstrument;
            if (instrument.IsTradeable)
                return ((ITradeableInstrument)instrument).AllowNetting;
            else
                return true;
        }

        private static bool checkUnNettedApprovedOrders(IList<IOrder> orders)
        {
            int countWrongStatus = 0;
            int countOrderNotAggr = 0;
            bool retVal = false;
            ArrayList ordersToRemove = new ArrayList();

            // Check for the status of the unNettedApprovedOrders (Approved but not sent)
            foreach (IOrder order in orders)
            {
                if (order.Status >= OrderStati.Routed)
                    countWrongStatus++;
                if (!(order.IsStgOrder))
                    countOrderNotAggr++;
                // Remove the orders that are nett orders themselve
                if (order.IsNetted)
                {
                    ordersToRemove.Add(order);
                }
            }
            if (countWrongStatus > 0)
                throw new ApplicationException(string.Format("The orders can not be netted since {0} orders have a wrong status", countWrongStatus.ToString()));
            else if (countOrderNotAggr > 0)
                throw new ApplicationException(string.Format("The orders can not be netted some of the orders ({0}) were not aggregated on stichting level", countOrderNotAggr.ToString()));
            else
                retVal = true;

            // Remove the orders that are nett orders themselve
            foreach (Order order in ordersToRemove)
            {
                orders.Remove(order);
            }

            return retVal;
        }

        private static void adjustValueNettedOrderWithServiceCharge(IStgOrder order)
        {
            // only applicable for stichting amount based orders
            if (((IOrder)order).OrderType == OrderTypes.AmountBased)
            {
                StgAmtOrder o = (StgAmtOrder)order;
                // do only when service charge will be charged
                if (o.ServiceChargeForBuy != null || o.ServiceChargeForSell != null)
                {
                    Money nettServCharge = o.ServiceChargeForBuy + o.ServiceChargeForSell;
                    if (nettServCharge != null && nettServCharge.IsNotZero)
                    {
                        // Nett the Buy & Sell side
                        InstrumentSize newOrderValue = o.ValueToBuy + o.ValueToSell;
                        // Subtract the Service Charge -> Nett Value
                        newOrderValue += nettServCharge;

                        // Calculate the service Charge on the Nett Value
                        IInstrumentExchange ie = o.getInstrumentExchange(true);
                        if (ie != null && ie.GetServiceChargePercentageForOrder(o) > 0)
                        {
                            // Calculate the Service Charge & add it to the nett Value
                            Money serviceCharge = (newOrderValue.Abs().GetMoney() * ie.GetServiceChargePercentageForOrder(o) * -1);
                            o.ServiceCharge = serviceCharge;
                            newOrderValue -= serviceCharge;
                        }
                        // Set The Order Value to the new calculated value
                        o.Value = newOrderValue;
                    }
                }
            }
        }

        #endregion

        #region Privates

        private static Dictionary<OrderSearchKey, IOrder> aggregatedOrders;
        private static List<OrderSearchKey> aggregatedOrderKeys;

        #endregion

        private class OrderSearchKey
        {
            public OrderSearchKey(IOrder newOrder)
                : this(newOrder, true)
            {
            }

            public OrderSearchKey(IOrder newOrder, bool netting)
            {
                OrderType = newOrder.OrderType;
                InstrumentKey = newOrder.RequestedInstrument.Key;
                OrderCurrencyKey = newOrder.OrderCurrency.Key;
                if (!netting)
                    Side = newOrder.Side;
            }

            public OrderTypes OrderType { get; set; }
            public int InstrumentKey { get; set; }
            public Side Side { get; set; }
            public int OrderCurrencyKey { get; set; }
            public bool Updated { get; set; }
            public int Count { get; set; }

            public override int GetHashCode()
            {
                return InstrumentKey;
            }

            public static bool operator ==(OrderSearchKey lhs, OrderSearchKey rhs)
            {
                return lhs.Equals(rhs);

            }
            public static bool operator !=(OrderSearchKey lhs, OrderSearchKey rhs)
            {
                return !(lhs == rhs);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is OrderSearchKey))
                    return false;
                else
                {
                    OrderSearchKey key = (OrderSearchKey)obj;
                    return ((Side == key.Side) && (InstrumentKey == key.InstrumentKey) && (OrderType == key.OrderType) && (OrderCurrencyKey == key.OrderCurrencyKey));
                }
            }
        }

        internal abstract InstrumentSize fillOrderValue(InstrumentSize size, Money value, Money serviceCharge, Money accruedInterest);


        //internal enum OrderSearchKeyOrderTypes
        //{
        //    SecurityAmountBased,
        //    SecuritySizeBased,
        //    MonetaryAmountBased,
        //    MonetarySizeBased
        //}

        #region IEnumerable Members

        public virtual IEnumerator GetEnumerator()
        {
            return Transactions.GetEnumerator();
        }

        #endregion
    }
}
