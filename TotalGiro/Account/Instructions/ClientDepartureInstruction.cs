using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Instructions.Exclusions;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.BackOffice.Orders;

namespace B4F.TotalGiro.Accounts.Instructions
{
    public class ClientDepartureInstruction : InstructionTypeRebalance, IClientDepartureInstruction
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.RebalanceInstruction">RebalanceInstruction</see> class.
        /// </summary>
        protected ClientDepartureInstruction() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.ClientDepartureInstruction">ClientDepartureInstruction</see> class.
        /// </summary>
        /// <param name="account">The account the withdrawal will belong to</param>
        /// <param name="executionDate">The date the instruction should execute</param>
        /// <param name="doNotChargeCommission">The instruction is without any charges</param>
        /// <param name="counterAccount">counter Account</param>
        /// <param name="transferDescription">transfer Description</param>
        public ClientDepartureInstruction(IAccountTypeCustomer account, DateTime executionDate, bool doNotChargeCommission, ICounterAccount counterAccount, string transferDescription)
            : base(account, executionDate, OrderActionTypes.Departure, doNotChargeCommission, null)
        {
            this.CounterAccount = counterAccount;
            this.TransferDescription = transferDescription;
            this.status = ClientDepartureInstructionStati.New;
        }

        #endregion

        #region props

        public virtual IMoneyTransferOrder MoneyTransferOrder { get; set; }
        public virtual string TransferDescription { get; set; }

