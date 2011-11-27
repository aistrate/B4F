using System;
using System.Collections;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.GeneralLedger.Static;
namespace B4F.TotalGiro.Instruments
{
    public class BenchMark : InstrumentsWithPrices, IBenchMark
    {
        public BenchMark() 
        {
            initialize();
        }

        /// <exclude/>
        private void initialize()
        {
            this.secCategoryID = SecCategories.BenchMark;
        }


        public override bool Validate()
        {
            return base.validate();
        }

        public override bool CalculateCosts(IOrderAllocation transaction, IFeeFactory feeFactory, IGLLookupRecords lookups)
        {
            return true;
        }
        public override bool CalculateCosts(IOrder order, IFeeFactory feeFactory)
        {

            return true;
        }

        public override PredictedSize PredictSize(Money inputAmount)
        {
            PredictedSize retVal = new PredictedSize(PredictedSizeReturnValue.NoRate);
             return retVal;
        }

        

        PredictedSize IInstrument.PredictSize(Money inputAmount)
        {
            throw new Exception("The method or operation is not implemented.");
        }



        /// <summary>
        /// Get tradeable flag
        /// </summary>
        public override bool IsTradeable
        {
            get { return false; }
        }

        /// <summary>
        /// Get cash flag
        /// </summary>
        public override bool IsCash
        {
            get { return false; }
        }
    }
}
