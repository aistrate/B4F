using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Communicator.FSInterface;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Orders
{
    #region Enums

    /// <summary>
    /// This enumeration lists the possiblities whether an order is fillable.
    /// If the order is not fillable there are several causes why not
    /// </summary>
    public enum OrderFillability
    {
        /// <summary>
        /// This order is fillable
        /// </summary>
        True,
        /// <summary>
        /// This order not is fillable since it is a child order, and only the parent orders can be filled
        /// </summary>
        NoChildOrder,
        /// <summary>
        /// This order not is fillable since the status of the order does not allow filling due to several causes
        /// </summary>
        NoStatusNotFillable
    }

    /// <summary>
    /// This enumeration lists the possible stati an order can receive
    /// </summary>
    public enum OrderStati
    {
        /// <summary>
        /// The order has just been entered
        /// </summary>
        New = 1,
        //Approved = 2,
        /// <summary>
        /// The order has been routed to the relevant desk/exchange
        /// </summary>
        Routed = 3,
        /// <summary>
        /// The order has been received at the relevant desk/exchange
        /// </summary>
        Placed = 4,
        /// <summary>
        /// The order has been partly filled
        /// </summary>
        PartFilled = 5,
        /// <summary>
        /// The order has been completely filled, but not all transactions have been approved
        /// </summary>
        Filled = 6,
        /// <summary>
        /// The order has been completely filled and all transactions have been approved
        /// </summary>
        Checked = 7,
        /// <summary>
        /// The order is finished
        /// </summary>
        Terminated = 8
    }

    /// <summary>
    /// This enumeration lists the possible cancel stati an order can receive
    /// </summary>
    public enum OrderCancelStati
    {
        /// <summary>
        /// The order is in neutral mode, so no cancel request has been made
        /// </summary>
        Neutral = 1,
        /// <summary>
        /// A cancel request was done
        /// </summary>
        CancelRequested = 2,
        /// <summary>
        /// The cancel request is in the queue to be sent to the relevant desk/exchange
        /// </summary>
        CancelQueued = 3,
        /// <summary>
        /// The cancel request is pending on the relevant desk/exchange
        /// </summary>
        CancelPending = 4,
        /// <summary>
        /// The cancel request has been received on the relevant desk/exchange
        /// </summary>
        CancelOnExchange = 5,
        /// <summary>
        /// The order has been cancelled
        /// </summary>
        Cancelled = 6
    }

    /// <summary>
    /// Open or Closed.
    /// </summary>
    public enum OpenClose
    {
        Open = 1,        
        Close
    }

    /// <summary>
    /// The side of the order. Buy or Sell.
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// Transfer in
        /// </summary>
        XI = 2,
        /// <summary>
        /// This is a buy order
        /// </summary>
        Buy = 1,
        /// <summary>
        /// This is a sell order
        /// </summary>
        Sell = -1,
        /// <summary>
        /// Transfer out
        /// </summary>
        XO = -2
    }

    /// <summary>
    /// This enumeration lists the type of errors that can happen to the order
    /// </summary>
    public enum OrderErrors
    {
        /// <summary>
        /// There was no error
        /// </summary>
        Neutral = 0,
        /// <summary>
        /// There was an allocation problem
        /// </summary>
        AllocationProblem = 1
    }

    /// <summary>
    /// This enumeration lists the types of orders
    /// </summary>
    public enum OrderTypes
    {
        /// <summary>
        /// This is an amount based order
        /// </summary>
        AmountBased = 0,
        /// <summary>
        /// This is an size based order
        /// </summary>
        SizeBased = 1,
        /// <summary>
        /// This is an monetary order
        /// </summary>
        Monetary = 2
    }

    /// <summary>
    /// This is the action that caused the creation of the order
    /// </summary>
    [Flags()]
    public enum OrderActionTypes
    {
        /// <summary>
        /// A first deposit caused the creation of the order
        /// </summary>
        FirstDeposit = 1,
        /// <summary>
        /// A rebalance caused the creation of the order
        /// </summary>
        Rebalance = 2,
        /// <summary>
        /// This order was entered as a single order 
        /// </summary>
        SingleOrder = 4,
        /// <summary>
        /// No special action caused the creation of the order
        /// </summary>
        NoAction = 8,
        /// <summary>
        /// A deposit of new money caused the creation of the order
        /// </summary>
        Deposit = 16,
        /// <summary>
        /// A withdrawal (in the future) caused the creation of the order
        /// </summary>
        Withdrawal = 32,
        /// <summary>
        /// The client closes the account (is leaving)
        /// </summary>
        Departure = 64
    }

    /// <summary>
    /// This enumeration lists the possible filter options on the <see cref="T:B4F.TotalGiro.Orders.T:B4F.TotalGiro.Orders.Side">Side</see> flag of the order
    /// </summary>
    public enum OrderSideFilter
    {
        /// <summary>
        /// All orders are returned independant of the side
        /// </summary>
        All,
        /// <summary>
        /// Only Buy orders are returned
        /// </summary>
        Buy,
        /// <summary>
        /// Only Sell orders are returned
        /// </summary>
        Sell
    }

    #endregion

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Orders.Order">Order</see> class
    /// </summary>
    public interface IOrder : ITotalGiroBase<IOrder>, IAuditable, ICommissionParent, IOrderFormulaDetails
    {
        int Key { get; }
        B4F.TotalGiro.Orders.OrderFillability IsFillable { get; }
        bool Approve();
        bool Approved { get; }
        bool Cancel();
        bool DoNotChargeCommission { get; set; }
        bool IsAggregateOrder { get; }
        bool IsAmountBased { get; }
        bool IsCancellable { get; }
        bool IsClosed { get; }
        bool IsCompleteFilled { get; }
        bool IsMonetary { get; }
        bool IsNetted { get; }
        bool IsSecurity { get; }
        bool IsSizeBased { get; }
        bool IsStgOrder { get; }
        bool IsUnApproveable { get; }
        bool IsCommissionRelevant { get; }
        bool SetExRate(decimal rate);
        bool SetNew();
        bool UnApprove();
        bool UpdateStateFromTX();
        Commission CommissionDetails { get; set; }
        DateTime AllocationDate { get; set; }
        DateTime CreationDate { get; }
        DateTime DateClosed { get; set; }
        DateTime LastUpdated { get; }
        decimal ExRate { get; }
        IAccountTypeInternal Account { get; }
        ICurrency OrderCurrency { get; }
        IFSExportFile ExportFile { get; }
        IInstruction Instruction { get; set; }
        IInstrument RequestedInstrument { get; }
        InstrumentSize FilledValue { get; }
        InstrumentSize OpenValue { get; }
        InstrumentSize PlacedValue { get; }
        InstrumentSize Value { get; }
        int OrderID { get; }
        IOrder ParentOrder { get; set; }
        IOrderCollection ChildOrders { get; }
        IOrderAllocation FillasAllocation(IOrderExecution ParentExecution, ITradingJournalEntry tradingJournalEntry, IGLLookupRecords lookups, IFeeFactory feeFactory);
        ITransactionOrderCollection Transactions { get; }
        Money Amount { get; }
        Money BaseAmount { get; }
        Money Commission { get; }
        Money EstimatedAmount { get; }
        Money GrossAmount { get; }
        Money GrossAmountBase { get; }
        Money OpenAmount { get; }
        OpenClose OrderOpenClose { get; set; }
        OrderActionTypes ActionType { get; set; }
        OrderCancelStati CancelStatus { get; set; }
        OrderErrors Err { get; set; }
        OrderStati Status { get; set; }
        OrderTypes OrderType { get; }
        OrderValidationResult Validate();
        Price Price { get; }
        Side Side { get; }
        string CommissionInfo { get; set; }
        string DisplayIsSizeBased { get; }
        string DisplayStatus { get; }
        string DisplayTradedInstrumentIsin { get; }
        string ErrDescription { get; set; }
        string OrderInfo { get; set; }
        string ToString();
        void SetParentOrder(IOrder parent);
    }
}
