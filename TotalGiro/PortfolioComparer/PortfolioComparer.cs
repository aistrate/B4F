using System;
using System.Collections;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.PortfolioComparer
{
    /// <summary>
    /// The PortfolioComparer class is used to rebalance an account's portfolio
    /// </summary>
    public class PortfolioComparer
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.PortfolioComparer.PortfolioComparer">PortfolioComparer</see> class.
        /// </summary>
        /// <param name="setting">The settings used in the portfolio comparer</param>
        /// <param name="feeFactory">An instance of the <see cref="T:B4F.TotalGiro.Fees.FeeFactory">FeeFactory</see> class</param>
        public PortfolioComparer(PortfolioCompareSetting setting, IFeeFactory feeFactory)
		{
            if (setting == null)
                throw new ApplicationException("Can not do a Portfolio check without an initialized PortfolioCompareSetting object");
            if (setting.Instruction == null || setting.Instruction.Account == null)
				throw new ApplicationException(string.Format("Not possible to instantiate a comparer class when account is null."));

            this.setting = setting;
			this.feeFactory = feeFactory;

            if (Account.ModelPortfolio == null)
                throw new ApplicationException(string.Format("Not possible to compare to model for account {0}: Model is null", Account.DisplayNumberWithName));
        }

        #endregion

        #region Props

        /// <summary>
        /// The <see cref="T:B4F.Accounts.AccountTypeCustomer">account</see> that should be rebalanced
		/// </summary>
        public IAccountTypeCustomer Account
		{
			get { return Setting.Instruction.Account; }
		}

        /// <summary>
        /// An instance of the <see cref="T:B4F.TotalGiro.Fees.FeeFactory">FeeFactory</see> class
        /// </summary>
		public IFeeFactory FeeFactory
		{
			get { return this.feeFactory; }
		}

        /// <summary>
        /// An instance of the <see cref="T:B4F.TotalGiro.PortfolioComparer.PortfolioCompareSetting">PortfolioCompareSetting</see> class
        /// </summary>
        public PortfolioCompareSetting Setting
        {
            get { return this.setting; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method checks whether instruments exist in the account's portfolio that are not in the account's model portfolio
        /// </summary>
        /// <returns>True if the mismatched positions exist</returns>
        public bool CheckInstrumentsNotInModel()
        {
            bool retVal = false;

            // Get the portfolio
            IFundPortfolio portfolio = this.Account.Portfolio.PortfolioInstrument.Exclude(setting.TradeableInstrumentsToExclude).ExcludeNonTradeableInstruments();

            // Get the modelinstruments
            IModelVersion mv = Account.ModelPortfolio.LatestVersion;
            if (mv == null)
                throw new ApplicationException(string.Format("Not possible to check the model for account {0}: Latest ModelVersion is null", Account.DisplayNumberWithName));

            if (portfolio != null && portfolio.Count > 0)
            {
                foreach (IFundPosition pos in portfolio.ExcludeNonTradeableInstruments())
                {
					if (pos.Size.IsNotZero)
                    {
                        IModelInstrument mi = mv.ModelInstruments.Find(pos.InstrumentOfPosition);
                        if (mi == null)
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// This method performs the actual rebalance on the account
        /// </summary>
        /// <param name="result">Output parameter that tells whether the method call was successfull</param>
        /// <returns>An list of <see cref="T:B4F.TotalGiro.Orders.Order">orders</see></returns>
        public IList CompareToModel(out RebalanceResults result)
        {
            bool success = false;
            IList newOrders = null;
            result = RebalanceResults.Success;

            switch (Setting.CompareAction)
            {
                case PortfolioCompareAction.CloseOrders:
                    success = doCloseOrders(out newOrders, out result);
                    break;
                case PortfolioCompareAction.BuyModel:
                    success = doBuyModel(out newOrders, out result);
                    break;
                case PortfolioCompareAction.Rebalance:
                    success = doRebalance(out newOrders, out result);
                    break;
                case PortfolioCompareAction.CashFundOrders:
                    success = doCashFundOrders(out newOrders, out result);
                    break;
            }

            if (success)
                return newOrders;
            else
                return null;
        }

        private bool doBuyModel(out IList newOrders, out RebalanceResults result)
        {
            newOrders = null;
            result = RebalanceResults.Success;
            IBuyModelInstruction bmi = (IBuyModelInstruction)setting.Instruction;

            // Get the modelinstruments
            IModelVersion mv = Account.ModelPortfolio.LatestVersion;
            if (mv == null)
                throw new ApplicationException(string.Format("Not possible to compare to model for account {0}: Latest ModelVersion is null", Account.DisplayNumberWithName));

            // Get the total portfolio value for the model allocation
            Money totalValue = bmi.CashTransfers.TotalTransferAmount;

            if (totalValue != null && totalValue.IsNotZero && totalValue.Sign)
            {
                // Instantiate a PortfolioComparer and fill it with Positions & model Instruments
                PositionComparerCollection pc = new PositionComparerCollection(this);
                pc.AddCash(totalValue);

                // Check if Model is correct
                if (mv.TotalAllocation() != 1m)
                    throw new ApplicationException(string.Format("Not possible to compare to model: {0} does not allocate completely to 100%", mv.ToString()));

                foreach (IModelInstrument mi in mv.ModelInstruments)
                {
                    pc.Add(mi, totalValue);
                }

                // check if order does not already exists
                IAccountOrderCollection orders = this.Account.OpenOrdersForAccount;
                if (orders != null && orders.Count > 0)
                {
                    if (orders.NewCollection(x => x.Side == Side.Sell).Count() > 0)
                        throw new ApplicationException("It is not possible to process a 'buy orders instruction' whenever there are sell orders.");
                }

                if (pc.Compare(out newOrders))
                {
                    // When no Orders -> Rebalance was not necessary
                    if (newOrders == null || newOrders.Count == 0)
                        result = RebalanceResults.RebalanceWasNotNeeded;
                    return true;
                }
            }
            else
                result = RebalanceResults.NoRebalanceNegativePortfolioAmount;
            return true;
        }

        private bool doCloseOrders(out IList newOrders, out RebalanceResults result)
        {
            newOrders = null;
            result = RebalanceResults.Success;
            bool retVal = false;

            // Get the portfolio
            IFundPortfolio portfolio = this.Account.Portfolio.PortfolioInstrument.Exclude(setting.TradeableInstrumentsToExclude);

            // Get the modelinstruments
            IModelVersion mv = Account.ModelPortfolio.LatestVersion;
            if (mv == null)
                throw new ApplicationException(string.Format("Not possible to compare to model for account {0}: Latest ModelVersion is null", Account.DisplayNumberWithName));

            // Instantiate a PortfolioComparer and fill it with Positions & model Instruments
            PositionComparerCollection pc = new PositionComparerCollection(this);
            foreach (IFundPosition pos in portfolio.ExcludeNonTradeableInstruments())
            {
                if (pos.Size.IsNotZero)
                    pc.Add(pos);
            }

            // check if order does not already exists
            IAccountOrderCollection orders = this.Account.OpenOrdersForAccount.Exclude(setting.InstrumentsToExclude);
            if (orders != null && orders.Count > 0)
            {
                foreach (IOrder order in orders)
                {
                    if (!order.IsMonetary)
                        pc.Add((ISecurityOrder)order, setting.CompareAction);
                }
            }

            // Check if Model is correct
            if (mv.TotalAllocation() != 1m)
                throw new ApplicationException(string.Format("Not possible to compare to model: {0} does not allocate completely to 100%", mv.ToString()));
            pc.SetInstrumentsInModel(mv.ModelInstruments);

            if (pc.CompareForCloseOrders(out newOrders))
                retVal = true;
            return retVal;
        }

        private bool doRebalance(out IList newOrders, out RebalanceResults result)
		{
            newOrders = null;
            result = RebalanceResults.Success;

            // Get the portfolio
            IFundPortfolio portfolio = this.Account.Portfolio.PortfolioInstrument.Exclude(setting.TradeableInstrumentsToExclude).ExcludeNonTradeableInstruments();
            ICashPortfolio cashPortfolio = this.Account.Portfolio.PortfolioCashGL;

            // Get the modelinstruments
            IModelVersion mv = Account.ModelPortfolio.LatestVersion;
            if (mv == null)
                throw new ApplicationException(string.Format("Not possible to compare to model for account {0}: Latest ModelVersion is null", Account.DisplayNumberWithName));

            // Get the total portfolio value for the model allocation
            // If there are withdrawals instructions -> Reduce the withdrawal amount
            // This amount has to be taken out of the rebalance, since it needs to be redrawned from the account
            Money withdrawalAmount = Account.ActiveWithdrawalInstructions.TotalAmount;

            Money totalValue = portfolio.TotalValueInBaseCurrency + cashPortfolio.SettledCashTotalInBaseValue + withdrawalAmount;

            if (totalValue.IsLessThanZero && Account.Portfolio.TotalValue().IsGreaterThanZero && withdrawalAmount != null)
                throw new ApplicationException(string.Format("The withdrawal amount is larger than the portfolio value for account {0}.", Account.DisplayNumberWithName));

            if (isRebalanceNeeded(totalValue, portfolio))
            {
                // Instantiate a PortfolioComparer and fill it with Positions & model Instruments
                PositionComparerCollection pc = new PositionComparerCollection(this);
                // add instruments
                foreach (IFundPosition pos in portfolio.Where(x => x.Size.IsNotZero))
                    pc.Add(pos);
                // Add cash
                foreach (ICashPosition pos in cashPortfolio.Where(x => x.SettledSize.IsNotZero))
                    pc.Add(pos);

                // check if order does not already exists
                IAccountOrderCollection orders = this.Account.OpenOrdersForAccount.Exclude(setting.InstrumentsToExclude);
                if (orders != null && orders.Count > 0)
                {
                    foreach (IOrder order in orders)
                        pc.Add(order, setting.CompareAction);
                }

                // Check if Model is correct
                IModelInstrumentCollection modelInstrumentDistribution = mv.ModelInstruments.StrippedCollection(((IRebalanceInstruction)setting.Instruction).ExcludedComponents);
                if (modelInstrumentDistribution.TotalAllocation != 1m)
                    throw new ApplicationException(string.Format("Not possible to compare to model: {0} does not allocate completely to 100%", mv.ToString()));

                if (totalValue != null && totalValue.IsNotZero)
                {
                    foreach (IModelInstrument mi in modelInstrumentDistribution)
                    {
                        pc.Add(mi, totalValue);
                    }

                    // Add the cash from the withrawal instructions
                    pc.AddReservedCash(withdrawalAmount, mv.GetCashManagementFund());

                    if (pc.Compare(out newOrders))
                    {
                        doSellForeignCash(newOrders);

                        // When no Orders -> Rebalance was not necessary
                        if (newOrders == null || newOrders.Count == 0)
                            result = RebalanceResults.RebalanceWasNotNeeded;
                        else
                        {
                            //// Set the cash transfers to being processed
                            if (Setting.Instruction.InstructionType == InstructionTypes.Rebalance)
                            {
                                IRebalanceInstruction ri = (IRebalanceInstruction)Setting.Instruction;
                                if (ri.CashTransfers != null && ri.CashTransfers.Where(c => !c.SkipOrders).Count() > 0)
                                {
                                    foreach (IJournalEntryLine transfer in ri.CashTransfers.Where(c => !c.SkipOrders))
                                        transfer.SkipOrders = true;
                                }
                            }
                        }
                    }
                }
            }
            else
                result = RebalanceResults.NoRebalanceNegativePortfolioAmount;
            return true;
		}

        private bool isRebalanceNeeded(Money totalValue, IFundPortfolio portfolio)
        {
            bool retVal = false;

            if (totalValue != null && totalValue.IsNotZero && totalValue.Sign)
                retVal = true;
            else if (totalValue != null && totalValue.IsZero)
            {
                foreach (IFundPosition pos in portfolio)
                {
                    if (pos.Size.IsNotZero && pos.Size.Sign)
                    {
                        retVal = true;
                        break;
                    }
                }
            }
            return retVal;
        }

        private bool doCashFundOrders(out IList newOrders, out RebalanceResults result)
        {
            //IPortfolio portfolio;
            ICashPortfolio cashPortfolio;
            newOrders = null;
            IInstrument cashFund = null;
            Money totalValue = null;
            bool retVal = false;
            result = RebalanceResults.Success;

            // Get the modelinstruments
            IModelVersion mv = Account.ModelPortfolio.LatestVersion;
            if (mv == null)
                throw new ApplicationException(string.Format("Not possible to place cash fund orders for account {0}: Latest ModelVersion is null", Account.DisplayNumberWithName));

            cashFund = mv.GetCashFundOrAlternative();
            if (cashFund != null)
            {
                if (Setting.UseCashOnly)
                    ((IBuyModelInstruction)Setting.Instruction).GetRoundingDifference(out totalValue);
                else
                {
                    // extra check -> does the account have a positive amount
                    if (this.Account.Portfolio.TotalValue().Sign)
                    {
                        // Get the cash positions
                        cashPortfolio = this.Account.Portfolio.PortfolioCashGL;

                        if (cashPortfolio != null && cashPortfolio.Count > 0)
                        {
                            // Check for foreign currency positions
                            if (cashPortfolio.Any(x => (!x.PositionCurrency.IsBase && x.SettledSize.IsNotZero)))
                                throw new ApplicationException(string.Format("Not possible to place cash fund orders: there are still foreign currency positions for account {0}. Close them first", this.Account.Number.ToString()));

                            // Check for pending Orders 
                            IAccountOrderCollection orders = this.Account.OpenOrdersForAccount.Exclude(setting.InstrumentsToExclude);
                            if (orders != null && orders.Count > 0)
                                throw new ApplicationException(string.Format("Not possible to place cash fund orders: there are {0} pending orders for account {1}", orders.Count.ToString(), this.Account.Number.ToString()));

                            // Get the total portfolio value for the Cash Fund Order
                            totalValue = cashPortfolio.SettledCashTotalInBaseValue;
                            // Keep cash free if there is a withdrawal
                            ICashWithdrawalInstruction withdrawal = null;
                            if (Setting.Instruction.InstructionType == InstructionTypes.Rebalance)
                                withdrawal = ((IRebalanceInstruction)Setting.Instruction).CashWithdrawalInstruction;
                            totalValue += Account.ActiveWithdrawalInstructions.TotalKeepCashAmount(withdrawal);
                            
                        }
                    }
                    else
                        result = RebalanceResults.NoCashFundOrdersNegativePortfolioAmount;
                }

                if (totalValue != null && totalValue.IsNotZero && totalValue.Abs().Quantity > 0.01M)
                {
                    IInstructionTypeRebalance instruction = (IInstructionTypeRebalance)Setting.Instruction;
                    OrderAmountBased order = new OrderAmountBased((IAccountTypeInternal)Account, totalValue, cashFund, true, this.FeeFactory, instruction.DoNotChargeCommission, Setting.ActionType);
                    order.Instruction = instruction;
                    newOrders = new ArrayList();
                    newOrders.Add(order);
                    retVal = true;
                }
            }
            return retVal;
        }

        private void doSellForeignCash(IList newOrders)
        {
            bool doIt = false;

            // check if there are foreign currency order
            foreach (IOrder order in newOrders)
            {
                IInstrument instrument = ((ISecurityOrder)order).TradedInstrument;
                if (!((ITradeableInstrument)instrument).CurrencyNominal.IsBase)
                {
                    doIt = true;
                    break;
                }
            }

            if (doIt)
            {
                // Check for foreign currency positions
                if (Account.Portfolio.PortfolioCashGL.Any(x => (!x.PositionCurrency.IsBase && x.SettledSize.IsNotZero)))
                    throw new ApplicationException(string.Format("Not possible to finish rebalance instruction {0}: there are still foreign currency positions for account {1}. Close them first", setting.Instruction.Key, Account.Number));
                // to do -> in future sell the foreign cash positions

                //foreach (Position position in Account.Portfolio)
                //{
                //    if (position.IsCashPosition)
                //    {
                //        CashPosition cashpos = (CashPosition)position;
                //        if (!((ICurrency)cashpos.Instrument).IsBase)
                //        {
                //            Money amount = cashpos.CurrentValue + cashpos.OpenOrderAmount - cashpos.OpenOrderAmountInSameCurrency;
                //            if (amount.IsNotZero)
                //            {
                //                IInstructionTypeRebalance instruction = (IInstructionTypeRebalance)Setting.Instruction;
                //                MonetaryOrder monetaryorder = new MonetaryOrder(Account, (amount * -1), FeeFactory);
                //                monetaryorder.ActionType = Setting.ActionType;
                //                monetaryorder.Instruction = instruction;
                //                newOrders.Add(monetaryorder);
                //            }
                //        }
                //    }
                //}
            }
        }

        #endregion

        #region Privates

        private IFeeFactory feeFactory;
        private PortfolioCompareSetting setting;

		#endregion

	}
}
