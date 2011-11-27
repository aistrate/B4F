using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.PortfolioComparer;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.ApplicationLayer.Fee;
using B4F.TotalGiro.Utils.Tuple;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// The InstructionEngine class is used to manage the Account Rebalance Instructions
    /// </summary>
    public class InstructionEngine: IInstructionEngine
	{
        #region Constructor

        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.InstructionEngine">InstructionEngine</see> class.
        /// </summary>
        public InstructionEngine()
		{
            this.stateMachine = new XMLStateMachine();
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Instructions.InstructionEngine">InstructionEngine</see> class.
        /// </summary>
        /// <param name="engineParams">Parameters used in the instruction engine</param>
        public InstructionEngine(InstructionEngineParameters engineParams)
            :this()
		{
            this.engineParams = engineParams;
		}

        #endregion

        /// <summary>
        /// An instance of the <see cref="T:B4F.TotalGiro.Fees.FeeFactory">FeeFactory</see> class
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        protected IFeeFactory getFeeFactory(FeeFactoryInstanceTypes instanceType)
        {
            if (this.feeFactory == null || (this.feeFactory != null && !this.feeFactory.IsInstanceTypeActivated(instanceType)))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                if (this.feeFactory == null)
                    this.feeFactory = FeeFactory.GetInstance(session, instanceType, true);
                else
                    this.feeFactory.InitiateInstance(session, instanceType, true);
            }
            return this.feeFactory;
        }

		/// <summary>
		/// This method processe the instruction to the next level until it is finished
		/// </summary>
        /// <param name="instruction">The <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">Instruction</see> to process</param>
		/// <returns>true when succesfull</returns>
        public bool ProcessInstruction(IInstruction instruction)
		{
            if (instruction.ExecutionDate.Date > DateTime.Now.Date)
                return false;

            int oldStatus = instruction.Status;
            string oldMessage = instruction.Message;
			int nextStatus;

            instruction.Engine = this;
            nextStatus = stateMachine.SetStatus(getStateTable(instruction.InstructionType), (int)oldStatus, (int)InstructionEvents.evNext, (IStateMachineClient)instruction);
            instruction.Engine = null;
            if (nextStatus != 0 && oldStatus.Equals(instruction.Status))
                instruction.Status = nextStatus;

            if (Util.IsNullDate(instruction.ActualExecutedDate))
                instruction.ActualExecutedDate = DateTime.Now.Date;

            return (instruction.Status != oldStatus || instruction.Message != oldMessage || (instruction.UpdateableOrders != null && instruction.UpdateableOrders.Count > 0));
        }

        /// <summary>
        /// This method tries to cancel the instruction (if allowed).
        /// </summary>
        /// <param name="instruction">The <see cref="T:B4F.TotalGiro.Accounts.Instructions.Instruction">Instruction</see> to process</param>
        /// <param name="cancelOrders">The cancelled orders that might result from the processing</param>
        /// <returns>true when succesfull</returns>
        public bool CancelInstruction(IInstruction instruction)
        {
            int oldStatus = instruction.Status;
            string oldMessage = instruction.Message;
            int nextStatus;

            nextStatus = stateMachine.SetStatus(getStateTable(instruction.InstructionType), (int)oldStatus, (int)InstructionEvents.evCancel, (IStateMachineClient)instruction);
            if (nextStatus != 0)
                instruction.Status = nextStatus;
            return (instruction.Status != oldStatus || instruction.Message != oldMessage);
        }

        private Stream getStateTable(InstructionTypes instructionType)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyname = assembly.ToString().Substring(0, assembly.ToString().IndexOf(","));
            const string subPath = ".Instructions.Engine.StateMachineSchemas.";
            Stream stateTable = null;
            switch (instructionType)
            {
                case InstructionTypes.Rebalance:
                    stateTable = assembly.GetManifestResourceStream(assemblyname + subPath + "RebalanceInstructionStates.xml");
                    break;
                case InstructionTypes.BuyModel:
                    stateTable = assembly.GetManifestResourceStream(assemblyname + subPath + "BuyModelInstructionStates.xml");
                    break;
                case InstructionTypes.CashWithdrawal:
                    stateTable = assembly.GetManifestResourceStream(assemblyname + subPath + "CashWithdrawalInstructionStates.xml");
                    break;
                case InstructionTypes.ClientDeparture:
                    stateTable = assembly.GetManifestResourceStream(assemblyname + subPath + "ClientDepartureInstructionStates.xml");
                    break;
            }
            return stateTable;
        }

        #region IInstructionEngine Members

        #region Conditions

        public bool PerformCheck(InstructionEngineChecks check, IInstruction instruction)
        {
            switch (check)
            {
                case InstructionEngineChecks.CheckSizeBaseCloseOrders:
                    return checkSizeBasedCloseOrders(instruction);
                    break;
                default:
                    return false;
                    break;
            }
        }

        private bool checkSizeBasedCloseOrders(IInstruction instruction)
        {
            if (instruction.InstructionType == InstructionTypes.BuyModel)
                return false;

            // Check whether instruments exist in the portfolio that do not exist in the modelportfolio
            PortfolioCompareSetting setting = new PortfolioCompareSetting(PortfolioCompareAction.CloseOrders, instruction, this.engineParams);
            PortfolioComparer.PortfolioComparer comparer = new PortfolioComparer.PortfolioComparer(setting, null);
            return comparer.CheckInstrumentsNotInModel();
        }

        #endregion

        #region Actions

        public void PerformAction(InstructionEngineActions action, IInstruction instruction)
        {
            RebalanceResults result;
            PortfolioCompareAction pfAction;
            string instructionMessage = "";
            string tempMessage = "";

            switch (action)
            {
                case InstructionEngineActions.PlaceSizeBaseCloseOrders:
                    pfAction = PortfolioCompareAction.CloseOrders;
                    break;
                case InstructionEngineActions.RunRebalance:
                    pfAction = PortfolioCompareAction.Rebalance;
                    break;
                case InstructionEngineActions.BuyModel:
                    pfAction = PortfolioCompareAction.BuyModel;
                    break;
                case InstructionEngineActions.PlaceCashFundOrders:
                    pfAction = PortfolioCompareAction.CashFundOrders;
                    break;
                case InstructionEngineActions.PlaceFreeUpCashFundOrder:
                    placeCashFundOrder((ICashWithdrawalInstruction)instruction);
                    return;
                case InstructionEngineActions.CreateFreeUpCashRebalanceInstruction:
                    createRebalanceInstruction((ICashWithdrawalInstruction)instruction);
                    return;
                case InstructionEngineActions.CreateMoneyTransferOrder:
                    createMoneyTransferOrder((ICashWithdrawalInstruction)instruction);
                    return;
                case InstructionEngineActions.LiquidatePortfolio:
                    liquidatePortfolio((IClientDepartureInstruction)instruction);
                    return;
                case InstructionEngineActions.SettleAccount:
                    settleAccount((IClientDepartureInstruction)instruction);
                    return;
                case InstructionEngineActions.TransferAllCash:
                    transferAllCash((IClientDepartureInstruction)instruction);
                    return;
                case InstructionEngineActions.TerminateAccount:
                    terminateAccount((IClientDepartureInstruction)instruction);
                    return;
                default:
                    throw new ApplicationException("Unknown action");
            }

            PortfolioCompareSetting setting = new PortfolioCompareSetting(pfAction, instruction, this.engineParams);
            PortfolioComparer.PortfolioComparer comparer = new PortfolioComparer.PortfolioComparer(setting, getFeeFactory(FeeFactoryInstanceTypes.Commission));

            IList updateableOrders = comparer.CompareToModel(out result);
            instruction.SetInstructionMessage((int)result);

            if (updateableOrders != null && updateableOrders.Count > 0)
            {
                switch (action)
                {
                    case InstructionEngineActions.PlaceSizeBaseCloseOrders:
                        instructionMessage = string.Format("{0} sizebased orders were created for instruments that are not in the modelportfolio.", updateableOrders.Count.ToString());
                        break;
                    case InstructionEngineActions.RunRebalance:
                    case InstructionEngineActions.BuyModel:
                        tempMessage = getNumberofMonetaryOrders(updateableOrders, true);
                        if (tempMessage != "0")
                            tempMessage = string.Format(" and {0} monetary order(s)", tempMessage);
                        else
                            tempMessage = string.Empty;
                        instructionMessage = string.Format("{0} amountbased orders {1} are created during rebalance", getNumberofMonetaryOrders(updateableOrders, false), tempMessage);
                        break;
                    case InstructionEngineActions.PlaceCashFundOrders:
                        instructionMessage = string.Format("{0} cash management fund order(s) were created.", updateableOrders.Count.ToString());
                        break;
                }
                instruction.UpdateableOrders = updateableOrders;
                instruction.Message = instructionMessage;
            }
        }

        private bool placeCashFundOrder(ICashWithdrawalInstruction instruction)
        {
            ITradeableInstrument cashFund = null;
            IFundPosition posCashFund = null;
            Money cashAmount = null;
            bool retVal = false;

            // Get the modelinstruments
            IModelVersion mv = instruction.Account.ModelPortfolio.LatestVersion;
            if (mv == null)
                throw new ApplicationException(string.Format("Not possible to place cash fund orders for account {0}: Latest ModelVersion is null", instruction.Account.DisplayNumberWithName));

            cashFund = mv.GetCashFundOrAlternative();
            if (cashFund != null)
                posCashFund = instruction.Account.Portfolio.PortfolioInstrument.GetPosition(cashFund);

            // Deduct the cash on the account
            ICurrency baseCurrency = instruction.Account.AccountOwner.StichtingDetails.BaseCurrency;
            ICashSubPosition cashPos = instruction.Account.Portfolio.PortfolioCashGL.GetSettledBaseSubPosition();
            if (cashPos != null)
                cashAmount = cashPos.Size;

            if (posCashFund != null && posCashFund.Size.IsGreaterThanZero && Money.Add(posCashFund.CurrentBaseValue + cashAmount, instruction.Amount).IsGreaterThanOrEqualToZero)
            {
                OrderAmountBased order = new OrderAmountBased(instruction.Account, instruction.Amount + cashAmount, cashFund, true, getFeeFactory(FeeFactoryInstanceTypes.Commission), instruction.DoNotChargeCommission, OrderActionTypes.Withdrawal);
                order.Instruction = instruction;
                order.OrderInfo = string.Format("Free up cash for cash withdrawal ({0}) on {1}", instruction.Amount.DisplayString, instruction.WithdrawalDate.ToString("dd-MM-yyyy"));
                IList orders = new ArrayList();
                orders.Add(order);
                instruction.UpdateableOrders = orders;
                instruction.Message = string.Format("Cash Fund order generated to free up cash on {0}", DateTime.Now.ToString());
                retVal = true;
            }
            else
                createRebalanceInstruction(instruction);

            return retVal;
        }

        private bool createRebalanceInstruction(ICashWithdrawalInstruction instruction)
        {
            bool retVal = false;

            // check -> does the account have a positive amount
            if (Money.Add(instruction.Account.Portfolio.TotalValue(), instruction.Amount).IsLessThanZero)
            {
                instruction.Message = "The account does not have enough buying power to finance this withdrawal.";
                instruction.Warning = true;
            }
            else
            {
                try
                {
                    IRebalanceInstruction rebalance = (IRebalanceInstruction)instruction.Account.CreateInstruction(InstructionTypes.Rebalance, OrderActionTypes.Withdrawal, DateTime.Today, instruction.DoNotChargeCommission);
                    rebalance.CashWithdrawalInstruction = instruction;
                    instruction.Message = "A rebalance instruction was created to free up cash";
                    retVal = true;
                }
                catch (Exception ex)
                {
                    instruction.Message = "It was not possible to create a rebalance instruction to free up cash. " + ex.Message;
                    instruction.Warning = true;
                }
            }
            return retVal;
        }

        private bool createMoneyTransferOrder(ICashWithdrawalInstruction instruction)
        {
            bool createMoneyOrder = false;
            if (Money.Add(instruction.Account.TotalCash, instruction.Amount).IsGreaterThanOrEqualToZero)
                createMoneyOrder = true;
            else
            {
                switch (instruction.CheckAccountCash(CashWithdrawalInstructionActions.PlaceFreeUpCashFundOrder))
                {
                    case CashWithdrawalInstructionCheckCashReturnValues.PlaceCashFundOrder:
                        instruction.Warning = true;
                        placeCashFundOrder(instruction);
                        createMoneyOrder = true;
                        break;
                    case CashWithdrawalInstructionCheckCashReturnValues.CreateRebalanceInstruction:
                        createRebalanceInstruction(instruction);
                        instruction.Warning = true;
                        instruction.Status = (int)CashWithdrawalInstructionStati.New;
                        break;
                    case CashWithdrawalInstructionCheckCashReturnValues.NotEnoughBuyingPower:
                        instruction.Message = "Not enough buying power to create the Money Transfer order.";
                        instruction.Warning = true;
                        break;
                }
            }

            if (createMoneyOrder)
            {
                IMoneyTransferOrder moneyOrder = new MoneyTransferOrder(instruction);
                string info = "";
                if (instruction.UpdateableOrders != null && instruction.UpdateableOrders.Count > 0)
                    info = " and a Cash Fund order";
                else if (instruction.Account.ActiveRebalanceInstructions.Count > 0)
                {
                    foreach (IInstruction rebInst in instruction.Account.ActiveRebalanceInstructions)
                    {
                        if (rebInst.Key == 0)
                            info = " and a Rebalance Instruction";
                    }
                }
                instruction.Message = string.Format("Money Transfer order{0} created on {1}", info, DateTime.Now.ToString("yyyy-MM-dd hh:mm"));
                instruction.Warning = (info != "");
            }            
            return true;
        }

        private bool liquidatePortfolio(IClientDepartureInstruction instruction)
        {
            bool retVal = false;
            bool sizeBasedOrdersExits = false;
            IList orders = new ArrayList();

            IList<IOrder> openOrders = instruction.Account.OpenOrdersForAccount;

            if (openOrders.Any(x => x.IsAmountBased))
                throw new ApplicationException(string.Format("The account {0} does have pending amount based orders and thus can not be liquidated.", instruction.Account.DisplayNumberWithName));

            sizeBasedOrdersExits = openOrders.Any(x => x.IsSizeBased);
            if (instruction.Account.Get(e => e.Portfolio).Get(e => e.PortfolioInstrument) != null)
            {
                foreach (IFundPosition pos in instruction.Account.Portfolio.PortfolioInstrument.ExcludeNonTradeableInstruments().Where(x => x.Size.IsNotZero))
                {
                    InstrumentSize openOrderSize = null;
                    if (sizeBasedOrdersExits)
                        openOrderSize = openOrders
                            .Where(x => x.RequestedInstrument.Key == pos.Instrument.Key)
                            .Select(x => x.OpenValue).Sum();
                    
                    InstrumentSize orderSize = pos.Size.Negate() - openOrderSize;
                    if (orderSize.IsNotZero)
                    {
                        IOrder order = new OrderSizeBased(instruction.Account, orderSize, true, getFeeFactory(FeeFactoryInstanceTypes.Commission), instruction.DoNotChargeCommission, OrderActionTypes.Departure);
                        order.Instruction = instruction;
                        order.OrderInfo = string.Format("Portfolio liquidated on {0}", DateTime.Now.ToString());
                        orders.Add(order);
                    }
                }

                if (orders.Count > 0)
                {
                    instruction.UpdateableOrders = orders;
                    instruction.Message = string.Format("{0} orders generated to liquidate portfolio on {1}", 
                        orders.Count, DateTime.Now.ToString());
                    retVal = true;
                }
            }
            return retVal;
        }

        private bool settleAccount(IClientDepartureInstruction instruction)
        {
            bool mgtFeeCharged = false;
            instruction.Message = "";

            if (instruction.CounterAccount == null)
            {
                instruction.Message = "Could not settle the account, look up the counter account.";
                return false;
            }
            
            IManagementPeriod managementPeriod = instruction.Account.CurrentManagementFeePeriod;
            if (managementPeriod != null)
            {
                IList<IManagementPeriodUnit> unitsNotCharged = managementPeriod.ManagementPeriodUnits
                    .Where(x => x.ManagementFee == null)
                    .OrderByDescending(x => x.StartDate)
                    .ToList();

                Dictionary<int, Tuple<int, int>> quarterYears = new Dictionary<int, Tuple<int, int>>();
                if (unitsNotCharged != null && unitsNotCharged.Count > 0)
                {
                    BatchExecutionResults results = new BatchExecutionResults();
                    IManagementPeriodUnit lastUnit = unitsNotCharged.FirstOrDefault();
                    if (lastUnit != null && lastUnit.EndDate.Equals(managementPeriod.EndDate))
                    {
                        // get all quarter periods not yet charged
                        foreach (IManagementPeriodUnit unit in unitsNotCharged)
                        {
                            Tuple<int, int> quarterYear = new Tuple<int, int>(Util.GetQuarter(unit.EndDate), unit.EndDate.Year);
                            int key = quarterYear.Item2 * 100 + quarterYear.Item1;
                            if (!quarterYears.ContainsKey(key))
                                quarterYears.Add(key, quarterYear);
                        }

                        if (quarterYears.Count() == 0)
                        {
                            instruction.Message = "Could not charge management fee.";
                            return false;
                        }
                        else if (quarterYears.Count() == 1)
                        {
                            mgtFeeCharged = ManagementFeeOverviewAdapter.CreateMgtFeeTransactions(results, new int[] { lastUnit.ManagementPeriod.Key }, quarterYears.Values.First().Item2, quarterYears.Values.First().Item1, B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.ManagementFee, false);
                        }
                        else if (quarterYears.Count() > 1)
                        {
                            // Only charge last 2
                            // check if last 2 are chronological
                            int maxItemKey = quarterYears.Keys.OrderByDescending(x => x).ElementAt(0);
                            int prevItemKey = quarterYears.Keys.OrderByDescending(x => x).ElementAt(1);
                            Tuple<int, int> maxQuarterYear = quarterYears[maxItemKey];
                            if (Util.GetPreviousQuarterYear(maxQuarterYear.Item1, maxQuarterYear.Item2).Equals(quarterYears[prevItemKey]))
                                ManagementFeeOverviewAdapter.CreateMgtFeeTransactions(results, new int[] { lastUnit.ManagementPeriod.Key }, quarterYears[prevItemKey].Item2, quarterYears[prevItemKey].Item1, B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.ManagementFee, false);

                            mgtFeeCharged = ManagementFeeOverviewAdapter.CreateMgtFeeTransactions(results, new int[] { lastUnit.ManagementPeriod.Key }, quarterYears[maxItemKey].Item2, quarterYears[maxItemKey].Item1, B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.ManagementFee, false);
                        }
                        instruction.Message = ManagementFeeOverviewAdapter.FormatErrorsForCreateMgtFeeTransactions(results).Replace("<br/>", "");
                    }
                    else
                    {
                        // check if lastUnit is charged
                        IManagementPeriodUnit lastChargedUnit = managementPeriod.ManagementPeriodUnits
                            .Where(x => x.ManagementFee != null)
                            .OrderByDescending(x => x.StartDate)
                            .FirstOrDefault();
                        if (lastChargedUnit != null && lastChargedUnit.EndDate.Equals(managementPeriod.EndDate))
                        {
                            if (lastUnit == null)
                                mgtFeeCharged = true;
                            else
                            {
                                Tuple<int, int> lastQY = new Tuple<int, int>(Util.GetQuarter(lastChargedUnit.EndDate), lastChargedUnit.EndDate.Year);
                                Tuple<int, int> unChargedQY = new Tuple<int, int>(Util.GetQuarter(lastUnit.EndDate), lastUnit.EndDate.Year);
                                if (Util.GetPreviousQuarterYear(lastQY.Item1, lastQY.Item2).Equals(unChargedQY))
                                    instruction.Message = "The previous quarter {0}{1} has not been charged";
                                else
                                    mgtFeeCharged = true;
                            }
                        }
                    }
                }
            }
            else
                mgtFeeCharged = true;

            if (mgtFeeCharged)
            {
                transferAllCash(instruction);
            }
            return true;
        }

        private bool transferAllCash(IClientDepartureInstruction instruction)
        {
            bool success = false;
            // Create MoneyTransferOrder
            Money totalValue = instruction.Account.TotalAll;
            if (totalValue.IsGreaterThanZero && instruction.MoneyTransferOrder == null)
            {
                if (instruction.Account.Portfolio.PortfolioInstrument.Where(x => x.Size.IsNotZero).Count() > 0)
                    instruction.Message += "Active positions still exist. Cancel this instruction and create a new one.";
                else
                {
                    Money moneyOrderAmount = instruction.Account.ActiveMoneyTransferOrders.Select(x => x.Amount).Sum();
                    if ((totalValue - moneyOrderAmount).IsGreaterThanZero)
                    {
                        IMoneyTransferOrder moneyOrder = new MoneyTransferOrder(instruction, totalValue);
                        instruction.MoneyTransferOrder = moneyOrder;
                        instruction.Message += string.Format("Money Transfer order created ({0})", moneyOrder.Amount.DisplayString);
                        success = true;
                    }
                    else
                        instruction.Message += string.Format("Money Transfer not possible since active moneyorders already exist ({0})", moneyOrderAmount.DisplayString);
                }
            }
            return success;
        }

        private bool terminateAccount(IClientDepartureInstruction instruction)
        {
            DateTime finalEndDate = instruction.Account.FinalManagementEndDate;
            if (Util.IsNullDate(finalEndDate))
                finalEndDate = DateTime.Today;

            IPortfolioModel closedModel = ((IAssetManager)instruction.Account.AccountOwner).ClosedModelPortfolio;
            if (closedModel != null)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IInternalEmployeeLogin employee = (IInternalEmployeeLogin)LoginMapper.GetCurrentLogin(session);
                    instruction.Account.SetModelPortfolio(closedModel, employee, finalEndDate.AddDays(1));
                }
            }
            instruction.Account.LastDateStatusChanged = DateTime.Now;
            instruction.Account.ValuationsEndDate = finalEndDate;
            instruction.Account.Status = AccountStati.Inactive;
            return true;
        }

        #endregion

        #region Helpers

        private string getNumberofMonetaryOrders(IList orders, bool isMonetary)
        {
            int count = 0;
            foreach (IOrder order in orders)
            {
                if (order.IsMonetary == isMonetary)
                    count++;
            }
            return count.ToString();
        }

        #endregion

        #endregion

        #region Privates

        private IFeeFactory feeFactory;
        private XMLStateMachine stateMachine;
        private InstructionEngineParameters engineParams;

        #endregion
    }
}
