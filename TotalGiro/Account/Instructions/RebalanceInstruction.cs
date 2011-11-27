using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Instructions.Exclusions;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This is a instruction used for rebalancing
    /// </summary>
    public class RebalanceInstruction : InstructionTypeRebalance, IRebalanceInstruction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.RebalanceInstruction">RebalanceInstruction</see> class.
        /// </summary>
        protected RebalanceInstruction() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.RebalanceInstruction">RebalanceInstruction</see> class.
        /// </summary>
        /// <param name="account">The account the withdrawal will belong to</param>
        /// <param name="executionDate">The date the instruction should execute</param>
        /// <param name="orderActionType">The type of instruction that is placed on the orders</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        /// <param name="cashTransfers">The transfers involved</param>
        public RebalanceInstruction(IAccountTypeCustomer account, DateTime executionDate, OrderActionTypes orderActionType, bool doNotChargeCommission, IList<IJournalEntryLine> cashTransfers)
            : base(account, executionDate, orderActionType, doNotChargeCommission, cashTransfers)
        {
            excludedComponents = new RebalanceExclusionCollection(this);
        }

        #endregion

        #region props

        /// <summary>
        /// After the Rebalance instruction is created the amount based orders
        /// it need to set the cash transfers to processed, otherwise they will be picked up 
        /// in the new porfolio screen
        /// </summary>
        public bool NeedsToCheckInstructionInSyncWithModel
        {
            get { return (status == RebalanceInstructionStati.PendingOrders); }
        }

        /// <summary>
        /// After the Rebalance instruction is created the amount based orders
        /// it need to set the cash transfers to processed, otherwise they will be picked up 
        /// in the new porfolio screen
        /// </summary>
        public bool NeedsToProcessCashTransfers
        {
            get
            {
                bool retVal = false;
                if (status < RebalanceInstructionStati.Rebalance)
                    retVal = true;
                return retVal;
            }
        }

        /// <summary>
        /// When this instruction [originates from/belongs to] a withdrawal
        /// </summary>
        public virtual ICashWithdrawalInstruction CashWithdrawalInstruction { get; set; }

        /// <summary>
        /// The instruments that this assetmanager is trading in.
        /// </summary>
        public virtual IRebalanceExclusionCollection ExcludedComponents
        {
            get
            {
                RebalanceExclusionCollection items = (RebalanceExclusionCollection)this.excludedComponents.AsList();
                if (items.Parent == null)
                    items.Parent = this;
                return items;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// The type of instruction.
        /// </summary>
        public override InstructionTypes InstructionType
        {
            get { return InstructionTypes.Rebalance; }
        }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        public override string ToString()
        {
            return "RebalanceInstruction for " + Account.ToString() + " -> " + " status: " + Status.ToString();
        }

        #endregion

        #region Private Variables

        private IDomainCollection<IRebalanceExclusion> excludedComponents;

        #endregion

        #region IStateMachineClient Members

        public override string CheckCondition(int conditionID)
        {
            string retVal = "None";
            RebalanceInstructionConditions condition = (RebalanceInstructionConditions)conditionID;

            switch (condition)
            {
                case RebalanceInstructionConditions.CheckSizeBasedClose:
                    if (checkPortfolioValueZero())
                        retVal = "Empty";
                    else
                    {
                        if (Engine.PerformCheck(InstructionEngineChecks.CheckSizeBaseCloseOrders, this))
                            retVal = "HitClose";
                        else if (checkPendingOrders(condition))
                            retVal = "Hit";
                    }
                    break;
                case RebalanceInstructionConditions.PendingRebalance:
                    if (checkPendingOrders(condition))
                        retVal = "Hit";
                    else
                    {
                        switch (checkForeignCurrencyPositions())
                        {
                            case CheckForCurPositionsReturnValue.NoCash:
                                retVal = "NoCash";
                                break;
                            case CheckForCurPositionsReturnValue.ForeignCurrencyPositionsFound:
                                retVal = "Hit";
                                break;
                            default: //CheckForCurPositionsReturnValue.CashFound
                                break;
                        }
                    }
                    break;
                case RebalanceInstructionConditions.CheckCancel:
                    if (checkCancel())
                        retVal = "OK";
                    break;
                default:
                    if (checkPendingOrders(condition))
                        retVal = "Hit";
                    break;
            }
            return retVal;
        }

        public override bool RunAction(int actionID)
        {
            switch ((RebalanceInstructionActions)actionID)
            {
                case RebalanceInstructionActions.SizeBasedCloseOrders:
                    Engine.PerformAction(InstructionEngineActions.PlaceSizeBaseCloseOrders, this);
                    break;
                case RebalanceInstructionActions.Rebalance:
                    Engine.PerformAction(InstructionEngineActions.RunRebalance, this);
                    break;
                case RebalanceInstructionActions.CashFundOrders:
                    Engine.PerformAction(InstructionEngineActions.PlaceCashFundOrders, this);
                    break;
                case RebalanceInstructionActions.Terminate:
                    terminateInstruction();
                    break;
                case RebalanceInstructionActions.Cancel:
                    cancelInstruction();
                    break;
                default:
                    break;
            }
            return true;
        }

        #endregion

        #region Conditions

        private bool checkPortfolioValueZero()
        {
            bool retVal = false;
            Money total = null;

            // Check whether the Account holds any Cash or Positions
            IPortfolio portfolio = Account.Portfolio;
            if (portfolio.PortfolioCashGL.Count > 0 || portfolio.PortfolioInstrument.Count > 0)
            {
                total = portfolio.TotalValue();
                if (total == null || total.IsZero || !total.Sign)
                    retVal = true;
            }
            else
                retVal = true;

            if (retVal)
            {
                if (total != null && !total.Sign)
                    Message = string.Format("The account {0} does have a negative portfolio value and thus can not be rebalanced.", Account.DisplayNumberWithName);
                else
                    Message = string.Format("The account {0} does not have any cash or positions and thus can not be rebalanced.", Account.DisplayNumberWithName);
                Warning = true;
            }
            return retVal;
        }

        private bool checkPendingOrders(RebalanceInstructionConditions condition)
        {
            bool retVal = false;
            string action = "";
            string orderType = "";
            //IAccountOrderCollection orders = null;
            int orderCount = 0;

            switch (condition)
            {
                case RebalanceInstructionConditions.PendingRebalance:
                    orderCount = ( Account.OpenOrdersForAccount.Exclude(ExcludedComponents.Instruments)).Count; 
                    action = "placing of the cash management orders";
                    break;
                case RebalanceInstructionConditions.PendingCashFund:
                    orderCount = ActiveOrders.Count; 
                    action = "terminating the instruction";
                    break;
                default:
                    orderCount = (Account.OpenOrdersForAccount.Filter(OrderTypes.SizeBased, OrderSideFilter.All).Exclude(ExcludedComponents.Instruments)).Count; 
                    action = "rebalance";
                    orderType = "size based";
                    break;
            }

            // Check for pending Orders
            if (orderCount > 0)
            {
                Message = string.Format("{0} pending {1} orders prevent the {2}", orderCount.ToString(), orderType, action);
                retVal = true;
            }
            return retVal;
        }

        private CheckForCurPositionsReturnValue checkForeignCurrencyPositions()
        {
            CheckForCurPositionsReturnValue retVal = CheckForCurPositionsReturnValue.NoCash;

            // Check whether Foreign Currency Positions exist in the portfolio
            if (Account.Portfolio != null)
            {
                ICashPortfolio portfolio = Account.Portfolio.PortfolioCashGL;
                if (portfolio != null && portfolio.Count > 0)
                {
                    foreach (ICashPosition pos in portfolio)
                    {
                        if (pos.SettledSize.IsNotZero)
                        {
                            retVal = CheckForCurPositionsReturnValue.CashFound;
                            if (!((ICurrency)pos.PositionCurrency).IsBase)
                            {
                                Message = string.Format("A foreign currency positions was found ({0}), close it first before the Cash Fund Order can be entered.", pos.SettledSize.DisplayString);
                                retVal = CheckForCurPositionsReturnValue.ForeignCurrencyPositionsFound;
                                Warning = true;
                                break;
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        private bool checkCancel()
        {
            // A rebalance instruction can always be cancelled
            bool retVal = true;
            cancelAttachedOrders();

            //// Check whether instruction can be cancelled
            //switch (Status)
            //{
            //    case InstructionStati.New:
            //        retVal = true;
            //        break;
            //    case InstructionStati.PendingOrders:
            //        retVal = true;
            //        break;
            //    case InstructionStati.Rebalance:
            //    case InstructionStati.PendingRebalanceOrders:
            //        retVal = cancelAllRebalanceOrders();
            //        if (!retVal)
            //            Message = string.Format("The instruction for account {0} can no longer be cancelled since the rebalance orders are already approved.", Account.DisplayNumberWithName);
            //        break;
            //    default:
            //        Message = string.Format("The instruction for account {0} can no longer be cancelled", Account.DisplayNumberWithName);
            //        break;
            //}
            return retVal;
        }

        #endregion

    }
}
