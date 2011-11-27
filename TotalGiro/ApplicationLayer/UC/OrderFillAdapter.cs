using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments.Prices;
using System.Data;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    [Flags]
    public enum PrefillCheckReturnValues
    {
        OK = 0x0002,
        AskCompleteFill = 0x0004,
        Warning = 0x0008
    }
    
    public class OrderFillAdapter
    {
        public static PrefillCheckReturnValues CheckCompleteFill(OrderFillView orderFillView)
        {
            PrefillCheckReturnValues retVal = PrefillCheckReturnValues.OK;
            Price price;
            InstrumentSize size;
            Money amount;
            InstrumentSize serviceCharge = null;
            Money accruedInterest = null;
            IInstrument tradedInstrument;
            InstrumentSize diff;
            decimal percLeft;
            orderFillView.Warning = "";

            IDalSession session = NHSessionFactory.CreateSession();
            IOrder order = OrderMapper.GetOrder(session, orderFillView.OrderId);

            if (order.IsMonetary)
            {
                tradedInstrument = (IInstrument)((IMonetaryOrder)order).RequestedCurrency;
                if (order.IsSizeBased)
                {
                    price = new Price(orderFillView.Price, (ICurrency)tradedInstrument, order.Value.Underlying);
                    size = new InstrumentSize(orderFillView.Size, order.Value.Underlying);
                    amount = new Money(orderFillView.Amount, (ICurrency)tradedInstrument);
                }
                else
                {
                    price = new Price(orderFillView.Price, (ICurrency)order.Value.Underlying, tradedInstrument);
                    amount = new Money(orderFillView.Amount, (ICurrency)order.Value.Underlying);
                    size = new InstrumentSize(orderFillView.Size, tradedInstrument);
                }
            }
            else
            {
                tradedInstrument = (IInstrument)((ISecurityOrder)order).TradedInstrument;
                size = new InstrumentSize(orderFillView.Size, tradedInstrument);

                if (!order.IsSizeBased)
                {
                    // Exchange rate (in base currency)
                    price = new Price(orderFillView.Price, ((ITradeableInstrument)tradedInstrument).CurrencyNominal, tradedInstrument);
                    amount = new Money(orderFillView.Amount, (ICurrency)order.Value.Underlying);
                    serviceCharge = new InstrumentSize(orderFillView.ServiceChargeAmount, order.Value.Underlying);
                }
                else
                {
                    ICurrency currency = (ICurrency)((ITradeableInstrument)tradedInstrument).CurrencyNominal;
                    price = new Price(orderFillView.Price, currency, tradedInstrument);
                    if (currency.IsObsoleteCurrency)
                        currency = currency.ParentInstrument as ICurrency;
                    amount = new Money(orderFillView.Amount, currency);
                    serviceCharge = new InstrumentSize(orderFillView.ServiceChargeAmount, currency);
                }
            }
            if (orderFillView.AccruedInterestAmount != 0M)
                accruedInterest = new Money(orderFillView.AccruedInterestAmount * (decimal)order.Side * -1M, amount.Underlying.ToCurrency);

            Order.CheckMaximalRoundOffError(order.IsSizeBased, size, amount, price, accruedInterest, order.Side);
            decimal contractSize = ((ITradeableInstrument)(order.RequestedInstrument)).ContractSize;

            InstrumentSize calcAmt = size.CalculateAmount(price) * (decimal)order.Side * -1M;
            //if (order.IsAmountBased && accruedInterest != null && accruedInterest.IsNotZero)
            //    calcAmt += accruedInterest;

            diff = (calcAmt.Abs() - amount.Abs());
            if (diff.IsNotZero && !diff.IsWithinTolerance(0.02M))
            {
                orderFillView.Warning = string.Format("Price times Size ({0}) does not equal the provided Amount ({1}).",
                                                      calcAmt.DisplayString, amount.DisplayString);

                retVal = PrefillCheckReturnValues.Warning;
                percLeft = diff.Quantity / amount.Abs().Quantity;
                orderFillView.FillPercentage = 1M - percLeft;
            }

            if (order.Transactions.Count == 0)
            {
                if (order.IsAmountBased)
                {
                    diff = order.PlacedValue.Abs() - amount.Abs();
                    if (serviceCharge != null && serviceCharge.IsNotZero)
                        diff -= serviceCharge.Abs();
                    //if (accruedInterest != null)
                    //    diff += accruedInterest;
                }
                else
                    diff = order.PlacedValue.Abs() - size.Abs();

                if (diff.IsNotZero && !diff.IsWithinTolerance(0.02M))
                {
                    percLeft = diff.Quantity / order.PlacedValue.Abs().Quantity;
                    orderFillView.FillPercentage = 1M - percLeft;

                    orderFillView.Warning += string.Format("{0}The order is {1} filled.", 
                                                (orderFillView.Warning != string.Empty ? "\n" : ""), orderFillView.DisplayFillPercentage);
                    retVal = PrefillCheckReturnValues.Warning;

                    if (percLeft <= 0.05m)
                    {
                        orderFillView.Warning += " If you want this order to be completely filled, check 'Complete Fill' on.";
                        retVal |= PrefillCheckReturnValues.AskCompleteFill;
                    }
                }

                if (order.IsAmountBased && orderFillView.TickSize > 0M)
                {
                    decimal factor = size.Quantity / orderFillView.TickSize;
                    if (Math.Abs(factor - (decimal)Convert.ToInt32(factor)) > 0.0001M)
                        orderFillView.Warning += string.Format("{0}The size is not in multiples of the tick size {1}.",
                                                    (orderFillView.Warning != string.Empty ? "\n" : ""), orderFillView.TickSize);
                    if (retVal == PrefillCheckReturnValues.OK)
                        retVal = PrefillCheckReturnValues.Warning;
                }
            }

            if ((retVal & PrefillCheckReturnValues.Warning) == PrefillCheckReturnValues.Warning)
                orderFillView.Warning += "\nPress OK again to fill the order.";
            
            return retVal;
        }
        
        public static DataSet GetCounterparties(bool nostro)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                List<IAccount> accounts = new List<IAccount>();
                if (nostro)
                {
                    accounts.AddRange(AccountMapper.GetAccounts<IAccount>(session, AccountTypes.Crumble));
                    accounts.AddRange(AccountMapper.GetAccounts<IAccount>(session, AccountTypes.Nostro));
                }
                else
                    accounts.AddRange(AccountMapper.GetAccounts<IAccount>(session, AccountTypes.Counterparty));

                return accounts
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayNumberWithName
                    })
                    .ToDataSet();
            }
        }
        
        public static decimal CalculateServiceChargeAmount(decimal orderAmount, decimal percentage)
        {
            decimal servCharge = 0;

            if (orderAmount != 0 && percentage != 0)
                servCharge = Math.Abs(orderAmount * percentage);

            return servCharge;
        }

        public static decimal CalculateServiceChargePercentage(decimal orderAmount, decimal servChargeAmount)
        {
            decimal percentage = 0;

            if (orderAmount != 0 && servChargeAmount > 0)
                percentage = Math.Abs(servChargeAmount / orderAmount);

            return percentage;
        }

        public static decimal CalculateAccruedInterestAmount(int orderId, decimal size, DateTime settlementDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                decimal accruedInterest = 0;
                IStgOrder order = (IStgOrder)OrderMapper.GetOrder(session, orderId);
                if (order != null && order.RequestedInstrument.SecCategory.Key == SecCategories.Bond)
                {
                    IBond bond = (IBond)order.RequestedInstrument;
                    if (bond.DoesPayInterest)
                    {
                        if (order.IsSizeBased)
                        {
                            InstrumentSize volume = new InstrumentSize(size, bond);
                            AccruedInterestDetails calc = bond.AccruedInterest(volume, settlementDate, null);
                            if (calc.IsRelevant)
                                accruedInterest = calc.AccruedInterest.Quantity;
                        }
                    }
                }
                return accruedInterest;
            }
        }
    }
}
