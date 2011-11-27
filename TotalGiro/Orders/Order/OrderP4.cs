using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Fees.CommRules;


namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Abstract class describing the general functionality for an order object
    /// </summary>
    abstract public partial class Order : IOrder
    {


        public IOrderAllocation FillasAllocation(IOrderExecution ParentExecution, ITradingJournalEntry tradingJournalEntry, IGLLookupRecords lookups, IFeeFactory feeFactory)
        {
            if (!RequestedInstrument.IsTradeable)
                throw new Exception("FillasAllocation not possible when the instrument is not tradeable.");

            ITradeableInstrument instrument = (ITradeableInstrument)RequestedInstrument;
            IExchange exchange = ParentExecution.Exchange ?? instrument.DefaultExchange ?? instrument.HomeExchange;
            TransactionFillDetails details = instrument.GetTransactionFillDetails(
                this, ParentExecution.Price, ParentExecution.ContractualSettlementDate, 
                feeFactory, ParentExecution.FillRatio, exchange);

            // convert servicecharge?
            if (details.ServiceCharge != null && details.ServiceCharge.IsNotZero)
            {
                if (!details.ServiceCharge.Underlying.Equals(ParentExecution.Price.Underlying) && !ParentExecution.Price.Underlying.IsObsoleteCurrency)
                    details.ServiceCharge = details.ServiceCharge.Convert(ExRate, ParentExecution.Price.Underlying);
            }

            decimal exRate = (ExRate != 0 ? ExRate : ParentExecution.ExchangeRate);
            if (IsMonetary)
                exRate = ParentExecution.ExchangeRate;

            ListOfTransactionComponents[] components = packageComponents(details.Amount, details.ServiceCharge, details.AccruedInterest);

            OrderAllocation newTrade = new OrderAllocation(
                this, this.account, ParentExecution.AccountA, details.Size,
                ParentExecution.Price, exRate, ParentExecution.TransactionDate,
                ParentExecution.TransactionDateTime, details.ServiceChargePercentage,
                this.Side, feeFactory, tradingJournalEntry, lookups, components);
            newTrade.Exchange = ParentExecution.Exchange;
            newTrade.ContractualSettlementDate = ParentExecution.ContractualSettlementDate;

            if (details.Commission == null)
                instrument.CalculateCosts(newTrade, feeFactory, lookups);
            else
                newTrade.setCommission(lookups, details.Commission);

            fillOrder(newTrade, details.Size, ParentExecution.Price, details.Amount, details.ServiceCharge, details.AccruedInterest);
            ParentExecution.Allocations.AddAllocation(newTrade);
            return newTrade;
        }

        
        public IOrderExecution Fill(InstrumentSize size, Price price, Money amount, decimal exRate,
            IAccount counterParty, DateTime transactionDate, DateTime transactionDateTime, IExchange exchange,
            bool isCompleteFill, Money serviceCharge, decimal serviceChargePercentage, Money accruedInterest,
            ITradingJournalEntry tradingJournalEntry, IGLLookupRecords lookups)
        {
            IOrderExecution returnValue = null;

            if (size == null || price == null || amount == null)
                throw new Exception("Size, Price and Amount are mandatory when filling the order.");

            if (IsFillable != OrderFillability.True)
                throw new ApplicationException("This Order is not fillable.");

            setSignServiceCharge(ref serviceCharge, ref serviceChargePercentage);
            if (accruedInterest != null && accruedInterest.IsNotZero)
                accruedInterest = accruedInterest.Abs() * (decimal)this.Side * -1M;
            CheckMaximalRoundOffError(IsSizeBased, size, amount, price, accruedInterest, this.Side);

            if (isCompleteFill)
            {
                // Check whether the order has not been filled before
                if (Transactions.Count > 0)
                    throw new ApplicationException("The order has already been filled and so it can not be a complete fill.");

                if (this.OpenValue.IsZero)
                    throw new ApplicationException("The open value of this order is zero.");

                InstrumentSize diff = getTradeDifferenceOpenValue(size, amount, serviceCharge, accruedInterest);
                if (diff.IsNotZero)
                {
                    const decimal maxAllowed = 0.05m;
                    // TickSize && NominalValue
                    decimal tickSize = 0M;
                    if (this.OrderType == OrderTypes.SizeBased)
                    {
                        IInstrumentExchange ie = getInstrumentExchange(exchange);
                        if (ie != null)
                            tickSize = ie.TickSize;
                    }
                    if (this.RequestedInstrument.SecCategory.Key == SecCategories.Bond)
                        tickSize = ((IBond)this.RequestedInstrument).NominalValue.Quantity;

                    if (!(tickSize > 0M && Math.Abs(diff.Quantity / tickSize) < 1M))
                    {
                        decimal fillRatio = diff.Abs().Quantity / this.OpenValue.Abs().Quantity;
                        if (fillRatio > maxAllowed)
                            throw new ApplicationException(
                                string.Format("The size of this transaction is different from the order size by more than {0:0.##}%.", maxAllowed * 100));
                    }
                }
                amount.XRate = exRate;
            }
            else
            {
                orderInitialCheck(ref amount, size, exRate, serviceCharge, accruedInterest);
            }

            this.IsCompleteFilled = isCompleteFill;
            orderCheckSide(this.Side, ref amount, ref size);
            checkServiceChargePercentage(serviceCharge, serviceChargePercentage, amount);

            ListOfTransactionComponents[] components = packageComponents(amount, serviceCharge, accruedInterest);

            returnValue = new OrderExecution(this, counterParty, size, price, exRate, transactionDate, transactionDateTime, serviceChargePercentage, tradingJournalEntry, lookups, components);
            fillOrder(returnValue, size, price, amount, serviceCharge, accruedInterest);
            return returnValue;
        }

        private ListOfTransactionComponents[] packageComponents(Money CValue, Money serviceCharge, Money accruedInterest)
        {
            ListOfTransactionComponents[] components = new ListOfTransactionComponents[3];
            components[0] = new ListOfTransactionComponents() { ComponentType = BookingComponentTypes.CValue, ComponentValue = CValue };
            components[1] = new ListOfTransactionComponents() { ComponentType = BookingComponentTypes.ServiceCharge, ComponentValue = serviceCharge };
            components[2] = new ListOfTransactionComponents() { ComponentType = BookingComponentTypes.AccruedInterest, ComponentValue = accruedInterest };
            return components;

        }




        /// <summary>
        /// Fills an order by setting the agreed size and price, exchange rate, counter party and transaction date.
        /// </summary>
        /// <param name="trade">The order to be filled</param>
        /// <param name="size">Size of the instrument</param>
        /// <param name="price">Agreed price of the instrument</param>
        /// <param name="amount">Amount of the order (=size * price)</param>
        /// <returns>True if it succeeded</returns>
        protected bool fillOrder(ITransactionOrder trade, InstrumentSize size, Price price, Money amount, Money serviceCharge, Money accruedInterest)
        {
            decimal ratio = 1m;
            Transactions.AddTransactionOrder(trade);
            FilledValue += fillOrderValue(size, amount, serviceCharge, accruedInterest);
            if (this.IsSecurity)
            {
                ((ISecurityOrder)this).ServiceCharge += serviceCharge;
                if (this.IsAmountBased)
                    ((ISecurityOrder)this).AccruedInterest += accruedInterest;
            }

            // If OrderExecution -> set FillRatio
            if (trade.TransactionType == TransactionTypes.Execution)
            {
                if (IsCompleteFilled)
                    // if order is already partly filled -> subtract rest
                    ratio = 1M - Transactions.TotalFillRatio();
                else
                {
                    InstrumentSize diff = PlacedValue.Abs() - fillOrderValue(size, amount, serviceCharge, accruedInterest).Abs();
                    if (diff.IsZero || diff.IsWithinTolerance(0.02M))
                        ratio = 1M;
                    else
                    {
                        ratio = fillOrderValue(size, amount, serviceCharge, accruedInterest).Abs().Quantity / PlacedValue.Abs().Quantity;
                        // If this trade filled the order (OpenSize = 0) -> take the remainder ratio
                        if (OpenValue.IsZero)
                        {
                            decimal var = ratio - (1M - getFillatioTransactions());
                            if (Math.Abs(var) < 0.0001M)
                                ratio = 1M - getFillatioTransactions();
                        }
                    }
                }
            }
            trade.FillRatio = ratio;
            OrderStateMachine.SetNewStatus(this, OrderStateEvents.Fill);

            //// If MoneyOrder on order -> set ExRate
            //if (IsMonetary)
            //{
            //    // Use the size as the amount -> because it is a conversion
            //    processMoneyFill(price, trade);
            //}

            return true;
        }

        private void setSignServiceCharge(ref Money serviceCharge, ref decimal serviceChargePercentage)
        {
            // Set the sign of the ServiceCharge
            if (serviceCharge != null && serviceCharge.IsNotZero)
                serviceCharge = serviceCharge.Abs() * -1;
            serviceChargePercentage = Math.Abs(serviceChargePercentage);
        }

        private void checkServiceChargePercentage(Money serviceCharge, decimal serviceChargePercentage, Money amount)
        {
            // Check the percentage of the ServiceCharge
            if (amount.IsNotZero && ((serviceCharge != null && serviceCharge.IsNotZero) || serviceChargePercentage != 0))
            {
                decimal presumedServiceChargeAmount = Math.Round(amount.Quantity * serviceChargePercentage, 2);
                if (!Util.IsWithinTolerance(serviceCharge.Quantity, presumedServiceChargeAmount, 0.01m))
                    throw new ApplicationException("The service charge percentage doesn't match the supplied service charge.");
            }
        }

        private void processMoneyFill(Price price, ITransactionOrder newTrade)
        {
            //// If MoneyOrder on order -> set ExRate
            //if (IsMonetary)
            //{
            //    IStgMonetaryOrder mo = null;
            //    if (IsStgOrder && IsMonetary)
            //        mo = this as IStgMonetaryOrder;

            //    if (mo != null && mo.MoneyParent != null)
            //    {
            //        // First check if already exist
            //        if (mo.MoneyParent.ParentOrder != null)
            //            throw new ApplicationException("Monetary orders can only be filled once.");

            //        decimal exRate = Math.Round(1 / price.Quantity, 6);
            //        Money amount = mo.MoneyParent.Amount.Convert(exRate, (ICurrency)price.Instrument);
            //        StgAmtOrder newParent = new StgAmtOrder((IStgAmtOrder)mo.MoneyParent, amount, exRate);
            //        mo.MoneyParent.SetParentOrder(newParent);
            //        newTrade.ConvertedOrder = newParent;
            //        ((IOrder)mo.MoneyParent).SetExRate(exRate);

            //        //newParent.Approve();
            //    }
            //}
        }

        protected bool orderCheckSide(Side side, ref Money amount, ref InstrumentSize size)
        {
            if (side == Side.Buy)
            {
                size = size.Abs();
                amount = amount.Abs() * -1;
            }
            else
            {
                size = size.Abs() * -1;
                amount = amount.Abs();
            }
            return true;
        }

        /// <summary>
        /// Allocates the (aggregated) transaction to the children of the order.
        /// </summary>
        /// <param name="ExecutedTrade">Order execution information</param>
        /// <param name="feeFactory">feeFactory containing rules for calculating costs</param>
        /// <returns></returns>
        //public bool Allocate(ITGOrderExecution ExecutedTrade, IFeeFactory feeFactory)
        //{
        //    //if (ExecutedTrade != null)
        //    //{
        //    //    IOrderExecution executedTrade = null;
        //    //    if (ExecutedTrade.IsOrderExecution)
        //    //        executedTrade = ExecutedTrade as IOrderExecution;

        //    //    if (executedTrade != null && (!executedTrade.IsAllocated) && (executedTrade.Approved == true))
        //    //    {
        //    //        if (executedTrade.IsAllocateable)
        //    //        {
        //    //            //throw new ApplicationException(string.Format("Trade {0} is not allocateable", executedTrade.ToString()));
        //    //            //if (this.allocate(executedTrade.ValueSize, executedTrade.Price, executedTrade.ExchangeRate, ExecutedTrade.AccountB,
        //    //            //    executedTrade.TransactionDate, executedTrade, null))

        //    //            if (GetAllocateableOrder().allocateTransaction(executedTrade, null, feeFactory, getInstrumentExchange(executedTrade.Exchange)))
        //    //            {
        //    //                //executedTrade.IsAllocated = true;
        //    //                this.AllocationDate = DateTime.Now;
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    return true;
        //}

        /// <summary>
        /// This order through a IAllocatable interface
        /// </summary>
        /// <returns>This order through a IAllocatable interface</returns>
        //public IAllocateableOrder GetAllocateableOrder()
        //{
        //    return this as IAllocateableOrder;
        //}

        #region IAllocateableOrder Members

        //bool IAllocateableOrder.allocateTransaction(ITGOrderExecution executedTrade, ITGOrderAllocation parentAllocation, IFeeFactory feeFactory, IInstrumentExchange ie)
        //{
        //    //ITGOrderAllocation returnValue = null;
        //    //InstrumentSize totalAllocatedSize = null;
        //    //Money totalAllocatedAmount = null;
        //    //bool allocated = false;

        //    //foreach (IOrder childOrder in this.ChildOrders)
        //    //{
        //    //    returnValue = childOrder.GetAllocateableOrder().allocateToChild(executedTrade, parentAllocation, feeFactory, ie);
        //    //    childOrder.fillOrder(returnValue, returnValue.ValueSize, returnValue.Price, returnValue.CounterValueSize, returnValue.ServiceCharge);
        //    //    returnValue.Approve();
        //    //    childOrder.allocateTransaction(executedTrade, returnValue, feeFactory, ie);
        //    //    totalAllocatedSize += returnValue.ValueSize;
        //    //    totalAllocatedAmount += returnValue.CounterValueSize;
        //    //    allocated = true;
        //    //}

        //    //if (allocated)
        //    //{
        //    //    //Which Transaction to Use?
        //    //    ITransactionOrder transToAllocate = (parentAllocation == null) ? (ITransactionOrder)executedTrade : (ITransactionOrder)parentAllocation;
        //    //    transToAllocate.IsAllocated = true;

        //    //    // Crumble stuff
        //    //    InstrumentSize diffSize = transToAllocate.ValueSize - totalAllocatedSize;
        //    //    //Money diffAmount = transToAllocate.CounterValueSize - totalAllocatedAmount;

        //    //    //if (diffSize.IsNotZero || diffAmount.IsNotZero)
        //    //    //{
        //    //    //    ICrumbleAccount crumbleAcct = Account.AccountOwner.StichtingDetails.CrumbleAccount;
        //    //    //    CrumbleTransaction ct = new CrumbleTransaction(crumbleAcct, transToAllocate.AccountA, diffSize, diffAmount, transToAllocate.Price, transToAllocate.ExchangeRate, transToAllocate.TransactionDate, (diffSize.Sign ? Side.XI : Side.XO), executedTrade, TopParentOrder);
        //    //    //    transToAllocate.CrumbleTransactions.Add(ct);
        //    //    //    ct.Approve();
        //    //    //}

        //    //    // Do it the Constant way

        //    //    if (diffSize != null && diffSize.IsNotZero)
        //    //    {
        //    //        Money diffAmount = diffSize * transToAllocate.Price * -1M;
        //    //        ICrumbleAccount crumbleAcct = Account.AccountOwner.StichtingDetails.CrumbleAccount;
        //    //        CrumbleTransaction ct = new CrumbleTransaction(crumbleAcct, transToAllocate.AccountA, diffSize, diffAmount, transToAllocate.Price, transToAllocate.ExchangeRate, transToAllocate.TransactionDate, (diffSize.Sign ? Side.XI : Side.XO), executedTrade, TopParentOrder);
        //    //        transToAllocate.CrumbleTransactions.Add(ct);
        //    //        ct.Approve();
        //    //    }


        //    //}
        //    return true;
        //}

        //ITGOrderAllocation IAllocateableOrder.allocateToChild(ITGOrderExecution ExecutedTrade, ITGOrderAllocation ParentAllocation, IFeeFactory feeFactory, IInstrumentExchange ie)
        //{
        //    return new TGOrderAllocation();
        //    //InstrumentSize size;
        //    //Money amount;
        //    //Money serviceCharge = null;
        //    //decimal exRate;

        //    ////Which Transaction to Use?
        //    //ITransactionOrder transToAllocate = (ParentAllocation == null) ? (ITransactionOrder)ExecutedTrade : (ITransactionOrder)ParentAllocation;
        //    ////Get the ratio
        //    //IAccount counterParty = transToAllocate.AccountA;
        //    //DateTime transactionDate = transToAllocate.TransactionDate;
        //    //Price price = transToAllocate.Price;

        //    //// Determine ordertype
        //    //if (IsSizeBased)
        //    //{
        //    //    // Use the Value of the child order -> difference will go to Crumble account
        //    //    size = Value * transToAllocate.FillRatio;
        //    //    amount = (size * price);
        //    //    if (ie != null && ie.GetServiceChargePercentageForOrder(this) > 0)
        //    //    {
        //    //        serviceCharge = (amount * ie.GetServiceChargePercentageForOrder(this));
        //    //        serviceCharge = serviceCharge.Abs() * -1M;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    calculateTradeAmounts(Value.GetMoney(), transToAllocate.FillRatio, ie, out amount, out serviceCharge);
        //    //    if (!amount.Underlying.Equals(price.Underlying))
        //    //        amount = amount.Convert(ExRate, price.Underlying);

        //    //    size = amount / price;
        //    //}

        //    //// convert servicecharge?
        //    //if (serviceCharge != null && serviceCharge.IsNotZero)
        //    //{
        //    //    if (!serviceCharge.Underlying.Equals(price.Underlying))
        //    //        serviceCharge = serviceCharge.Convert(ExRate, price.Underlying);
        //    //}

        //    //orderCheckSide(this.side, ref amount, ref size);
        //    //if (IsMonetary)
        //    //    exRate = ExecutedTrade.ExchangeRate;
        //    //else
        //    //    exRate = (ExRate != 0 ? ExRate : ExecutedTrade.ExchangeRate);
        //    //OrderAllocation newTrade = new OrderAllocation(this, counterParty, size, amount, price, exRate,
        //    //    transactionDate, ExecutedTrade, ParentAllocation,
        //    //    feeFactory, serviceCharge);
        //    //newTrade.Exchange = transToAllocate.Exchange;
        //    //newTrade.ContractualSettlementDate = transToAllocate.ContractualSettlementDate;
        //    //newTrade.ServiceCharge = serviceCharge;
        //    //return (IOrderAllocation)newTrade;
        //}

        //protected bool calculateTradeAmounts(Money orderAmount, Money grossAmount, decimal fillRatio, 
        //    IExchange exchange, IBond bond, Price price, DateTime settlementDate, IFeeFactory feeFactory,
        //    out InstrumentSize size, out Money tradeAmount, out Money commission, out Money serviceCharge, 
        //    out decimal serviceChargePercentageforOrder, out Money accruedInterest)
        //{
        //    bool retVal = false;
        //    tradeAmount = null;
        //    serviceCharge = null;
        //    serviceChargePercentageforOrder = 0M;
        //    accruedInterest = null;
        //    commission = null;

        //    if (bond != null && bond.DoesPayInterest)
        //    {
        //        decimal realAmount;
        //        serviceChargePercentageforOrder = getServiceChargePercentageforOrder(exchange);
        //        ICommClient client;
        //        ICommRule rule = feeFactory.GetRelevantCommRule(Account, bond, Side,
        //            ActionType, settlementDate, true, out client);
        //        decimal estimate = grossAmount.CalculateSize(price).Quantity;
        //        decimal servChargePerc = serviceChargePercentageforOrder;

        //        if (rule.AdditionalCalculation != null)
        //            realAmount = FinancialMath.GoalSeek(x =>
        //                new InstrumentSize(x, bond).CalculateAmount(price).Quantity +
        //                rule.CommCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, bond), price, rule.AdditionalCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, bond), price)))).Quantity +
        //                rule.AdditionalCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, bond), price)).Quantity +
        //                bond.AccruedInterest(new InstrumentSize(x, bond), settlementDate, exchange).AccruedInterest.Quantity +
        //                (new InstrumentSize(x, bond).CalculateAmount(price).Quantity * servChargePerc),
        //                grossAmount.Quantity, estimate, 2);
        //        else
        //            realAmount = FinancialMath.GoalSeek(x =>
        //                    new InstrumentSize(x, bond).CalculateAmount(price).Quantity +
        //                    rule.CommCalculation.Calculate(client.GetNewInstance(new InstrumentSize(x, bond), price)).Quantity +
        //                    bond.AccruedInterest(new InstrumentSize(x, bond), settlementDate, exchange).AccruedInterest.Quantity +
        //                    (new InstrumentSize(x, bond).CalculateAmount(price).Quantity * servChargePerc),
        //                    grossAmount.Quantity, estimate, 2);


        //        size = MoneyMath.GetMinimumSizeForAmount(
        //            grossAmount, new InstrumentSize(realAmount, bond), price, bond, settlementDate, exchange,
        //            rule.CommCalculation, rule.AdditionalCalculation, client, servChargePerc);
        //        tradeAmount = size.CalculateAmount(price);
        //        accruedInterest = bond.AccruedInterest(size, settlementDate, exchange).AccruedInterest;
        //        serviceCharge = (tradeAmount * servChargePerc);
        //        Money addComm = tradeAmount.ZeroedAmount();
        //        if (rule.AdditionalCalculation != null)
        //            addComm = rule.AdditionalCalculation.Calculate(client.GetNewInstance(size, price));
        //        Money comm = rule.CommCalculation.Calculate(client.GetNewInstance(size, price, addComm));

        //        Money total = tradeAmount + accruedInterest + serviceCharge + addComm + comm;
        //        if (!total.Equals(grossAmount) && comm.IsNotZero)
        //        {
        //            Money diff = grossAmount - total;
        //            if (diff.Abs().Quantity > 0.01M)
        //                throw new ApplicationException("There is a bug in calculateTradeAmounts, contact your developer");
        //            comm += diff;
        //        }

        //        commission = (comm + addComm).Negate();
        //        retVal = true;
        //    }
        //    else
        //    {
        //        tradeAmount = orderAmount * fillRatio;
        //        // deduct ServiceCharge
        //        retVal = calculateServiceChargeForAmountBasedOrder(exchange, ref tradeAmount, out serviceCharge, out serviceChargePercentageforOrder);

        //        if (!tradeAmount.Underlying.Equals(price.Underlying) && !price.Underlying.IsObsoleteCurrency)
        //            tradeAmount = tradeAmount.Convert(ExRate, price.Underlying);

        //        size = tradeAmount.CalculateSize(price);
        //    }
        //    return retVal;
        //}

        /// <summary>
        /// We need to calculate the serviceCharge for Netting.
        /// On the netted order, take the nett amount (minus service charge) and then add the service charge
        /// </summary>
        protected decimal getServiceChargePercentageforOrder(IExchange exchange)
        {
            decimal percentage = 0m;
            IInstrumentExchange ie = null;

            if (exchange != null)
                ie = getInstrumentExchange(exchange);
            else
                ie = getInstrumentExchange(true);
            if (ie != null)
                percentage = ie.GetServiceChargePercentageForOrder(this);
            return percentage;
        }


        /// <summary>
        /// We need to calculate the serviceCharge for Netting.
        /// On the netted order, take the nett amount (minus service charge) and then add the service charge
        /// </summary>
        protected bool calculateServiceChargeForAmountBasedOrder(
            IExchange exchange, ref Money tradeAmount, 
            out Money serviceCharge, out decimal serviceChargePercentageforOrder)
        {
            bool retVal = false;
            serviceCharge = null;
            serviceChargePercentageforOrder = 0m;

            serviceChargePercentageforOrder = getServiceChargePercentageforOrder(exchange);

            // deduct ServiceCharge
            if (serviceChargePercentageforOrder > 0)
            {
                Money newAmount = tradeAmount * (decimal)(1M / (1M + serviceChargePercentageforOrder));
                serviceCharge = (tradeAmount.Abs() - newAmount.Abs()) * -1M;
                tradeAmount = newAmount;
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// We need to calculate the serviceCharge for Netting.
        /// On the netted order, take the nett amount (minus service charge) and then add the service charge
        /// </summary>
        internal void setServiceChargeForAmountBasedOrder()
        {
            if (!this.IsAmountBased)
                throw new ApplicationException("The method setServiceChargeForAmountBasedOrder can only be called for amount based orders");
            
            IOrderAmountBased o = (IOrderAmountBased)this;
            Money serviceCharge;
            Money amount = o.Amount;
            decimal serviceChargePercentageforOrder;
            if (calculateServiceChargeForAmountBasedOrder(null, ref amount, out serviceCharge, out serviceChargePercentageforOrder))
                o.ServiceCharge = serviceCharge;
        }

        //private Decimal GetParentFillRatio(ITransactionOrder trade)
        //{
        //    //// returns the percentage of filling of the order
        //    //// when completely filled it should return 1
        //    //decimal ratio;

        //    //decimal totalValue = this.ParentOrder.Value.Abs().Quantity;
        //    //if (!IsSizeBased && !(trade.CounterValueSize.Underlying.Equals(this.ParentOrder.Value.Underlying)))
        //    //{
        //    //    // Is this code used?
        //    //    decimal exRate = ExRate;
        //    //    if (exRate == 0)
        //    //    {
        //    //        ICurrency fromCur = (ICurrency)trade.CounterValueSize.Underlying;
        //    //        ICurrency toCur = (ICurrency)this.ParentOrder.Value.Underlying;
        //    //        exRate = fromCur.GetExRate(toCur, this.Side);
        //    //    }
        //    //    totalValue *= exRate;
        //    //}

        //    //if (IsSizeBased)
        //    //    ratio = trade.ValueSize.Abs().Quantity / totalValue;
        //    //else
        //    //    ratio = trade.CounterValueSize.Abs().Quantity / totalValue;

        //    //if (Math.Abs(1M - ratio) <= 0.05M)
        //        return 1m;
        //    //else
        //    //    return ratio;
        //}


        #endregion

     

        private decimal getFillatioTransactions()
        {
            decimal retVal = 0;
            if (Transactions != null && Transactions.Count > 0)
            {
                foreach (ITransactionOrder trade in Transactions)
                {
                    retVal += trade.FillRatio;
                }
            }
            return retVal;
        }

        public bool RemoveTransaction(ITransactionOrder trade)
        {
            bool retVal = false;

            if (Transactions.ContainsTrade(trade))
            {
                Transactions.RemoveTrade(trade);
                FilledValue = fillOrderValue(Transactions.TotalValueSize, Transactions.TotalCounterValueSize, Transactions.TotalServiceCharge, Transactions.TotalAccruedInterest);
                if (this.IsSecurity)
                {
                    ((ISecurityOrder)this).ServiceCharge = Transactions.TotalServiceCharge.IsZero ? null : Transactions.TotalServiceCharge;
                    if (this.IsAmountBased)
                        ((ISecurityOrder)this).AccruedInterest = Transactions.TotalAccruedInterest.IsZero ? null : Transactions.TotalAccruedInterest;
                }
                IsCompleteFilled = false;

                OrderStateMachine.ResetStatus(this);
                retVal = true;

                //// If MoneyOrder on order -> set ExRate
                //if (IsMonetary)
                //{
                //    // Use the size as the amount -> because it is a conversion
                //    processMoneyFill(price, trade);
                //}
            }
            else
                throw new Exception("Can not find the trade on the order");

            return retVal;
        }

        protected void orderInitialCheck(ref Money amount, InstrumentSize size, decimal exRate, Money serviceCharge, Money accruedInterest)
        {
            InstrumentSize diff = getTradeDifferenceOpenValue(size, amount, serviceCharge, accruedInterest);
            if (!diff.Sign)
            {
                if (diff < diff.Clone(-0.02m))
                    throw new ApplicationException("This size of the trade is larger than the order size.");
            }

            if (IsMonetary) // can not be partfilled
            {
                if (FilledValue != null)
                    throw new ApplicationException("Monetary orders can only be filled once.");
                if (!diff.IsWithinTolerance(0.02m))
                    throw new ApplicationException("Monetary orders can only be filled completely.");
            }
            else if (!(IsSizeBased)) // applies only to amount based orders
            {
                // If first fill -> check if size should not be equal to Order size
                if (FilledValue == null)
                {
                    if (diff.IsWithinTolerance(0.02m) && this.Amount != null)
                    {
                        amount = this.Amount;
                        // reduce it with servicecharge
                        amount = MoneyMath.AdjustAmountForServiceCharge(amount, serviceCharge, Side, MathOperator.Subtract);
                        // reduce it with accruedInterest
                        amount = MoneyMath.AdjustAmountForServiceCharge(amount, accruedInterest, Side, MathOperator.Subtract);
                    }
                }
            }

            amount.XRate = exRate;

            //bool approved = (route != null) ? route.ApproveTransactions : false;
            if (IsFillable != OrderFillability.True)
            {
                throw new ApplicationException("This Order is not fillable.");
            }
        }

        private InstrumentSize getTradeDifferenceOpenValue(InstrumentSize size, Money amount, Money serviceCharge, Money accruedInterest)
        {
            InstrumentSize diff;
            if (IsSizeBased)
                diff = (this.OpenValue.Abs() - size.Abs());
            else
                diff = (this.OpenValue.Abs() - amount.Abs() + serviceCharge + accruedInterest);
            return diff;
        }

        public static void CheckMaximalRoundOffError(bool isSizeBased, InstrumentSize size, Money amount, Price price, Money accruedInterest, Side side)
        {
            if (!isSizeBased)
            {
                InstrumentSize calcAmt = size.CalculateAmount(price) * (decimal)side * -1M;
                //if (accruedInterest != null && accruedInterest.IsNotZero)
                //{
                //    accruedInterest = accruedInterest.Abs() * (decimal)side * -1M;
                //    calcAmt += accruedInterest;
                //}

                InstrumentSize diff = (calcAmt.Abs() - amount.Abs());
                if (diff.IsNotZero && !diff.IsWithinTolerance(0.02m))
                {
                    decimal percLeft = diff.Abs().Quantity / amount.Abs().Quantity;
                    if (percLeft >= 0.05m)
                        throw new ApplicationException(string.Format(
                            "Price times Size ({0}) differs by {1}% from the provided Amount ({2}). Order cannot be filled.",
                            calcAmt.DisplayString, Math.Round(percLeft * 100m, 1), amount.DisplayString));
                }
            }
        }
    }
}
