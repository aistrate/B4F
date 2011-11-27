using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Notas
{
    public class NotaTransaction : NotaTransactionBase, INotaTransaction
    {
        #region Constructor

        internal NotaTransaction() { }

        public NotaTransaction(IOrderAllocation underlyingTx)
            : base(underlyingTx)
        {
        }

        #endregion

        #region Props

        public override string Title
        {
            get { return "Transactieoverzicht"; }
        }

        public virtual IOrder Order
        {
            get { return originalAllocation.Order; }
        }

        public virtual string ExchangeName
        {
            get 
            { 
                IExchange exchange = null;
                if (originalAllocation.ParentExecution != null)
                    exchange = originalAllocation.ParentExecution.Exchange;

                if (exchange != null)
                    return exchange.ExchangeName;
                else
                    return (TradedInstrument.DefaultExchange != null ? TradedInstrument.DefaultExchange.ExchangeName : ""); 
            }
        }

        public virtual DateTime TransactionDateTime
        {
            get
            {
                if (originalAllocation.ParentExecution != null)
                    return originalAllocation.ParentExecution.TransactionDateTime;
                else
                    return TransactionDate;
            }
        }

        public virtual decimal ServiceChargePercentage
        {
            get
            {
                decimal serviceChargePercentage = 0m;

                if (UnderlyingTx.ServiceCharge != null)
                {
                    if (TradedInstrument != null && TradedInstrument.InstrumentExchanges.Count > 0)
                    {
                        IInstrumentExchange instrumentExchange = TradedInstrument.InstrumentExchanges.GetDefault();
                        if (instrumentExchange != null)
                            serviceChargePercentage = (UnderlyingTx.TxSide == Side.Buy ? instrumentExchange.ServiceChargePercentageBuy :
                                                            (UnderlyingTx.TxSide == Side.Sell ? instrumentExchange.ServiceChargePercentageSell : 0m));
                    }

                    // there are two types of zero (?)
                    if (serviceChargePercentage == 0m)
                        serviceChargePercentage = 0m;
                }

                return serviceChargePercentage;
            }
        }

        private IOrderAllocation originalAllocation
        {
            get { return (IOrderAllocation)OriginalTransaction; }
        }

        #endregion
    }
}
