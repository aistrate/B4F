using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transactions;
using System.Collections;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// Abstract class to hold implementation for security order, inherits from Order
    /// </summary>
	public abstract class SecurityOrder: Order, ISecurityOrder
	{
        protected SecurityOrder() { }

		protected SecurityOrder(IAccountTypeInternal account, InstrumentSize value, ITradeableInstrument tradedInstrument, bool doNotChargeCommission)
			: base(account, value)
        {
			if (tradedInstrument == null)
			{
				throw new ApplicationException("TradedInstrument is mandatory on an order.");
			}
			this.tradedInstrument = tradedInstrument;
            this.DoNotChargeCommission = doNotChargeCommission;
            if (DoNotChargeCommission)
                this.OrderInfo = "No commission charged";
		}

        /// <summary>
        /// Is it a security order (always true)
        /// </summary>
        public override bool IsSecurity
        {
            get { return true; }
        }

        /// <summary>
        /// Gets/sets the traded instrument
        /// </summary>
		public virtual ITradeableInstrument TradedInstrument
		{
			get { return tradedInstrument; }
			set { tradedInstrument = value; }
		}

        public Money ServiceCharge
        {
            get { return this.serviceCharge; }
            set { this.serviceCharge = value; }
        }

        /// <summary>
        /// Accrued Interest that needs to be paid/ is received
        /// For Amount based orders the accrued interest is in the amount
        /// </summary>
        public virtual Money AccruedInterest { get; set; }

        /// <summary>
        /// Gets a value indicating whether the order is sendable to Fund Settle.
        /// </summary>
        public bool IsFsSendable
        {
            get
            {
                return (Status == OrderStati.New);
            }
        }
        

        public INota CreateNota()
        {
            if (!IsAggregateOrder && !IsStgOrder)
            {
                if (Status == OrderStati.Checked)
                {
                    foreach (ITransactionOrder transaction in Transactions)
                    {
                        // this should be replaced when a Nota will be able to have more than one transaction
                        if (transaction.TransactionType == TransactionTypes.Allocation && 
                            transaction.Approved &&
                            !transaction.NotaMigrated && 
                            transaction.StornoTransaction == null)
                        {
                            if (transaction.TxNota == null)
                            {
                                Status = OrderStati.Terminated;
                                return new NotaTransaction((IOrderAllocation)transaction);
                            }
                            else
                                throw new ApplicationException(
                                                string.Format("Order {0} (transaction {1}) already has a nota ({2}).",
                                                              Key, transaction.Key, transaction.TxNota.Key));
                        }
                    }
                }

                return null;
            }
            else
                throw new ApplicationException("Only normal orders can have Notas created.");
        }

        public abstract ISecurityOrder Convert(B4F.TotalGiro.Instruments.Price price, B4F.TotalGiro.OrderRouteMapper.IOrderRouteMapper route);

        
		#region Private Variables

		private ITradeableInstrument tradedInstrument;
        private Money serviceCharge;
        
        #endregion
	}
}
