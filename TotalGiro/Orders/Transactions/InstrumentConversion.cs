using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Orders.Transactions
{
    public class InstrumentConversion : CorporateAction, IInstrumentConversion
    {
        #region Constructor

        public InstrumentConversion() { }

        public InstrumentConversion(IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize, InstrumentSize convertedInstrumentSize,
                decimal exRate, IInstrumentHistory instrumentTransformation, 
                ITradingJournalEntry tradingJournalEntry)
            : this(acctA, acctB, valueSize, convertedInstrumentSize, null,
                exRate, instrumentTransformation, tradingJournalEntry, null)
        {

        }

        public InstrumentConversion(IAccountTypeInternal acctA, IAccount acctB,
                InstrumentSize valueSize, InstrumentSize convertedInstrumentSize, Money additionalCash,
                decimal exRate, IInstrumentHistory instrumentTransformation,
                ITradingJournalEntry tradingJournalEntry, IGLLookupRecords lookups)
            : base(acctA, acctB, valueSize, valueSize.GetPrice(0M), exRate,
                instrumentTransformation.ChangeDate, instrumentTransformation.ChangeDate, 0M,
                (valueSize.Sign ? Side.XI : Side.XO), tradingJournalEntry, lookups, null)
        {
            if (instrumentTransformation == null)
                throw new ApplicationException("Corporate action information is missing");


            if (!(valueSize != null && valueSize.IsNotZero && convertedInstrumentSize != null && convertedInstrumentSize.IsNotZero))
                throw new ApplicationException("Not all instrument information for this corporate action is available, both instruments have to be supplied");

            if (valueSize.Underlying.Equals(convertedInstrumentSize.Underlying))
                throw new ApplicationException("Both instruments can not be the same in a corporate action");

            if (valueSize.Sign.Equals(convertedInstrumentSize.Sign))
                throw new ApplicationException("Both instruments can not have the same side in a corporate action");

            IInstrumentsHistoryConversion conversion = (IInstrumentsHistoryConversion)instrumentTransformation;
            if (!(conversion.Instrument.Equals(valueSize.Underlying) && conversion.NewInstrument.Equals(convertedInstrumentSize.Underlying)))
                throw new ApplicationException("Corporate action does not equal the instrument history data");

            decimal diff = (convertedInstrumentSize.Abs().Quantity / valueSize.Abs().Quantity) - conversion.ConversionRate;
            if (diff != 0)
                throw new ApplicationException("Sizes do not correspond with conversion rate of the Corporate action");




            if (additionalCash != null && additionalCash.IsNotZero)
            {
                ListOfTransactionComponents[] components = new ListOfTransactionComponents[1];
                components[0] = new ListOfTransactionComponents() { ComponentType = BookingComponentTypes.Conversion, ComponentValue = additionalCash };
                createComponents(lookups, components);

            }

            this.CorporateActionType = CorporateActionTypes.Conversion;

            ConvertedInstrumentSize = convertedInstrumentSize;
            CorporateActionType = instrumentTransformation.CorporateActionType;
            InstrumentTransformation = instrumentTransformation;
        }

        #endregion

        #region Props

        /// <summary>
        /// Used for Corporate actions
        /// </summary>
        public InstrumentSize ConvertedInstrumentSize { get; set; }
        public virtual IInstrumentHistory InstrumentTransformation { get; set; }
        



        public virtual decimal ConversionRate
        {
            get { return ConvertedInstrumentSize.Abs().Quantity / ValueSize.Abs().Quantity; }
        }

        #endregion

        #region Overrides

        public override bool Approve(IInternalEmployeeLogin employee)
        {
            return Approve(employee, false);
        }

        public override bool Approve(IInternalEmployeeLogin employee, bool raiseStornoLimitExceptions)
        {
            createPositionTxs(AccountA, AccountB, ConvertedInstrumentSize, PositionsTxValueTypes.Conversion);
            return approve(employee, raiseStornoLimitExceptions);
        }

        public override ITransaction Storno(IAccountTypeInternal stornoAccount, B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry)
        {
            InstrumentConversion newStorno = new InstrumentConversion();
            if (this.ConvertedInstrumentSize != null)
                newStorno.ConvertedInstrumentSize = this.ConvertedInstrumentSize.Negate();
            this.storno(stornoAccount, employee, reason, tradingJournalEntry, newStorno);
            return newStorno;
        }

        public override TransactionTypes TransactionType
        {
            get { return TransactionTypes.InstrumentConversion; }
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
                    string convInstInfo = "";
                    if (this.ConvertedInstrumentSize.Underlying != null)
                        convInstInfo = string.Format("for {0} {1}",
                            ConvertedInstrumentSize.ToString("#,###,##0.00####"),
                            ConvertedInstrumentSize.Underlying.Name);

                    this.Description = string.Format("Transfer {0} {1} {2} {3}",
                        TxSide == Side.XI ? "in" : "out",
                        ValueSize.ToString("#,###,##0.00####"),
                        TradedInstrument.Name,
                        convInstInfo);
                }
            }
        }

        public override INota CreateNota()
        {
            // TODO: 'CorporateActionType == CorporateActionTypes.Conversion' is TEMPORARY, until Bonus Emission nota is created
            if (Approved && !NotaMigrated && StornoTransaction == null )
            {
                if (TxNota == null)
                    return new NotaInstrumentConversion(this);
                else
                    throw new ApplicationException(string.Format("Transaction {0} already has a nota ({1}).", Key, TxNota.Key));
            }

            return null;
        }

        #endregion


    }
}
