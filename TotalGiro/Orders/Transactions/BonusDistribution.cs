using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.Instruments.CorporateAction;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class BonusDistribution : CorporateAction , IBonusDistribution
    {
        public BonusDistribution() { }

        public BonusDistribution(IAccountTypeInternal acctA, IAccount acctB,
        InstrumentSize valueSize,
        Price price, decimal exRate, DateTime transactionDate, DateTime transactionDateTime,
        Decimal ServiceChargePercentage, Side txSide,
        ICorporateActionBonusDistribution bonusDistributionDetails, ITradingJournalEntry tradingJournalEntry,
        IGLLookupRecords lookups, ListOfTransactionComponents[] components)
            : base(acctA, acctB, valueSize,
                 price, exRate, transactionDate, transactionDateTime,
                 ServiceChargePercentage, txSide,
                 tradingJournalEntry,
                 lookups, components)
        {
            this.CorporateActionType = CorporateActionTypes.BonusDistribution;
            this.CorporateActionDetails = bonusDistributionDetails;
        }

        public Decimal BonusPercentage { get; set; }

        //public IInstrumentHistoryBonusDistribution ParentDistribution
        //{
        //    get
        //    {
        //        return (IInstrumentHistoryBonusDistribution)InstrumentTransformation;
        //    }
        //    set
        //    {
        //        InstrumentTransformation = value;
        //    }
        //}

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.BonusDistribution; }
        }

        

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            BonusDistribution newStorno = new BonusDistribution();
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            return newStorno;
        }


        protected override void setDescription()
        {
            if (string.IsNullOrEmpty(base.description))
            {
                if (this.IsStorno)
                {
                    this.Description = string.Format("Storno ({0}) {1}",
                        this.OriginalTransaction.Key,
                        this.StornoReason);
                }
                else
                {
                    this.Description = string.Format("Transfer {0} {1} {2}",
                        TxSide == Side.XI ? "in" : "out",
                        ValueSize.ToString("#,###,##0.00####"),
                        TradedInstrument.Name);
                }
            }
        }

        public override INota CreateNota()
        {
            // TODO: 'CorporateActionType == CorporateActionTypes.Conversion' is TEMPORARY, until Bonus Emission nota is created
            if (Approved && !NotaMigrated && StornoTransaction == null)
            {
                //if (TxNota == null)
                //    return new NotaInstrumentConversion(this);
                //else
                    throw new ApplicationException(string.Format("Transaction {0} already has a nota ({1}).", Key, TxNota.Key));
            }

            return null;
        }
    }


}
