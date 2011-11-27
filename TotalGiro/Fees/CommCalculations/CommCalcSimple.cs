using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.CommCalculations
{
	/// <summary>
	/// Calculates a simple, constant commission.
	/// </summary>
    public class CommCalcSimple: CommCalc
	{
        protected CommCalcSimple() { }

		public CommCalcSimple(string name, ICurrency commCurrency, Money fixedSetup)
			: base(name, commCurrency, null, null, fixedSetup)
		{ }


        /// <summary>
        /// Indicates whether the commission calculation is flat, slab, size based, or simple.
        /// </summary>
        public override FeeCalcTypes CalcType
        {
            get { return FeeCalcTypes.Simple; }
        }

        /// <summary>
        /// Calculates a simple, constant commission for a given order.
        /// </summary>
        /// <param name="client">The order for which to calculate the commission.</param>
        /// <returns>The value of the commission.</returns>
        public override Money Calculate(ICommClient client)
		{
			if (FixedSetup != null)
			{
                SetCommissionInfoOnOrder(client, string.Format("Fixed commission of {0}.", FixedSetup.ToString()));
				return FixedSetup;
			}
			else
			{
                SetCommissionInfoOnOrder(client, "No commission needed.");
				return new Money(0m, CommCurrency);
			}
		}
	}
}
