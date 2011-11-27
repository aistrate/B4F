using System;
using System.Collections.Generic;
//using B4F.TotalGiro.Orders;
//using B4F.TotalGiro.Orders.Transactions;
//using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Class representing a stock
    /// </summary>
    public class Stock : SecurityInstrument, IStock
    {
        public Stock()
        {
            initialize();
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.Stock;
        }

        public override bool Transform(DateTime changeDate, decimal oldChildRatio, byte newParentRatio, bool isSpinOff,
                string instrumentName, string isin, DateTime issueDate)
        {
            Stock newStock = new Stock();
            return transform(newStock, changeDate, oldChildRatio, newParentRatio, isSpinOff, instrumentName, isin, issueDate);
        }

        public override bool Validate()
        {
            return base.validate();
        }
    }
}
