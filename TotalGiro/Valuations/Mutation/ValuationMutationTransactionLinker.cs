using System;
using System.Collections.Generic;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationMutationTransactionLinker
    {
        protected ValuationMutationTransactionLinker() { }
        
        public virtual int Key
        {
            get { return key; }
        }

        public virtual ITransaction Transaction
        {
            get { return transaction; }
        }

        #region Privates

        private int key;
        private ITransaction transaction;
        
        #endregion
    }
}
