using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Fees.CommCalculations
{
	/// <summary>
	/// Calculates a flat commission.
	/// </summary>
    public class CommCalcFlat: CommCalc
	{
        protected CommCalcFlat() : base() { }

		public CommCalcFlat(string name, ICurrency commCurrency, Money minValue, Money maxValue, Money fixedSetup)
			: base(name, commCurrency, minValue, maxValue, fixedSetup) { }

        /// <summary>
        /// Indicates whether the commission calculation is flat, slab, size based, or simple.
        /// </summary>
        public override FeeCalcTypes CalcType
        {
            get { return FeeCalcTypes.Flat; }
        }

        /// <summary>
        /// Calculates a flat commission for a given order.
        /// </summary>
        /// <param name="client">The order for which to calculate the commission.</param>
        /// <returns>The value of the commission.</returns>
        public override Money Calculate(ICommClient client)
		{
			Money amount = null;
			Money fee = null;

            if (!(client.IsSizeBased))
			{
                // Value based order
                bool isValueInclComm = true;
                if (client.OrderType == OrderTypes.AmountBased)
                    isValueInclComm = client.IsValueInclComm;

                if (isValueInclComm)
                {
                    // Value includes the commission -> calculate backwards using Amount
                    // Value includes commission -> Subtract the previous calculated commission
                    amount = client.Amount + client.PreviousCalculatedFee;
                    fee = calculateNett(amount, client);

                    AdjustBondCommission(client, ref fee);
                }
                else
                {
                    // Value does not include the commission -> calculate normal
                    amount = client.Amount - client.PreviousCalculatedFee;
                    // Sell -> Add Commission over the commission
                    if (client.Type == CommClientType.Order && client.Side == Side.Sell)
                        fee = calculateForwards(amount, client);
                    else
                        fee = calculateNormal(amount, client);
                }
			}
			else
			{
				// Size based Order
                if (client.Type == CommClientType.Order)
				{
					// Calculate the Indicative Commission
                    amount = GetAmountSizeBasedOrder(client);
					if (amount != null)
					{
                        // amount is fixed (size * price) -> Add the previous calculated commission
                        amount = amount - client.PreviousCalculatedFee;
                        fee = calculateNormal(amount, client);
					}
				}
				else
				{
					// Commission is calculated at Order.Fill time
					// sp price is known
                    fee = calculateNormal(client.Amount, client);
				}
			}
            return ConvertToOrderCurrency(fee, client);
		}

        /// <summary>
        /// Used by method <b>Calculate</b> on derived classes to perform the common part of the commission calculation.
        /// </summary>
        /// <param name="amount">The amount for which to calculate commission.</param>
        /// <param name="client">The order for which to calculate commission.</param>
        /// <returns>The value of the commission.</returns>
        protected Money calculateNormal(Money amount, ICommClient client)
        {
            Money result = new Money(0m, CommCurrency);
            Money comAmount = getCommAmount(amount);

            foreach (CommCalcLine line in CommLines)
            {
                if (line.Envelops(comAmount))
                {
                    result = line.Calculate(comAmount);
                    SetCommissionInfoOnOrder(client, string.Format("Flat -> Amount ({0}) {1} -> use {2}.", comAmount.ToString(), line.DisplayRange, line.LineDistinctives));
                    break;
                }
            }
            return addFixMinMax(result, client);
        }

        /// <summary>
        /// Used by method <b>Calculate</b> on derived classes to perform the commission calculation for amount based sell exclusive orders.
        /// </summary>
        /// <param name="amount">The amount for which to calculate commission.</param>
        /// <param name="client">The order for which to calculate commission.</param>
        /// <returns>The value of the commission.</returns>
        protected Money calculateForwards(Money nettAmount, ICommClient client)
        {
            if (!(client.Type != CommClientType.Transaction && client.Side == Side.Sell && client.IsValueInclComm == false))
                throw new ApplicationException("There is a bug in the program flow in the fees module.");

            Money result = new Money(0m, CommCurrency);
            Money comAmount = getCommAmount(nettAmount);

            foreach (CommCalcLineAmountBased line in CommLines)
            {
                if (line.Envelops(comAmount))
                {
                    result = line.CalculateExtra(comAmount);
                    SetCommissionInfoOnOrder(client, string.Format("Flat (forwards) -> Amount ({0}) {1} -> use {2}.", comAmount.ToString(), line.DisplayRange, line.LineDistinctives));
                    break;
                }
            }
            return addFixMinMax(result, client);
        }

        private Money calculateNett(Money amount, ICommClient client)
        {
            Money fee = null;

            foreach (CommCalcLineAmountBased line in CommLines)
            {
                if (line.CalculateBackwards(amount, out fee))
                {
                    SetCommissionInfoOnOrder(client, string.Format("Flat (backwards) -> Amount ({0}) {1} -> use {2}.", amount.ToString(), line.DisplayRange, line.LineDistinctives));
                    break;
                }
            }
            return fee;
        }
	}
}
