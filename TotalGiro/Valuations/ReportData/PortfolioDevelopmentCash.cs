using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Valuations
{
    /// <summary>
    /// Used to report cash development using the ValuationCashMutation class
    /// </summary>
    public class PortfolioDevelopmentCash
    {
        public PortfolioDevelopmentCash(PortfolioDevelopment parent, ValuationCashTypes valuationCashType, string cashTypeDescription, Money amount)
        {
            this.parent = parent;
            this.valuationCashType = valuationCashType;
            this.cashTypeDescription = cashTypeDescription;
            this.amount = amount;
        }
        
        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return parent.Account; }
        }

        public virtual DateTime BeginDate
        {
            get { return parent.BeginDate; }
        }

        public virtual DateTime EndDate
        {
            get { return parent.EndDate; }
        }

        public virtual ValuationCashTypes ValuationCashType
        {
            get { return valuationCashType; }
            internal set { this.valuationCashType = value; }
        }

        public virtual string ValuationCashTypeDescription
        {
            get 
            {
                if (this.cashTypeDescription != string.Empty)
                    return this.cashTypeDescription;
                else
                    return ValuationCashType.ToString(); 
            }
        }

        public virtual Money Amount
        {
            get { return this.amount; }
            internal set { this.amount = value; }
        }

        public virtual int IsIncome
        {
            get { return Convert.ToInt32(Amount.Sign); }
        }

        #region Privates

        private PortfolioDevelopment parent;
        private ValuationCashTypes valuationCashType;
        private string cashTypeDescription = string.Empty;
        private Money amount;

        #endregion
    }
}
