using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Orders.Transactions
{
    #region enums

    public enum TransactionTypes
    {
        Transaction = 0,
        TransactionTrading = 1,
        TransactionOrder = 2,
        TransactionOrderChild = 4,
        Allocation = 8,
        Crumble = 16,
        Execution = 32,
        Migration = 64,
        NTM = 128,       
        CorporateAction = 256,
        InstrumentConversion = 512,
        BonusDistribution = 1024,
        StockDividend = 2048,
        CorporateActionExecution = 4096
    }

    public enum TransactionSide
    {
        /// <summary>
        /// The PositionTx looks at the Transaction from the perspective of Account A
        /// </summary>
        A,
        /// <summary>
        /// The PositionTx looks at the Transaction from the perspective of Account B
        /// </summary>
        B
    }

    public class ListOfTransactionComponents
    {
        public Money ComponentValue { get; set; }
        public BookingComponentTypes ComponentType { get; set; }
    }

    #endregion

    public interface ITransaction : ITotalGiroBase<ITransaction>, IAuditable, ICashPresentation
    {
        int Key { get; }
        bool Approved { get; set; }
        bool IsApproveable { get; }
        bool IsClientSettled { get; set; }
        bool IsExecution { get; }
        bool IsStorno { get; set; }
        bool IsStornoable { get; }
        bool IsRelevant { get; }
        DateTime ApprovalDate { get; set; }
        DateTime ContractualSettlementDate { get; set; }
        DateTime CreationDate { get; set; }
        string CreatedBy { get; set; }
        string ApprovedBy { get; set; }
        DateTime TransactionDate { get; set; }
        DateTime TransactionDateTime { get; set; }
        string Description { get; set; }
        Decimal ExchangeRate { get; set; }
        Decimal ServiceChargePercentage { get; set; }
        IAccount AccountB { get; set; }
        IAccountTypeInternal AccountA { get; set; }
        IAccountTypeInternal[] GetStornoAccounts(IManagementCompany managementCompany);
        IExchange Exchange { get; set; }
        ITradeableInstrument TradedInstrument { get; }
        InstrumentSize ValueSize { get; set; }
        int MigratedTradeKey { get; set; }
        ITransactionType TradeType { get; }
        ITransaction OriginalTransaction { get; set; }
        ITransaction Storno(IAccountTypeInternal stornoAccount, IInternalEmployeeLogin employee, string reason, ITradingJournalEntry tradingJournalEntry);
        ITransaction StornoTransaction { get; set; }
        IList<IBondCouponPayment> GetBondPaymentsToStorno();
        ITransactionComponentCollection Components { get; }
        ITxPositionTxCollection PositionTransactions { get; }
        ITradingJournalEntry TradingJournalEntry { get; set; }
        Money CounterValueSize { get; }
        Money CounterValueSizeBaseCurrency { get; }
        Money Commission { get; }
        Money ServiceCharge { get; }
        Money AccruedInterest { get; }
        Money ObsoleteCounterValue { get; set; }
        Money ObsoleteCommission { get; set; }
        Money ObsoleteServiceCharge { get; set; }
        Price Price { get; set; }
        Side TxSide { get; set; }
        string DisplayTradedInstrumentIsin { get; }
        string StornoReason { get; set; }
        string TransactionTypeDisplay { get; }
        TransactionTypes TransactionType { get; }
        Money TotalAmount { get; }
        Money TotalBaseAmount { get; }
        INota TxNota { get; set; }
        bool NotaMigrated { get; }
        bool TempMigrationFlag { get; set; }

        bool Approve(IInternalEmployeeLogin employee);
        bool Approve(IInternalEmployeeLogin employee, bool raiseStornoLimitExceptions);

        INota CreateNota();
        void Migrate();
    }
}
