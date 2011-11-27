using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.Fees.CommCalculations
{
	/// <summary>
	/// Calculates a flat commission on a size basis.
	/// </summary>
    public class CommCalcFlatSizeBased: CommCalc
    {
        #region Constructor

        protected CommCalcFlatSizeBased() : base() { }

        public CommCalcFlatSizeBased(string name, ICurrency commCurrency, Money minValue, Money maxValue, Money fixedSetup)
			:
			base(name, commCurrency, minValue, maxValue, fixedSetup) { }

        #endregion

        /// <summary>
        /// Indicates whether the commission calculation is flat, slab, size based, or simple.
        /// </summary>
        public override FeeCalcTypes CalcType
        {
            get { return FeeCalcTypes.FlatSizeBased; }
        }

        /// <summary>
        /// Calculates a flat commission for a given order.
        /// </summary>
        /// <param name="client">The order for which to calculate the commission.</param>
        /// <returns>The value of the commission.</returns>
        public override Money Calculate(ICommClient client)
		{
            InstrumentSize size = null;
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
                }
                else
                {
                    // Value does not include the commission -> calculate normal

                    // Calculate the Indicative Commission
                    size = GetSizeAmountBasedOrder(client, isValueInclComm);
                    if (size == null)
                        return null;
                    else                    
                        fee = calculateNormal(size, client);
                }
			}
			else
                fee = calculateNormal(client.Value, client);
            return ConvertToOrderCurrency(fee, client);
		}

        /// <summary>
        /// Used by method <b>Calculate</b> on derived classes to perform the common part of the commission calculation.
        /// </summary>
        /// <param name="size">The size for which to calculate commission.</param>
        /// <param name="client">The order for which to calculate commission.</param>
        /// <returns>The value of the commission.</returns>
        protected Money calculateNormal(InstrumentSize size, ICommClient client)
        {
            Money result = new Money(0m, CommCurrency);
            InstrumentSize comSize = size.Abs();

            foreach (CommCalcLineSizeBased line in CommLines)
            {
                if (line.Envelops(comSize))
                {
                    result = line.Calculate(comSize);
                    SetCommissionInfoOnOrder(client, string.Format("Flat -> Size ({0}) {1} -> use {2}.", comSize.DisplayString, line.DisplayRange, line.LineDistinctives));
                    break;
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
            if (!(client.Type != CommClientType.Transaction && client.Side == Side.Sell && client.IsValueInclComm == false))
                throw new ApplicationException("There is a bug in the program flow in the fees module.");

            Money result = new Money(0m, CommCurrency);
            Money comAmount = getCommAmount(nettAmount);

            ITradeableInstrument instrument = (ITradeableInstrument)client.TradedInstrument;
            if (instrument.CurrentPrice == null)
                throw new ApplicationException("It is not possible to calculate the commission when there is no current price.");
            InstrumentSize size = comAmount.CalculateSize(instrument.CurrentPrice.Price);

            foreach (CommCalcLineSizeBased line in CommLines)
            {
                if (line.Envelops(size))
                {
                    result = line.Calculate(size);
                    SetCommissionInfoOnOrder(client, string.Format("Flat (forwards) -> Size ({0}) {1} -> use {2}.", comAmount.ToString(), line.DisplayRange, line.LineDistinctives));
                    break;
                }
            }
            return addFixMinMax(result, client);
        }

        private Money calculateNett(Money amount, ICommClient client)
        {
            Money fee = null;
            Tuple<InstrumentSize, Money> result;
            Price price = client.Price;
            if (price == null && client.TradedInstrument.IsTradeable)
                price = ((ITradeableInstrument)client.TradedInstrument).CurrentPrice.Get(e => e.Price);

            foreach (CommCalcLineSizeBased line in CommLines)
            {
                if (line.CalculateBackwards(amount, price, client.Side, out result))
                {
                    SetCommissionInfoOnOrder(client, string.Format("Flat (backwards) -> Size ({0}) {1} -> use {2}.", result.Item1.DisplayString, line.DisplayRange, line.LineDistinctives));
                    fee = result.Item2;
                    break;
                }
            }
            return fee;
        }

        /// <summary>
        /// Calculates the amount for a size-based order, by finding out the price first.
        /// </summary>
        /// <param name="client">The order for which to calculate.</param>
        /// <param name="isValueInclComm">Is Value Incl. Comm.</param>
        /// <returns>The calculated amount.</returns>
        protected InstrumentSize GetSizeAmountBasedOrder(ICommClient client, bool isValueInclComm)
        {
            Price price = client.Price;
            if (price == null)
            {
                IPriceDetail priceDetail = client.TradedInstrument.CurrentPrice;
                if (priceDetail != null)
                {
                    price = priceDetail.Price;
                    SetCommissionInfoOnOrder(client, string.Format("Use current price {0} from date {1}", price.ToString(), priceDetail.Date.ToShortDateString()));
                }
            }
            else if (client.Price != null)
            {
                price = client.Price;
                SetCommissionInfoOnOrder(client, string.Format("Use price {0} from order", client.Price.ToString()));
            }

            if (price == null)
            {
                SetCommissionInfoOnOrder(client, "No price available so the commission is €0");
                return null;
            }
            else
            {
                if (isValueInclComm)
                {
                    // Value includes the commission 
                    return (client.Amount + client.PreviousCalculatedFee).CalculateSize(price);
                }
                else
                {
                    // Value does not include the commission
                    return (client.Amount - client.PreviousCalculatedFee).CalculateSize(price);
                }
            }
        }
	}
}