        public virtual ICounterAccount CounterAccount
        {
            get
            {
                if (counterAccount == null)
                    return Account.CounterAccount;
                else
                    return counterAccount;
            }
            set
            {
                //if (value == null && (Account == null || Account.CounterAccount == null))
                //    throw new ApplicationException(string.Format("The account {0} does not have a default counter account.", Account.DisplayNumberWithName));

                counterAccount = value;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// The type of instruction.
        /// </summary>
        public override InstructionTypes InstructionType
        {
            get { return InstructionTypes.ClientDeparture; }
        }

        public override int Status
        {
            get { return (int)status; }
            set { status = (ClientDepartureInstructionStati)value; }
        }

        /// <exclude/>
        public override string DisplayStatus
        {
            get
            {
                return Status.ToString() + "-" + ((ClientDepartureInstructionStati)Status).ToString();
            }
        }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        public override string ToString()
        {
            return "Departure for " + Account.ToString() + " -> " + " status: " + Status.ToString();
        }

        #endregion

        #region Private Variables

        private ClientDepartureInstructionStati status;
        private ICounterAccount counterAccount;

        #endregion

        #region IStateMachineClient Members

        public override string CheckCondition(int conditionID)
        {
            string retVal = "None";
            ClientDepartureInstructionConditions condition = (ClientDepartureInstructionConditions)conditionID;

            switch (condition)
            {
                case ClientDepartureInstructionConditions.CheckNeedForCloseOrders:
                    if (checkPortfolioValueZero(false))
                        retVal = "Empty";
                    else
                    {
                        switch (checkNeedForCloseOrders())
                        {
                            case checkNeedForCloseOrdersReturnValues.PlaceSizeBasedCloseOrders:
                                retVal = "Hit";
                                break;
                            case checkNeedForCloseOrdersReturnValues.OrdersAlreadyExist:
                                retVal = "OrdersAlreadyExist";
                                break;
                            case checkNeedForCloseOrdersReturnValues.AllCash:
                                retVal = "AllCash";
                                break;
                            default: //CheckForCurPositionsReturnValue.CashFound
                                break;
                        }
                    }
                    break;
                case ClientDepartureInstructionConditions.PendingOrders:
                    switch (checkPendingOrders())
                    {
                        case checkPendingOrdersReturnValues.None:
                            retVal = "None";
                            break;
                        case checkPendingOrdersReturnValues.PortfolioValueZero:
                            retVal = "NoCash";
                            break;
                        default:
                            retVal = "Hit";
                            break;
                    }
                    break;
                case ClientDepartureInstructionConditions.IsValuated:
                    if (checkIsValuated())
                        retVal = "Yes";
                    else
                        retVal = "No";
                    break;
                case ClientDepartureInstructionConditions.CheckPendingCashOrder:
                    if (checkPortfolioValueZero(true))
                        retVal = "OK";
                    else
                    {
                        if (this.MoneyTransferOrder == null)
                            retVal = "CreateMoneyOrder";
                        else
                        {
                            switch (checkPendingMoneyTransferOrder())
                            {
                                case MoneyTransferOrderStati.Cancelled:
                                    retVal = "Cancel";
                                    break;
                            }
                        }
                    }
                    break;
                case ClientDepartureInstructionConditions.CheckCancel:
                    if (checkCancel())
                        retVal = "OK";
                    break;
            }
            return retVal;
        }

        public override bool RunAction(int actionID)
        {
            switch ((ClientDepartureInstructionActions)actionID)
            {
                case ClientDepartureInstructionActions.SizeBasedCloseOrders:
                    Engine.PerformAction(InstructionEngineActions.LiquidatePortfolio, this);
                    break;
                case ClientDepartureInstructionActions.SetMgtEndDates:
                    setMgtEndDates();
                    break;
                case ClientDepartureInstructionActions.SettleAccount:
                    Engine.PerformAction(InstructionEngineActions.SettleAccount, this);
                    break;
                case ClientDepartureInstructionActions.TransferAllCash:
                    Engine.PerformAction(InstructionEngineActions.TransferAllCash, this);
                    break;
                case ClientDepartureInstructionActions.Terminate:
                    terminateInstruction();
                    break;
                case ClientDepartureInstructionActions.Cancel:
                    cancelInstruction();
                    break;
                default:
                    break;
            }
            return true;
        }

        #endregion

        #region Conditions

        private bool checkPortfolioValueZero(bool checkValueOnly)
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

            if (retVal && !checkValueOnly)
            {
                if (total != null && !total.Sign)
                {
                    Message = string.Format("The account {0} does have a negative portfolio value and thus can not be rebalanced.", Account.DisplayNumberWithName);
                    Warning = true;
                }
                else
                {
                    Message = string.Format("The account {0} does not have any cash or positions and thus can not be rebalanced.", Account.DisplayNumberWithName);
                    Warning = true;
                }
            }
            return retVal;
        }

        private checkNeedForCloseOrdersReturnValues checkNeedForCloseOrders()
        {
            checkNeedForCloseOrdersReturnValues retVal = checkNeedForCloseOrdersReturnValues.AllCash;

            // Check whether the Account holds any Positions
            if (Account.Portfolio.PortfolioInstrument.ExcludeNonTradeableInstruments().Any(x => x.Size != null && x.Size.IsNotZero))
            {
                retVal = checkNeedForCloseOrdersReturnValues.PlaceSizeBasedCloseOrders;
                if (Account.OpenOrdersForAccount.Count > 0)
                {
                    if (Account.OpenOrdersForAccount.Any(x => x.IsAmountBased))
                    {
                        Message = string.Format("The account {0} does have pending amount based orders and thus can not be liquidated.", Account.DisplayNumberWithName);
                        retVal = checkNeedForCloseOrdersReturnValues.Error;
                    }
                    else
                    {
                        var p = Account.Portfolio.PortfolioInstrument.ExcludeNonTradeableInstruments()
                            .Where(x => x.Size.IsNotZero)
                            .Select(x => x.Size)
                            .Union(
                                Account.OpenOrdersForAccount
                                .Where(x => x.OpenValue.IsNotZero && x.Status == OrderStati.New)
                                .Select(x => x.OpenValue))
                            .GroupBy(x => x.Underlying.Key)
                            .Select(g =>
                            new
                            {
                                InstrumentID = g.Key,
                                PosSizeLeft = g.Sum()
                            })
                            .Where(a => a.PosSizeLeft.IsNotZero)
                            .ToArray();

                        if (p.Count() == 0)
                            retVal = checkNeedForCloseOrdersReturnValues.OrdersAlreadyExist;
                    }
                }
            }
            return retVal;
        }

        private checkPendingOrdersReturnValues checkPendingOrders()
        {
            checkPendingOrdersReturnValues retVal = checkPendingOrdersReturnValues.Hit;
            int orderCount = Account.OpenOrdersForAccount.Count;

            // Check for pending Orders
            if (orderCount == 0)
            {
                int unClosedPositions = Account.Portfolio.PortfolioInstrument.ExcludeNonTradeableInstruments().Where(x => x.Size.IsNotZero).Count();
                if (unClosedPositions > 0)
                    Message = string.Format("{0} unclosed positions found.", unClosedPositions);
                else
                    retVal = checkPendingOrdersReturnValues.None;

                if (checkPortfolioValueZero(true))
                {
                    retVal = checkPendingOrdersReturnValues.PortfolioValueZero;
                    Message = "Portfolio value is zero.";
                }
                else if (Account.Portfolio.PortfolioCashGL.Any(x => x.SettledSize.IsNotZero && !x.PositionCurrency.IsBase))
                {
                    retVal = checkPendingOrdersReturnValues.ForeignCurrencyPositions;
                    Message = "Close the foreign currency positions.";
                }
            }
            else if (orderCount > 0)
                Message = string.Format("{0} pending orders found.", orderCount);
            return retVal;
        }

        private bool checkIsValuated()
        {
            bool isOK = checkIsValuatedSub(Account.CurrentManagementFeePeriod) &&
                checkIsValuatedSub(Account.CurrentKickBackFeePeriod);
            if (isOK)
            {
                if (CounterAccount == null)
                {
                    Message = "Look up the counter account, not possible to settle.";
                    isOK = false;
                }
            }
            return isOK;
        }

        private bool checkIsValuatedSub(IManagementPeriod managementPeriod)
        {
            bool retVal = false;

            if (managementPeriod != null)
            {
                if (Util.IsNullDate(managementPeriod.EndDate))
                    Message = string.Format("The enddate is not set on management period {0}.", managementPeriod.Key);
                
                if (managementPeriod.ManagementType == ManagementTypes.ManagementFee)
                {
                    IManagementPeriodUnit lastUnit = managementPeriod.ManagementPeriodUnits
                        .OrderByDescending(x => x.StartDate).FirstOrDefault();
                    if (lastUnit != null)
                    {
                        if (lastUnit.EndDate.Equals(managementPeriod.EndDate) &&
                            lastUnit.AverageHoldings.Count > 0 &&
                            lastUnit.FeesCalculated == FeesCalculatedStates.Yes)
                            retVal = true;
                        else
                            if (Util.IsNotNullDate(Account.LastValuationDate))
                            {
                                int days = ((TimeSpan)(managementPeriod.EndDate - Account.LastValuationDate)).Days;
                                if (days > 0)
                                    Message = string.Format("Wait {0} more days", days);
                                else
                                    Message = string.Format("Management Fee needs to be calculated for {0} days", Math.Abs(days));
                            }
                            else
                                Message = "Account has no valuations???";
                    }
                }
            }
            else
                retVal = true;
            return retVal;
        }

        private MoneyTransferOrderStati checkPendingMoneyTransferOrder()
        {
            MoneyTransferOrderStati retVal = MoneyTransferOrderStati.New;

            if (MoneyTransferOrder != null)
            {
                return MoneyTransferOrder.Status;
            }
            return retVal;
        }

        private bool checkCancel()
        {
            // A rebalance instruction can always be cancelled
            return true;
        }

        #endregion

        #region Actions

        private void setMgtEndDates()
        {
            if (Account.Portfolio.Get(e => e.PortfolioInstrument).Any(x => x.Size.IsNotZero))
                throw new ApplicationException("Can not set final mgt end date, there are still open positions");

            DateTime lastTxDate = Account.Portfolio.LastTransactionDate;
            if (Util.IsNullDate(lastTxDate))
                throw new ApplicationException("Can not set final mgt end date, last transaction date is empty");

            Account.FinalManagementEndDate = lastTxDate;
            Message = string.Format("The enddate ({0}) has been set.", lastTxDate.ToString("dd-MM-yyyy"));
        }

        protected override void terminateInstruction()
        {
            this.Engine.PerformAction(InstructionEngineActions.TerminateAccount, this);
            base.terminateInstruction();
        }

        #endregion

        #region private enums

        enum checkNeedForCloseOrdersReturnValues
        {
            PlaceSizeBasedCloseOrders,
            AllCash,
            OrdersAlreadyExist,
            Error
        }

        enum checkPendingOrdersReturnValues
        {
            Hit,
            PortfolioValueZero,
            ForeignCurrencyPositions,
            None
        }

        #endregion
    }
}
