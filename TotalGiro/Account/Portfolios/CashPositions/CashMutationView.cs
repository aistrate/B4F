using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public abstract class CashMutationView : ICashMutationView
    {
        public CashMutationView()
        {
            IsSettled = true;
        }
        
        public int Key { get { return this.SearchKey.GetHashCode(); } }
        public abstract CashMutationViewTypes CashMutationViewType { get; }
        public string SearchKey { get; set; }
        public ICashSubPosition Position { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public abstract IAccountTypeInternal Account { get; }
        public abstract string FullDescription { get; }
        public abstract bool IsTransaction { get;  }
        public Money Amount { get; set; }
        public string DisplayAmount
        {
            get
            {
                try
                {
                    return Amount != null ? Amount.DisplayString : "Onbekend";
                }
                catch (Exception)
                {
                    
                    throw;
                }

            }
        }
        public string TransactionType { get; set; }
        public abstract void setTransactionType();
        public abstract string TypeID { get; }
        public abstract string TypeDescription { get; }
        public bool IsSettled { get; set; }
    }
}
