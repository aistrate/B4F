using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public enum CashMutationViewTypes
    {
        CashTransfer = 0,
        ManagementFee,
        CashDividend,
        ForeignExchange,
        Transaction,
        BondCouponPayment
    }
    
    public interface ICashMutationView
    {
        int Key { get;  }
        string SearchKey { get; set; }
        DateTime TransactionDate { get; set; }
        DateTime CreationDate { get; set; }
        IAccountTypeInternal Account { get; }
        string FullDescription { get; }
        Money Amount { get; set; }
        string DisplayAmount { get; }
        string TransactionType { get; set; }
        CashMutationViewTypes CashMutationViewType { get; }
        void setTransactionType();
        bool IsTransaction { get; }
        string TypeID { get; }
        string TypeDescription { get; }
    }
}
