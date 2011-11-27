using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Communicator.ExtCustodians.Positions
{
    public class ExtPosition
    {
        protected ExtPosition() { }

        public ExtPosition(ExtCustodian custodian, InstrumentSize size, DateTime balanceDate)
        {
            Custodian = custodian;
            Size = size;
            BalanceDate = balanceDate;
        }   
    
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual ExtCustodian Custodian
        {
            get { return custodian; }
            set { custodian = value; }
        }

        public virtual InstrumentSize Size
        {
            get { return size; }
            set { size = value; }
        }

        public virtual DateTime BalanceDate
        {
            get { return balanceDate; }
            set { balanceDate = value; }
        }

        #region Privates

         private int key;
         private ExtCustodian custodian;
         private InstrumentSize size;
         private DateTime balanceDate;
        
         #endregion Privates
 }
}
