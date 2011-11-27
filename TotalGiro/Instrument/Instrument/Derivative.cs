using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public abstract class Derivative : TradeableInstrument, IDerivative
    {
        protected Derivative() { }

        #region Props

        public virtual IDerivativeMaster Master { get; set; }

        #endregion

        #region Overrides

        public override int ContractSize
        {
            get { return Master.ContractSize; }
        }

        public override IExchange DefaultExchange
        {
            get { return Master.Exchange ?? base.DefaultExchange; }
            set { base.DefaultExchange = value; }
        }

        public override IExchange HomeExchange
        {
            get { return Master.Exchange ?? base.HomeExchange; }
            set { base.HomeExchange = value; }
        }

        public override string DefaultExchangeName
        {
            get { return (HomeExchange != null ? HomeExchange.ExchangeName : null); }
        }

        public override ICurrency CurrencyNominal
        {
            get { return Master.CurrencyNominal; }
            set { base.CurrencyNominal = value; }
        }

        public override int DecimalPlaces
        {
            get { return Master.DecimalPlaces; }
            set { base.DecimalPlaces = value; }
        }

        public override string CompanyName
        {
            get { return (Master.Underlying != null ? Master.Underlying.CompanyName : null); }
            set { base.CompanyName = value; }
        }

        public override IInstrumentExchangeCollection InstrumentExchanges
        {
            get { return new InstrumentExchangeCollection(HomeExchange, this); }
        }

        #endregion

        #region Methods

        protected override bool validate()
        {

            if (this.Master == null)
                throw new ApplicationException("Derivative Master is mandatory.");
            return base.validate();
        }

        #endregion
    }
}
