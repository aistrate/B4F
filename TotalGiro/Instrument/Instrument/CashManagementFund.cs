using System;
using System.Collections.Generic;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class representing mutual fund for cash positions
    /// </summary>
    public class CashManagementFund : SecurityInstrument, ICashManagementFund
	{
		public CashManagementFund()
		{
            initialize();
		}

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.CashManagementFund;
        }

        /// <summary>
        /// Overridden composition of the name of this class
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return this.Name.ToString();
		}

        /// <summary>
        /// Get/set number of decimals
        /// </summary>
		public override int DecimalPlaces
		{
			get
			{
				int places = base.DecimalPlaces;
				if (places == 0)
					places = 6;
				return places;
			}
			set { base.DecimalPlaces = value; }
		}

        /// <summary>
        /// Change screen format of a number
        /// </summary>
        /// <param name="Quantity">Quantity</param>
        /// <returns>Formatted number</returns>
		public override string DisplayToString(decimal Quantity)
		{
			if (DecimalPlaces > 0)
			{
				string places = new string('0', DecimalPlaces);
				string format = "#,##0." + places;
				return Quantity.ToString(format);
			}
			else
			{
				return Quantity.ToString("#,##0.000000");
			}
		}

        public override bool Transform(DateTime changeDate, decimal oldChildRatio, byte newParentRatio, bool isSpinOff,
                string instrumentName, string isin, DateTime issueDate)
        {
            CashManagementFund newFund = new CashManagementFund();
            newFund.DecimalPlaces = this.DecimalPlaces;
            return transform(newFund, changeDate, oldChildRatio, newParentRatio, isSpinOff, instrumentName, isin, issueDate);
        }

        public override bool Validate()
        {
            return base.validate();
        }

        public override bool IsCashManagementFund
        {
            get { return true; }
        }
    }
}
