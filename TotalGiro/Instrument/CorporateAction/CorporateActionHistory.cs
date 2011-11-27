using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments.CorporateAction
{
    public abstract class CorporateActionHistory : ICorporateActionHistory
    {
        protected CorporateActionHistory() { }

        public CorporateActionHistory(ISecurityInstrument instrument)
        {
            this.Instrument = instrument;
        }

        public virtual int Key { get; set; }
        public virtual ISecurityInstrument Instrument { get; set; }
        public virtual decimal TotalNumberOfUnits { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
            protected set { this.creationDate = value; }
        }
        public abstract string DisplayString { get; }

        #region Privates

        private DateTime creationDate = DateTime.Now;

        #endregion

    }
}
