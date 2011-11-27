using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public class DividendHistory : CorporateActionHistory, IDividendHistory
    {
        protected DividendHistory() { }

        public DividendHistory(ISecurityInstrument instrument)
            : base(instrument)
        {
            this.DividendType = DividendTypes.Cash;
        }

        public DividendHistory(DividendTypes dividendType, ISecurityInstrument instrument,
            DateTime exDividendDate, DateTime settlementDate, Price unitPrice, decimal scripRatio)
            : this(instrument)
        {
            if (dividendType == DividendTypes.Cash && (unitPrice == null || unitPrice.IsZero))
                throw new ApplicationException("The unit price can not be null for cash dividend");
            else if (dividendType == DividendTypes.Scrip && scripRatio == 0M)
                throw new ApplicationException("The scrip ratio can not be 0 for scrip dividend");
            
            this.DividendType = dividendType;
            this.ExDividendDate = exDividendDate;
            this.SettlementDate = settlementDate;
            this.UnitPrice = unitPrice;
            this.ScripRatio = scripRatio;
        }

        public virtual DateTime ExDividendDate
        {
            get
            {
                return this.exDividendDate.HasValue ? this.exDividendDate.Value : DateTime.MinValue;
            }
            set
            {
                this.exDividendDate = value;
            }
        }
        public virtual DateTime SettlementDate         
        {
            get 
            {
                return this.settlementDate.HasValue ? this.settlementDate.Value : DateTime.MinValue;
            }
            set
            {
                this.settlementDate = value;
            }
        }

        public virtual DividendTypes DividendType { get; set; }
        public virtual Price UnitPrice { get; set; }
        public virtual decimal ScripRatio { get; set; }
        public virtual decimal TaxPercentage { get; set; }
        public virtual DividendTaxStyle TypeOfDividendTax { get; set; }
        public virtual bool IsInitialised { get; set; }
        public virtual bool IsExecuted { get; set; }
        public virtual bool IsGelicht { get; set; }
        public virtual string StockDivIsin { get; set; }
        public virtual IStockDividend StockDividend { get; set; }

        public virtual bool NeedsStockDividend 
        {
            get { return !ExDividendDate.Equals(SettlementDate) || DividendType == DividendTypes.Scrip; }
        }

        public virtual string DisplayStatus
        {
            get 
            {
                string status = "New(1)";
                if (IsInitialised) status = "Initialised(2)";
                if (IsExecuted)
                {
                    if (NeedsStockDividend)
                        status = "Executed(3)";
                    else
                        status = "Finished(3)";
                }
                if (IsGelicht) status = "Finished(4)";

                return status; 
            }
        }



        public virtual Price StockDivUnitPrice 
        {
            get
            {
                Price price = null;
                if (StockDividend != null && UnitPrice != null)
                    price = new Price(UnitPrice.Quantity, UnitPrice.Underlying, StockDividend);
                return price;
            }   
        }


        public virtual IStockDividendCollection StockDividends
        {
            get
            {
                IStockDividendCollection divs = (IStockDividendCollection)dividends.AsList();
                if (divs.Parent == null) divs.Parent = this;
                return divs;
            }
        }

        public virtual ICashDividendCollection CashDividends
        {
            get
            {
                ICashDividendCollection divs = (ICashDividendCollection)cashdividends.AsList();
                if (divs.Parent == null) divs.Parent = this;
                return divs;
            }
        }

        #region Overrides

        public override string DisplayString
        {
            get
            {
                return string.Format("Div. {0} {1} @ {2}", Instrument.Name, ExDividendDate.ToString("yyyy-MM-dd"), UnitPrice.Quantity.ToString("0.00###"));
            }
        }

        public override string ToString()
        {
            return DisplayString;
        }

        #endregion

        #region Privates

        private IDomainCollection<ICashDividend> cashdividends = new CashDividendCollection();
        private IDomainCollection<ICorporateActionStockDividend> dividends = new StockDividendCollection();
        private DateTime? exDividendDate;
        private DateTime? settlementDate;

        #endregion
    }
}
