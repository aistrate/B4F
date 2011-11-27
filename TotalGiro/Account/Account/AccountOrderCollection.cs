using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments;
using System.Collections;

namespace B4F.TotalGiro.Accounts
{
    public class AccountOrderCollection : TransientDomainCollection<IOrder>, IAccountOrderCollection
    {
        public AccountOrderCollection() : base() { }

        /// <exclude/>
        internal AccountOrderCollection(IAccountTypeInternal parentAccount)
            : base()
        {
            this.ParentAccount = parentAccount;
        }


        /// <summary>
        /// The account the order collection belongs to
        /// </summary>
        public IAccountTypeInternal ParentAccount 
        {
            get { return this.parentAccount; }
            set
            {
                this.parentAccount = value;
                IsInitialized = true;
            }
        }

        public IAccountOrderCollection NewCollection(Func<IOrder, bool> criteria)
        {
            AccountOrderCollection returnValue = new AccountOrderCollection(this.ParentAccount);
            returnValue.AddRange(this.Where(criteria));
            return returnValue;
        }

        public IAccountOrderCollection NewCollection(List<Func<IOrder, bool>> criteria)
        {
            AccountOrderCollection returnValue = new AccountOrderCollection(this.ParentAccount);
            Func<IOrder, bool> finalPredicate = (c => criteria.TrueForAll(pred => pred(c)));
            returnValue.AddRange(this.Where(finalPredicate));
            return returnValue;
        }

        public IAccountOrderCollection Exclude(IList<IInstrument> excludedInstruments)
        {
            if (excludedInstruments == null || excludedInstruments.Count == 0)
                return this;

            Func<IOrder, bool> predicate = x => !excludedInstruments.Contains(x.RequestedInstrument);
            return NewCollection(predicate);
        }

        public IAccountOrderCollection Filter(IInstrument tradedInstrument, OrderSideFilter sideFilter)
        {
            Func<IOrder, bool> predicate = x => ((x.RequestedInstrument == tradedInstrument)
                                                &&  ((sideFilter == OrderSideFilter.All) ||
                                                    (sideFilter == OrderSideFilter.Buy && x.Side == Side.Buy) ||
                                                    (sideFilter == OrderSideFilter.Sell && x.Side == Side.Sell)));
            return NewCollection(predicate);
        }

        public IAccountOrderCollection Filter(OrderTypes orderType, OrderSideFilter sideFilter)
        {
            Func<IOrder, bool> predicate = x => ((x.OrderType.Equals(orderType))
                                    && ((sideFilter == OrderSideFilter.All) ||
                                        (sideFilter == OrderSideFilter.Buy && x.Side == Side.Buy) ||
                                        (sideFilter == OrderSideFilter.Sell && x.Side == Side.Sell)));
            return NewCollection(predicate);
        }

        public InstrumentSize TotalSize(IInstrument instrument)
        {
            // Returns the total ordered size of the requested instrument
            // If Amount Based Order -> we use predicted size
            InstrumentSize size = null;

            foreach (IOrder order in this)
            {
                if (order.RequestedInstrument.Equals(instrument))
                {
                    if (order.IsSizeBased)
                        size += order.Value;
                    else
                    {
                        // Amount based order -> predict the size with latest rate/price
                        PredictedSize predSize = order.RequestedInstrument.PredictSize(order.Amount);
                        if (predSize.Status != PredictedSizeReturnValue.NoRate)
                            size += predSize.Size;
                    }
                }
            }
            return size;
        }

        public Money TotalAmountInSpecifiedNominalCurrency(ICurrency currencyNominal)
        {
            //This method returns the total amount of orders that 
            //are in a instrument in the specified currency
            //It only returns the orders in tradeable instruments
            Money amount = null;

            foreach (IOrder order in this)
            {
                ITradeableInstrument instrument = order.RequestedInstrument as ITradeableInstrument;
                if (instrument != null && instrument.IsTradeable && instrument.CurrencyNominal.Equals(currencyNominal))
                {
                    if (order.Amount.Underlying.Equals(currencyNominal))
                        amount += order.GrossAmount;
                    else
                        amount += order.GrossAmount.Convert(currencyNominal);
                }

            }
            return amount;
        }

        public Money TotalAmount()
        {
            return this.Select(x => x.BaseAmount).Sum();
        }

        public Money TotalGrossAmount()
        {
            return this.Select(x => x.GrossAmountBase).Sum();
        }

        public Money TotalAmount(IInstrument instrument)
        {
            return TotalAmount(instrument, true);
        }

        public Money TotalAmount(IInstrument instrument, bool useRequestedInstrument)
        {
            Money amount = null;

            foreach (IOrder order in this)
            {
                if (useRequestedInstrument)
                {
                    if (order.RequestedInstrument.Equals(instrument))
                        amount += order.Amount;
                }
                else
                {
                    // Monetary only -> Value needs to be in foreign currency but req inst is system currency
                    if (order.Value.Underlying.Equals(instrument) && order.RequestedInstrument.Equals(ParentAccount.AccountOwner.StichtingDetails.BaseCurrency))
                        amount += order.Amount;
                }
            }
            return amount;
        }

        #region Private Variables

        private IAccountTypeInternal parentAccount;

        #endregion

    }
}
