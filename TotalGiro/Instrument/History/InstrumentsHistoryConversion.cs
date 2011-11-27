using System;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments.History
{
    public class InstrumentsHistoryConversion : InstrumentHistory, IInstrumentsHistoryConversion
    {
        protected InstrumentsHistoryConversion() { }

        public InstrumentsHistoryConversion(IInstrument oldInstrument, IInstrument newInstrument, 
                DateTime changeDate, decimal oldChildRatio, byte newParentRatio, bool isSpinOff)
            : base(oldInstrument, changeDate)
        {
            if (oldInstrument.ParentInstrument != null)
                throw new ApplicationException(string.Format("This instrument {0} already has a parent: {1}", 
                    oldInstrument.DisplayNameWithIsin,
                    oldInstrument.ParentInstrument.DisplayIsinWithName));
            
            if (newInstrument == null)
                throw new ApplicationException("newInstrument can not be null");

            if (oldChildRatio <= 0 || newParentRatio <= 0)
                throw new ApplicationException("Both ratios have to be greater than zero");

            
            NewInstrument = newInstrument; 
            OldChildRatio = oldChildRatio;
            NewParentRatio = newParentRatio;
            IsSpinOff = isSpinOff;
        }


        public virtual IInstrument NewInstrument { get; set; }
        public virtual decimal OldChildRatio { get; set; }
        public virtual byte NewParentRatio { get; set; }
        public virtual decimal ConversionRate
        {
            get
            {
                decimal newSize = Convert.ToDecimal(NewParentRatio);
                return newSize / OldChildRatio;
            }
        }
        public virtual bool IsSpinOff { get; set; }
        public override CorporateActionTypes CorporateActionType
        {
            get { return CorporateActionTypes.Conversion; }
        }

        public override string Description
        {
            get { return string.Format("{0:0.######} oude voor {1} nieuwe", OldChildRatio, NewParentRatio); }
        }

        /// <summary>
        /// The coupons that belong to this item.
        /// </summary>
        public virtual IInstrumentConversionCollection Conversions
        {
            get
            {
                InstrumentConversionCollection items = (InstrumentConversionCollection)this.conversions.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        private IDomainCollection<IInstrumentConversion> conversions;
    
    }
}
