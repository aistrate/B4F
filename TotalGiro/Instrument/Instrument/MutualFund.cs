using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments.History;
using System.Data.SqlTypes;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class representing a Mutual fund
    /// </summary>
    public class MutualFund : SecurityInstrument, IMutualFund
	{
		public MutualFund() 
        {
            initialize();
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.MutualFund;
        }

        /// <summary>
        /// Get/set screen representation of name
        /// </summary>
        public override string DisplayName
        {
            get 
            {
                string curInfo = "";
                if (CurrencyNominal != null)
                {
                    if (!string.IsNullOrEmpty(CurrencyNominal.AltSymbol) && !this.Name.Contains(CurrencyNominal.AltSymbol))
                        curInfo = " (" + (CurrencyNominal.AltSymbol == string.Empty ? CurrencyNominal.ToString() : CurrencyNominal.AltSymbol) + ")";
                }
                return this.Name + curInfo;
            }
        }

        /// <summary>
        /// Get/set administrative fee
        /// </summary>
		public virtual string AdminFee
        {
            get { return adminFee; }
            set { adminFee = value; }
        }

        /// <summary>
        /// Get/set administrative buying cost
        /// </summary>
		public virtual decimal BuyCost
        {
            get { return buyCost; }
            set { buyCost = value; }
        }

       /// <summary>
        /// Get/set administrative selling cost
        /// </summary>
        public virtual decimal SellCost
        {
            get { return sellCost; }
            set { sellCost = value; }
        }

        /// <summary>
        /// Get/set tax amount paid buying a fund.
        /// Tax is obsolete in Netherlands
        /// </summary>
		public virtual decimal CapitalisationCost
        {
            get { return capitalisationCost; }
            set { capitalisationCost = value; }
        }

        // TODO RatingMS
		public virtual string RatingMS
        {
            get { return ratingMS; }
            set { ratingMS = value; }
        }

        /// <summary>
        /// Get/set dividend flag
        /// </summary>
        public virtual bool Dividend
        {
            get { return dividend; }
            set { dividend = value; }
        }

        /// <summary>
        /// Get/set overridden composition of name
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString()
        {
            return DisplayName;
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
        /// Get/set screen format of numbers with much decimals
        /// </summary>
        /// <param name="Quantity"></param>
        /// <returns></returns>
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

        public override bool Validate()
        {
            return base.validate();
        }

        public override bool Transform(DateTime changeDate, decimal oldChildRatio, byte newParentRatio, bool isSpinOff,
                string instrumentName, string isin, DateTime issueDate)
        {
            MutualFund newFund = new MutualFund();
            newFund.AdminFee = this.AdminFee;
            newFund.BuyCost = this.BuyCost;
            newFund.CapitalisationCost = this.CapitalisationCost;
            newFund.Dividend = this.Dividend;
            newFund.RatingMS = this.RatingMS;
            newFund.SellCost = this.SellCost;
            return transform(newFund, changeDate, oldChildRatio, newParentRatio, isSpinOff, instrumentName, isin, issueDate);
        }

		#region Private Variables

		private string adminFee;
		private decimal buyCost;
		private decimal sellCost;
		private decimal capitalisationCost;
		private string ratingMS;
		private bool dividend;

		#endregion
    }
}
