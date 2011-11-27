using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class TransactionType : ITransactionType
    {
        protected TransactionType() { }
        
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// The name of the TransactionType
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            protected set { name = value; }
        }

        /// <summary>
        /// The description of the TransactionType
        /// </summary>
        public virtual string Description
        {
            get { return description; }
            protected set { description = value; }
        }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString()
        {
            return Name;
        }

        #region PrivateVariables

        private int key;
        private string name;
        private string description;

        #endregion
    }
}
