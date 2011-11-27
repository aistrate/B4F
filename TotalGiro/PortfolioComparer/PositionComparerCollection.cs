using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.PortfolioComparer
{
	internal class PositionComparerCollection : IEnumerable
	{
		private enum whichValue
		{
			ActualPosition,
			ModelPosition,
			Order,
            OpenOrderAmount,
            ReservedCash,
            Cash
}

		public PositionComparerCollection(PortfolioComparer parent)
		{
			if (parent.Account == null)
				throw new ApplicationException(string.Format("Not possible to instantiate a comparer class when account is null."));
			this.parent = parent;
		}

		internal void Add(IFundPosition pos)
		{
			PositionComparer pc = getPos((IInstrument)pos.InstrumentOfPosition);
            if (parent.Account.BaseCurrency.Equals(pos.CurrentValue.Underlying))
                pc.ActualPositionValue = pos.CurrentValue;
			else
                pc.ActualPositionValue = pos.CurrentValue.CurrentBaseAmount;
			pc.ActualPositionSize = pos.Size;
			if (pos.InstrumentOfPosition.CurrentPrice != null)
                pc.LastPrice = pos.InstrumentOfPosition.CurrentPrice.Price;
		}

        internal void Add(ICashPosition pos)
		{
			PositionComparer pc = getPos(pos.SettledSize.Underlying);
            pc.ActualPositionValue = pos.SettledSizeInBaseCurrency;
            pc.ActualPositionSize = pos.SettledSize;
		}

        internal void AddCash(Money amount)
        {
            PositionComparer pc = getPos((IInstrument)amount.Underlying);
            if (parent.Account.BaseCurrency.Equals(amount.Underlying))
                pc.ActualPositionValue = amount;
            else
                throw new ApplicationException("Not in base currency");
            pc.ActualPositionSize = amount;
        }

        internal void AddReservedCash(Money amount, ITradeableInstrument cashManagementFund)
        {
            if (amount != null && amount.IsNotZero)
            {
                PositionComparer pc = getPos((IInstrument)amount.Underlying);
                if (parent.Account.BaseCurrency.Equals(amount.Underlying))
                {
                    pc.ReservedCash = amount;
                    if (cashManagementFund != null)
                    {
                        PositionComparer pcmf = findPos(cashManagementFund);
                        if (pcmf != null)
                            pcmf.ReservedWithdrawalAmount += amount;
                    }
                }
                else
                    throw new ApplicationException("Not in base currency");
            }
        }

		internal void Add(IModelInstrument mi, Money totalPortfolioValue)
		{
            this.totalPortfolioValue = totalPortfolioValue;
            PositionComparer pc = getPos(mi.Component);
			pc.ModelPositionValue = (totalPortfolioValue * mi.Allocation);
			pc.InModel = true;
		}

        internal void Add(IOrder order, PortfolioCompareAction compareAction)
        {
            if (!(order.IsMonetary && ((IMonetaryOrder)order).MoneyParent != null))
            {
                if (!order.IsClosed && order.OpenValue.IsNotZero)
                {
                    decimal ratio = 1M;
                    // remove already approved transactions from the ratio
                    if (order.Transactions != null && order.Transactions.Count > 0)
                        ratio -= order.Transactions.TotalApprovedFillRatio();
                    
                    // Find traded instrument
                    PositionComparer pc = getPos(order.RequestedInstrument);
                    if (pc != null)
                    {
                        if (order.IsAmountBased)
                            pc.ExistingOrderAmount += order.GrossAmount * ratio;
                        else if (order.IsSizeBased)
                            pc.ExistingOrderSize += order.OpenValue;
                        pc.OrderIds += (pc.OrderIds != string.Empty ? ", " : "") + order.OrderID.ToString();
                    }

                    if (compareAction != PortfolioCompareAction.CloseOrders)
                    {
                        if (!(order.OpenValue.Underlying.IsCash && order.OpenValue.Underlying.ToCurrency.IsBase))
                            throw new ApplicationException("Order not in base currency");

                        // Find cash record
                        pc = getPos(order.OpenValue.Underlying);
                        if (pc != null)
                            pc.OpenOrderAmount += (order.GrossAmount * ratio * - 1M);
                    }
                }
            }
        }

        internal void SetInstrumentsInModel(IModelInstrumentCollection modelInstruments)
        {
            foreach (IModelInstrument mi in modelInstruments)
            {
                PositionComparer pc = getPos(mi.Component);
                pc.InModel = true;
            }
        }

		private PositionComparer getPos(IInstrument instrument)
		{
			PositionComparer pc = null;

			pc = this[instrument.Key];
			if (pc == null)
			{
				pc = new PositionComparer(instrument);
				positions.Add(instrument.Key, pc);
			}
			return pc;
		}

        private PositionComparer findPos(IInstrument instrument)
        {
            return this[instrument.Key];
        }

		private void adjustModelPortfolioToActualPortfolio(Money diff)
		{
			Money max = null;
			PositionComparer maxPos = null;

			// Get the biggest position
			foreach (PositionComparer pos in this)
			{
				if (pos.InModel)
				{
					if (max == null)
					{
						max = pos.ModelPositionValue;
						maxPos = pos;
					}
					else if (max < pos.ModelPositionValue)
					{
						max = pos.ModelPositionValue;
						maxPos = pos;
					}
				}
			}
			// Increment the difference
			if (maxPos != null)
			{
				maxPos.ModelPositionValue += diff;
			}
		}

		private void adjustOrderValue(Money diff)
		{
			Money max = null;
			PositionComparer maxPos = null;

			// Get the biggest position
			foreach (PositionComparer pos in this)
			{
				if (pos.InModel)
				{
					if (max == null)
					{
						max = pos.OrderValue.Abs();
						maxPos = pos;
					}
					else if (max < pos.OrderValue.Abs())
					{
						max = pos.OrderValue.Abs();
						maxPos = pos;
					}
				}
			}
			// Increment the difference
			if (maxPos != null)
			{
				maxPos.OrderValue -= diff;
			}
		}

		#region TotalValues

		public Money TotalActualPositionValue()
		{
			return totalValue(whichValue.ActualPosition);
		}

		public Money TotalModelPositionValue()
		{
			return totalValue(whichValue.ModelPosition);
		}

        public Money TotalAbsOrderValue()
        {
            Money value = null;
            foreach (PositionComparer pos in this)
            {
                if (pos.OrderValue != null)
                    value += pos.OrderValue.Abs();
            }
            return value;
        }

		public Money TotalOrderValue()
		{
			return totalValue(whichValue.Order);
		}

        public Money TotalOpenOrderAmount()
        {
            return totalValue(whichValue.OpenOrderAmount);
        }

        public Money TotalReservedCash()
        {
            return totalValue(whichValue.ReservedCash);
        }

        public Money TotalCash()
        {
            return totalValue(whichValue.Cash);
        }

		private Money totalValue(whichValue type)
		{
			Money value = null;

			foreach (PositionComparer pos in this)
			{
				switch (type)
				{
					case whichValue.ActualPosition:
						value += pos.ActualPositionValue;
						break;
                    case whichValue.ModelPosition:
                        value += pos.ModelPositionValue;
                        break;
                    case whichValue.Order:
						value += pos.OrderValue;
						break;
                    case whichValue.OpenOrderAmount:
                        value += pos.OpenOrderAmount;
                        break;
                    case whichValue.ReservedCash:
                        value += pos.ReservedCash;
                        break;
                    case whichValue.Cash:
                        if (pos.Instrument.IsCash && pos.ActualPositionValue != null && pos.ActualPositionValue.IsNotZero)
                        {
                            if (pos.Instrument.ToCurrency.IsBase)
                                value += pos.ActualPositionValue.GetMoney();
                            else
                                value += pos.ActualPositionValue.GetMoney().CurrentBaseAmount;
                        }
                        break;
                    default:
						throw new ApplicationException("You did not select a size type");
				}
			}
			return value;
		}

		#endregion

        public bool CompareForCloseOrders(out IList newOrders)
        {
            bool result = false;
            newOrders = null;

            if (parent.Setting.CompareAction == PortfolioCompareAction.CloseOrders)
            {
                foreach (PositionComparer pos in this)
                {
                    if (!pos.InModel)
                    {
                        // Check existing order and its type
                        if (pos.ExistingOrderAmount != null && pos.ExistingOrderAmount.IsNotZero)
                            throw new ApplicationException(string.Format("It is not possible to place close orders whenever there are still active amount based orders for {0}", pos.Instrument.Name));

                        if (pos.ExistingOrderSize != null && pos.ActualPositionSize != null && !pos.ExistingOrderSize.Underlying.Equals(pos.ActualPositionSize.Underlying))
                            throw new ApplicationException("Something spooky went wrong in the close orders procedure");

                        // check if there is a previous order
                        InstrumentSize prevOrderSize = null;
                        if (pos.ExistingOrderSize != null && pos.ExistingOrderSize.IsNotZero)
                        {
                            // Check if the previous Order(s) do(es) not exceed the Position Size                                
                            if (pos.ActualPositionSize.Sign != pos.ExistingOrderSize.Sign && pos.ActualPositionSize.Abs() < pos.ExistingOrderSize.Abs())
                                throw new ApplicationException(string.Format("Account {0} already has orders for {1}. The current order size {2} exceed the position size {3}. Please cancel these order(s) {4}.", parent.Account.DisplayNumberWithName, pos.ActualPositionSize.Underlying.DisplayName, pos.ExistingOrderSize.DisplayString, pos.ActualPositionSize.DisplayString, pos.OrderIds));

                            prevOrderSize = pos.ExistingOrderSize;
                        }
                        // Close the position
                        pos.OrderSize = (pos.ActualPositionSize * -1) - prevOrderSize;
                        pos.IsClosure = true;
                    }
                }

                try
                {
                    // Create the Orders
                    newOrders = new ArrayList();
                    IOrder order = null;

                    foreach (PositionComparer pos in this)
                    {
                        // Only add Close Orders for Tradeable instruments
                        if (!pos.Instrument.SecCategory.IsCash)
                        {
                            if (pos.IsClosure && pos.OrderSize != null && pos.OrderSize.IsNotZero)
                            {
                                InstructionTypeRebalance instruction = (InstructionTypeRebalance)parent.Setting.Instruction;
                                order = new OrderSizeBased(parent.Account, pos.OrderSize, true, parent.FeeFactory, instruction.DoNotChargeCommission, parent.Setting.ActionType);
                                order.Instruction = instruction;
                                newOrders.Add(order);
                            }
                        }
                    }
                    result = true;
                }

                catch (Exception ex)
                {
                    result = false;
                    throw new ApplicationException(string.Format("An error occured in the compare module while creating closing orders for account {0}.", parent.Account.DisplayNumberWithName), ex);
                }
            }
            return result;
        }

		public bool Compare(out IList newOrders)
        {
			bool result = false;
            newOrders = null;

            if (parent.Setting.CompareAction == PortfolioCompareAction.Rebalance || parent.Setting.CompareAction == PortfolioCompareAction.BuyModel)
            {
                // Check if allocated ok
                Money diff = TotalActualPositionValue() - TotalModelPositionValue() + TotalReservedCash();
                if (diff.IsNotZero)
                {
                    if (diff.IsWithinTolerance(0.05M))
                        adjustModelPortfolioToActualPortfolio(diff);
                    else
                        throw new ApplicationException(string.Format("For account {0} a rounding error in the rebalance was determined in the compare module", parent.Account.DisplayNumberWithName));
                }

                foreach (PositionComparer pos in this)
                {
                    if (!pos.Instrument.SecCategory.IsCash)
                    {
                        if (pos.InModel)
                        {
                            // Check existing order and its type
                            if (pos.ExistingOrderSize != null && pos.ExistingOrderSize.IsNotZero)
                                throw new ApplicationException(string.Format("It is not possible to place amount base orders whenever there are still active size based orders for {0}", pos.Instrument.Name));

                            // Instrument is in model so adjust the Amount
                            pos.OrderValue = pos.ModelPositionValue - (pos.ActualPositionValue + pos.ExistingOrderAmount + pos.ReservedWithdrawalAmount);
                        }
                        else
                            throw new ApplicationException("It is not possible to do the rebalance whenever there are either still positions or active orders for instruments that are not in the current model. Cancel the existing instruction and create a new one");
                    }
                }

                if (TotalAbsOrderValue() == null || TotalAbsOrderValue().IsZero)
                    return false;

                // Check if allocated ok
                diff = TotalOrderValue() - TotalCash() - TotalOpenOrderAmount() - (ModelContainsCashManagementFund ? null : TotalReservedCash());
                if (diff.IsNotZero)
                {
                    if (diff.IsWithinTolerance(0.01m))
                        adjustOrderValue(diff);
                    else
                        throw new ApplicationException(string.Format("For account {0} a rounding error in the Orders was determined in the compare module", parent.Account.DisplayNumberWithName));
                }

                try
                {
                    // Create the Orders
                    newOrders = new ArrayList();
                    IOrder order = null;

                    foreach (PositionComparer pos in this)
                    {
                        if (!pos.Instrument.SecCategory.IsCash)
                        {
                            if (!pos.IsClosure)
                            {
                                if (pos.OrderValue.IsNotZero)
                                {
                                    // If not use Cash only
                                    // exclude the Cash Management fund unless it is a sell order
                                    // exception -> when the model consists of 100% Cash Management Fund
                                    if (parent.Setting.UseCashOnly
                                        || (!parent.Setting.UseCashOnly && (pos.Instrument.SecCategory.Key != SecCategories.CashManagementFund
                                        || (pos.Instrument.SecCategory.Key == SecCategories.CashManagementFund && pos.Side == Side.Sell)))
                                        || isModelOnlyCashManagementFund())
                                    {
                                        if (IsOrderValueWithinTolerance(pos.OrderValue.GetMoney()))
                                        {
                                            InstructionTypeRebalance instruction = (InstructionTypeRebalance)parent.Setting.Instruction;
                                            order = new OrderAmountBased(parent.Account, pos.OrderValue.GetMoney(), pos.Instrument, (pos.Side == Side.Buy), parent.FeeFactory, instruction.DoNotChargeCommission, parent.Setting.ActionType);
                                            order.Instruction = instruction;
                                            newOrders.Add(order);
                                        }
                                    }
                                }
                            }
                            else
                                throw new ApplicationException(string.Format("During a rebalance all positions should match the model portfolio. Close all mismatched positions first fro account {0}.", parent.Account.DisplayNumberWithName));
                        }
                    }
                    result = true;
                }

                catch (Exception ex)
                {
                    result = false;
                    throw new ApplicationException(string.Format("An error occured in the compare module while rebalancing account {0}.", parent.Account.DisplayNumberWithName), ex);
                }
            }
			return result;
        }

        private bool IsOrderValueWithinTolerance(Money orderValue)
        {
            bool retVal = false;

            if (parent.Setting.EngineParams != null)
            {
                retVal = parent.Setting.EngineParams.IsOrderValueWithinTolerance(orderValue, this.totalPortfolioValue);
            }
            else
                retVal = true;
            return retVal;
        }

		internal PositionComparer this[int index]
		{
			get
			{
                if (positions.ContainsKey(index))
                    return positions[index];
                else
                    return null;
			}
		}

        private bool isModelOnlyCashManagementFund()
        {
            int i = 0;
            bool retVal = false;
            foreach (PositionComparer pos in this)
            {
                if (pos.InModel)
                {
                    if (pos.Instrument.SecCategory.Key != SecCategories.CashManagementFund)
                        return false;
                    else
                        i++;
                }
            }
            // only 1 instrument -> model is 100 % CashManagementFund
            if (i == 1)
                retVal = true;
            return retVal;
        }

        private bool ModelContainsCashManagementFund
        {
            get
            {
                foreach (PositionComparer pos in this)
                {
                    if (pos.InModel && pos.Instrument.SecCategory.Key == SecCategories.CashManagementFund)
                        return true;
                }
                return false;
            }
        }


		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
            return positions.Values.GetEnumerator();
        }

		#endregion

		#region Privates

        private Money totalPortfolioValue;
        private PortfolioComparer parent;
        private Dictionary<int, PositionComparer> positions = new Dictionary<int, PositionComparer>();

		#endregion

	}
}
