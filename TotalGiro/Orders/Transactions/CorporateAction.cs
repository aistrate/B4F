using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.Instruments.CorporateAction;

namespace B4F.TotalGiro.Orders.Transactions
{
    public abstract class CorporateAction : Transaction, ICorporateAction
    {
        public CorporateAction() { }

        public CorporateAction(IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize,
                Price price, decimal exRate, DateTime transactionDate, DateTime transactionDateTime,
                Decimal ServiceChargePercentage, Side txSide,
                ITradingJournalEntry tradingJournalEntry,
                IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(acctA, acctB, valueSize,
                 price, exRate, transactionDate, transactionDateTime,
                 ServiceChargePercentage, txSide,
                 tradingJournalEntry,
                 lookups, components)
        {

        }

        public virtual CorporateActionTypes CorporateActionType { get; set; }
        public virtual InstrumentSize PreviousSize { get; set; }

        /// <summary>
        /// The details of the cash dividend (date & price)
        /// </summary>
        public virtual ICorporateActionHistory CorporateActionDetails
        {
            get { return corporateActionDetails; }
            set { corporateActionDetails = value; }
        }

        #region Private Variables

        private ICorporateActionHistory corporateActionDetails;

        #endregion

    }
}
