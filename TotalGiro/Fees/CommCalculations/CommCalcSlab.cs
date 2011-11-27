using System;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Fees.CommCalculations
{
    /// <summary>
    /// Calculates a slab commission.
    /// </summary>
    public class CommCalcSlab : CommCalc
	{
        protected CommCalcSlab() : base() { }

		public CommCalcSlab(string name, ICurrency commCurrency, Money minValue, Money maxValue, Money fixedSetup)
			: base(name, commCurrency, minValue, maxValue, fixedSetup) { }

        /// <summary>
        /// Indicates whether the commission calculation is flat, slab, size based, or simple.
        /// </summary>
        public override FeeCalcTypes CalcType
        {
            get { return FeeCalcTypes.Slab; }
        }

        /// <summary>
        /// Calculates a slab commission for a given order.
        /// </summary>
        /// <param name="client">The order for which to calculate the commission.</param>
        /// <returns>The value of the commission.</returns>
        public override Money Calculate(ICommClient client)
		{
			Money amount = null;
			Money fee = null;

            if (!(client.IsSizeBased))
            // Value based order
            {
                bool isValueInclComm = true;
                if (client.OrderType == OrderTypes.AmountBased)
                    isValueInclComm = client.IsValueInclComm;

                if (isValueInclComm)
                {
                    decimal realAmount;
                    amount = client.Amount + client.PreviousCalculatedFee;

                    // Value does not include the commission
                    if (client.Side == Side.Sell)
                        // Sell -> Add Commission over the commission
                        realAmount = FinancialMath.GoalSeek(x => x + calculateForwards(new Money(x, (ICurrency)amount.Underlying), client).Quantity,
                                                            amount.Quantity, amount.Quantity);
                    else
                         //Buy -> calculate normal
                        realAmount = FinancialMath.GoalSeek(x => x + calculateNormal(new Money(x, (ICurrency)amount.Underlying), client).Quantity,
                                                            amount.Quantity, amount.Quantity);

                    fee = amount - new Money(realAmount, (ICurrency)client.Amount.Underlying);

                    AdjustBondCommission(client, ref fee);
                }
                else
                {
                    amount = client.Amount - client.PreviousCalculatedFee;

                    // Value does not include the commission
                    if (client.Side == Side.Sell)
                        // Sell -> Add Commission over the commission
                        fee = calculateForwards(amount, client);
                    else
                        // Buy -> calculate normal
                        fee = calculateNormal(amount, client);
                }
			}
			else
            // Size based order
            {
                if (client.Type == CommClientType.Order)
				{
					// Calculate the Indicative Commission
                    amount = GetAmountSizeBasedOrder(client);
					if (amount != null)
					{
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

            if (client.IsValueInclComm)
                client.CommissionInfo = "";

            foreach (CommCalcLineAmountBased line in CommLines)
            {
                if (line.IsUnder(comAmount))
                {
                    Money lineAmount = (line.UpperRange - line.LowerRange);
                    result += line.Calculate(lineAmount);
                    SetCommissionInfoOnOrder(client, string.Format("Slab {0} -> {1} multiplied with {2}.", line.SerialNo.ToString(), lineAmount.ToString(), line.LineDistinctives));
                }
                else if (line.Envelops(comAmount))
                {
                    Money lineAmount = (comAmount - line.LowerRange);
                    result += line.Calculate(comAmount - line.LowerRange);
                    SetCommissionInfoOnOrder(client, string.Format("Slab {0} -> {1} multiplied with {2}.", line.SerialNo.ToString(), lineAmount.ToString(), line.LineDistinctives));
                }
            }
            return addFixMinMax(result, client);
        }


        /// <summary>
        /// Used by method <b>Calculate</b> on derived classes to perform the commission calculation for amount based sell exclusive orders.
        /// </summary>
        /// <param name="nettAmount">The amount for which to calculate commission.</param>
        /// <param name="client">The order for which to calculate commission.</param>
        /// <returns>The value of the commission.</returns>
        protected Money calculateForwards(Money nettAmount, ICommClient client)
        {
            if (!(client.Type != CommClientType.Transaction && client.Side == Side.Sell))
                throw new ApplicationException("There is a bug in the program flow in the fees module.");

            Money result = new Money(0m, CommCurrency);
            Money comAmount = getCommAmount(nettAmount);

            if (client.IsValueInclComm)
                client.CommissionInfo = "";

            foreach (CommCalcLineAmountBased line in CommLines)
            {
                if (line.IsUnder(comAmount))
                {
                    Money lineAmount = (line.UpperRange - line.LowerRange);
                    result += line.CalculateExtra(lineAmount);
                    SetCommissionInfoOnOrder(client, string.Format("Slab (forwards) {0} -> {1} multiplied with {2}.", line.SerialNo.ToString(), lineAmount.ToString(), line.LineDistinctives));
                }
                else if (line.Envelops(comAmount))
                {
                    Money lineAmount = (comAmount - line.LowerRange);
                    result += line.CalculateExtra(comAmount - line.LowerRange);
                    SetCommissionInfoOnOrder(client, string.Format("Slab (forwards) {0} -> {1} multiplied with {2}.", line.SerialNo.ToString(), lineAmount.ToString(), line.LineDistinctives));
                }
            }
            return addFixMinMax(result, client);
        }
    
    }
}
