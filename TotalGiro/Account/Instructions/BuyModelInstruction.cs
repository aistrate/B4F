using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// This is a instruction used for buying the model (rebalance just the cash)
    /// </summary>
    public class BuyModelInstruction : InstructionTypeRebalance, IBuyModelInstruction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.BuyModelInstruction">BuyModelInstruction</see> class.
        /// </summary>
        protected BuyModelInstruction() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.BuyModelInstruction">BuyModelInstruction</see> class.
        /// </summary>
        /// <param name="account">The account the withdrawal will belong to</param>
        /// <param name="executionDate">The date the instruction should execute</param>
        /// <param name="orderActionType">The type of instruction that is placed on the orders</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        /// <param name="cashTransfers">The transfers involved</param>
        public BuyModelInstruction(IAccountTypeCustomer account, DateTime executionDate, OrderActionTypes orderActionType, bool doNotChargeCommission,
            IList<IJournalEntryLine> cashTransfers)
            : base(account, executionDate, orderActionType, doNotChargeCommission, cashTransfers)
        {
            //DepositCashPositionDifference = depositCashPositionDifference;
        }

        #endregion

        #region Properties

        public virtual Money DepositCashPositionDifference { get; set; }

        /// <summary>
        /// When successfull it returns the difference between the total transfer amount
        /// and the amount that actually is invested.
        /// </summary>
        /// <param name="diff">The difference</param>
        /// <returns>True when successfull</returns>
        public bool GetRoundingDifference(out Money diff)
        {
            bool success = false;
            diff = null;
            if (InstructionType == InstructionTypes.BuyModel && status >= RebalanceInstructionStati.Rebalance)
            {
                Money totalFilledAmount = null;
                foreach (IOrder order in this.Orders)
                {
                    success = true;
                    if (order.Status <= OrderStati.Checked)
                    {
                        success = false;
                        break;
                    }
                    else if (order.CancelStatus == OrderCancelStati.Cancelled || order.FilledValue == null || order.FilledValue.IsZero)
                    {
                        // Do Nothing -> To dangerous to fuck things up
                        diff = null;
                        return false;
                    }
                    else
                        // BuyModel so only amount based orders
                        totalFilledAmount += order.FilledValue.GetMoney();
                }

                diff = CashTransfers.TotalTransferAmount - totalFilledAmount;
            }
            return success;
        }

        #endregion

        #region overrides

        /// <summary>
        /// The type of instruction.
        /// </summary>
        public override InstructionTypes InstructionType
        {
            get { return InstructionTypes.BuyModel; }
        }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        public override string ToString()
        {
            return "BuyModelInstruction for " + Account.ToString() + " -> " + " status: " + Status.ToString();
        }

        #endregion

        #region Private Variables

        #endregion

        #region IStateMachineClient Members

        public override string CheckCondition(int conditionID)
        {
            string retVal = "None";
            BuyModelInstructionConditions condition = (BuyModelInstructionConditions)conditionID;

            switch (condition)
            {
                case BuyModelInstructionConditions.EnoughBuyingPower:
                    if (checkPortfolioValueZero())
                        retVal = "Empty";
                    break;
                case BuyModelInstructionConditions.PendingOrders:
                    if (checkPendingOrders(condition))
                        retVal = "Hit";
                    else
                    {
                        switch (checkCashLeft())
                        {
                            case CheckForCurPositionsReturnValue.NoCash:
                                retVal = "NoCash";
                                break;
                            default: //CheckForCurPositionsReturnValue.CashFound
                                break;
                        }
                    }
                    break;
                case BuyModelInstructionConditions.CheckCancel:
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
            switch ((BuyModelInstructionActions)actionID)
            {
                case BuyModelInstructionActions.BuyModel:
                    Engine.PerformAction(InstructionEngineActions.BuyModel, this);
                    break;
                case BuyModelInstructionActions.CashFundOrders:
                    Engine.PerformAction(InstructionEngineActions.PlaceCashFundOrders, this);
                    break;
                case BuyModelInstructionActions.Terminate:
                    terminateInstruction();
                    break;
                case BuyModelInstructionActions.Cancel:
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

            if (CashTransfers == null || CashTransfers.Count == 0 ||
                CashTransfers.TotalTransferAmount == null || CashTransfers.TotalTransferAmount.IsLessThanZero ||
                CashTransfers.TotalTransferAmount.IsZero)
                retVal = true;
            return retVal;
        }

        private bool checkPendingOrders(BuyModelInstructionConditions condition)
        {
            bool retVal = false;
            string action = "";

            switch (condition)
            {
                case BuyModelInstructionConditions.PendingOrders:
                    action = "placing of the cash management orders";
                    break;
                case BuyModelInstructionConditions.PendingCashFund:
                    action = "terminating the instruction";
                    break;
            }

            // Check for pending Orders
            IAccountOrderCollection orders = Account.OpenOrdersForAccount;
            if (orders != null && orders.Count > 0)
            {
                Message = string.Format("{0} pending orders prevent the {1}", orders.Count.ToString(), action);
                retVal = true;
            }
            return retVal;
        }

        private CheckForCurPositionsReturnValue checkCashLeft()
        {
            CheckForCurPositionsReturnValue retVal = CheckForCurPositionsReturnValue.NoCash;

            Money totalValue;
            if (GetRoundingDifference(out totalValue) &&
                totalValue != null && totalValue.IsNotZero)
                retVal = CheckForCurPositionsReturnValue.CashFound;

            return retVal;
        }

        private bool checkCancel()
        {
            cancelAttachedOrders();
            return true;
        }


        #endregion

    }
}
