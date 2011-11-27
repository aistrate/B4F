using System;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationCashType : IValuationCashType
    {
        protected ValuationCashType () { }

        public virtual ValuationCashTypes Key
        {
            get { return (ValuationCashTypes)key; }
        }

        public virtual string CashType
        {
            get { return cashType; }
            protected set { cashType = value; }
        }

        public virtual string Description
        {
            get { return description; }
            protected set { description = value; }
        }

        public virtual short Sign
        {
            get { return sign; }
            protected set { sign = value; }
        }

        public virtual bool IsSettled
        {
            get { return isSettled; }
            protected set { isSettled = value; }
        }
	
        #region Privates

        private int key;
        private string cashType;
        private string description;
        private short sign;
        private bool isSettled;

        #endregion
    }
}
