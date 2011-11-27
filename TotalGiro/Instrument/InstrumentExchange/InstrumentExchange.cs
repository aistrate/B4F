using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class holds information specific to exchange and instrument combination
    /// </summary>
    public class InstrumentExchange : IInstrumentExchange
    {
        protected InstrumentExchange() {}

        public InstrumentExchange(ITradeableInstrument instrument, IExchange exchange, byte numberOfDecimals)
        {
            Instrument = instrument;
            Exchange = exchange;
            NumberOfDecimals = numberofdecimals;
        }

        /// <summary>
        /// Identifier for combination instrument/exchange
        /// </summary>
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Get/set instrument
        /// </summary>
        public virtual ITradeableInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        /// <summary>
        /// Get/set exchange
        /// </summary>
        public virtual IExchange Exchange
        {
            get { return exchange; }
            set { exchange = value; }
        }

        /// <summary>
        /// Get/set DefaultCounterParty
        /// </summary>
        public virtual ICounterPartyAccount DefaultCounterParty
        {
            get { return defaultCounterParty; }
            set { defaultCounterParty = value; }
        }

        /// <summary>
        /// Get/set number of decimals for instrument/exchange
        /// </summary>
        public virtual byte NumberOfDecimals
        {
            get { return numberofdecimals; }
            set { numberofdecimals = value; }
        }

        /// <summary>
        /// The minimum size unit in which the instruments is traded and in multiples of this size unit.
        /// </summary>
        public virtual decimal TickSize { get; set; }

        /// <summary>
        /// Get/set certification required for instrument/exchange
        /// </summary>
        public virtual bool CertificationRequired
        {
            get { return certificationrequired; }
            set { certificationrequired = value; }
        }

        public virtual string RegisteredInNameOf
        {
            get { return registeredinnameof; }
            set { registeredinnameof = value; }
        }

        public virtual string DividendPolicy
        {
            get { return dividendpolicy; }
            set { dividendpolicy = value; }
        }

        public virtual string CommissionRecipientName
        {
            get { return commissionrecipientname; }
            set { commissionrecipientname = value; }
        }

        /// <summary>
        /// Get/set default settlement period
        /// </summary>
        public virtual Int16 DefaultSettlementPeriod
        {
            get { return defaultSettlementPeriod; }
            set { defaultSettlementPeriod = value; }
        }

        public virtual bool DoesSupportAmountBasedBuy
        {
            get { return doesSupportAmountBasedBuy; }
            set { doesSupportAmountBasedBuy = value; }
        }

        public virtual bool DoesSupportAmountBasedSell
        {
            get { return doesSupportAmountBasedSell; }
            set { doesSupportAmountBasedSell = value; }
        }

        public virtual bool DoesSupportServiceCharge
        {
            get { return doesSupportServiceCharge; }
            set { doesSupportServiceCharge = value; }
        }

        public virtual decimal ServiceChargePercentageBuy
        {
            get { return serviceChargePercentageBuy; }
            set { serviceChargePercentageBuy = value; }
        }

        public virtual decimal ServiceChargePercentageSell
        {
            get { return serviceChargePercentageSell; }
            set { serviceChargePercentageSell = value; }
        }

        public decimal GetServiceChargePercentageForOrder(IOrder order)
        {
            decimal scPerc = 0;
            if (order.Side == Side.Buy && ServiceChargePercentageBuy > 0)
                scPerc = ServiceChargePercentageBuy;
            else if (order.Side == Side.Sell && ServiceChargePercentageSell > 0)
                scPerc = ServiceChargePercentageSell;
            return scPerc;
        }

        public virtual bool ServiceChargeBothSides
        {
            get 
            { 
                return (ServiceChargePercentageBuy > 0 && ServiceChargePercentageSell > 0); 
            }
        }

        public virtual string ServiceChargeDisplayInfo
        {
            get 
            {
                string info = "";
                
                if (ServiceChargePercentageBuy > 0)
                    info = string.Format("buy {0:0.##}%", ServiceChargePercentageBuy * 100m);
                
                if (ServiceChargePercentageSell > 0)
                    info += (info == "" ? "" : ", ") + string.Format("sell {0:0.##}%", ServiceChargePercentageSell * 100m);
                
                return info; 
            }
        }


        #region Private Variables

        private int key;
        private IExchange exchange;
        private ICounterPartyAccount defaultCounterParty;
        private ITradeableInstrument instrument;
        byte numberofdecimals;
        bool certificationrequired;
        private string registeredinnameof;
        private string dividendpolicy;
        private string commissionrecipientname;
        private Int16 defaultSettlementPeriod;
        private bool doesSupportAmountBasedBuy = true;
        private bool doesSupportAmountBasedSell = true;
        private bool doesSupportServiceCharge = false;
        private decimal serviceChargePercentageBuy;
        private decimal serviceChargePercentageSell;

        #endregion

    }
}
