using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions
{
    /// <summary>
    /// Parameters that are used during the rebalance
    /// </summary>
    public class InstructionEngineParameters
    {
        public InstructionEngineParameters() { }

        public InstructionEngineParameters(PricingTypes pricingType, decimal minimumQty, ICurrency underlying)
        {
            this.pricingType = pricingType;
            switch (pricingType)
            {
                case PricingTypes.Direct:
                    this.minimumRebalanceAmount = new Money(minimumQty, underlying);
                    break;
                case PricingTypes.Percentage:
                    this.minimumRebalancePercentage = minimumQty;
                    break;
            }
        }
        
        /// <summary>
        /// The minimum amount (either negative or positive) that an order has to match during the rebalance
        /// </summary>
        public Money MinimumRebalanceAmount
        {
            get { return this.minimumRebalanceAmount; }
            set { this.minimumRebalanceAmount = value; }
        }

        /// <summary>
        /// The minum percentage (of the model) that an order must match
        /// </summary>
        public decimal MinimumRebalancePercentage
        {
            get { return minimumRebalancePercentage; }
            set { minimumRebalancePercentage = value; }
        }

        /// <summary>
        /// Are there any Tolerance parameters set?
        /// </summary>
        public bool IsToleranceParameterSet
        {
            get 
            {
                bool retVal = false;

                if (pricingType == PricingTypes.Direct && MinimumRebalanceAmount != null && MinimumRebalanceAmount.IsNotZero)
                    retVal = true;
                else if (pricingType == PricingTypes.Percentage && MinimumRebalancePercentage != 0M)
                    retVal = true;
                return retVal;
            }
        }

        /// <summary>
        /// Checks the value against the tolerance parameters (but only wheb they are set)
        /// </summary>
        /// <param name="orderValue">The value to check</param>
        /// <param name="totalPortfolioValue">The total portfolio value</param>
        /// <returns></returns>
        public bool IsOrderValueWithinTolerance(Money orderValue, Money totalPortfolioValue)
        {
            bool retVal = false;
            Money orderVal;

            if (IsToleranceParameterSet)
            {
                switch (PricingType)
                {
                    case PricingTypes.Direct:
                        orderVal = orderValue.Abs();
                        if (!orderVal.Underlying.Equals(MinimumRebalanceAmount.Underlying))
                            orderVal = orderVal.Convert((ICurrency)MinimumRebalanceAmount.Underlying);

                        if (orderVal >= MinimumRebalanceAmount)
                            retVal = true;
                        break;
                    case PricingTypes.Percentage:
                        orderVal = orderValue.Abs();
                        if (!orderVal.Underlying.Equals(totalPortfolioValue.Underlying))
                            orderVal = orderVal.Convert((ICurrency)totalPortfolioValue.Underlying);

                        decimal perc = orderVal.Quantity / totalPortfolioValue.Abs().Quantity;
                        if (perc >= MinimumRebalancePercentage)
                            retVal = true;
                        break;
                }
            }
            else
                retVal = true;
            return retVal;
        }

        public PricingTypes PricingType
        {
            get { return pricingType; }
            set { pricingType = value; }
        }
	

        #region Private Variables

        private int key;
        private Money minimumRebalanceAmount;
        private decimal minimumRebalancePercentage;
        private PricingTypes pricingType;

        #endregion

    }
}
