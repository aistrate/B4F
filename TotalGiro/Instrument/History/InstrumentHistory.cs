using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments.History
{
    public abstract class InstrumentHistory : IInstrumentHistory
    {
        protected InstrumentHistory() 
        {
        }

        internal InstrumentHistory(IInstrument instrument, DateTime changeDate)  :this()
        {
            if (instrument == null)
                throw new ApplicationException("instrument can not be null");
            
            this.Instrument = instrument;
            this.ChangeDate = changeDate.Date;
        }
        
        public virtual int Key {get;set;}
        public virtual IInstrument Instrument { get; set; }
        public abstract CorporateActionTypes CorporateActionType { get; }
        public virtual bool IsInitialised { get; set; }
        public virtual bool IsExecuted { get; set; }
        public virtual DateTime ChangeDate
        {
            get
            {
                return this.changeDate.HasValue ? changeDate.Value : DateTime.MinValue;
            }
            set
            {
                this.changeDate = value;
            }
        }

        public virtual DateTime CreationDate
        {
            get
            {
                return this.creationDate.HasValue ? creationDate.Value : DateTime.MinValue;
            }
            set
            {
                this.creationDate = value;
            }
        }

        public virtual DateTime LastUpdated { get; internal set; }

        public DateTime ExecutionDate
        {
            get
            {
                return this.executionDate.HasValue ? executionDate.Value : DateTime.MinValue;
            }
            set
            {
                this.executionDate = value;
            }
        }

        public virtual string Description
        {
            get { return string.Empty; }
        }

        #region Private Variables

        private DateTime? changeDate;
        private DateTime? creationDate;
        private DateTime? executionDate;

        #endregion

    }
}
