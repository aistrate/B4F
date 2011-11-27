using System;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class is returned after a size of an instrument is predicted using the last known price/rate of the instrument.
    /// After the prediction the instance of this class will hold the <see cref="T:B4F.TotalGiro.Instruments.PredictedSizeReturnValue">status</see>, size and used rate. 
    /// </summary>
    public class PredictedSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.PredictedSize">PredictedSize</see> class.
        /// </summary>
        /// <param name="status">The <see cref="T:B4F.TotalGiro.Instruments.PredictedSizeReturnValue">status</see> of the prediction</param>
        public PredictedSize(PredictedSizeReturnValue status)
        {
            this.status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.PredictedSize">PredictedSize</see> class.
        /// </summary>
        /// <param name="size">The predicted size</param>
        /// <param name="rateDate">The date of the used rate to predict the size</param>
        public PredictedSize(InstrumentSize size, DateTime rateDate)
        {
            this.size = size;
            this.rateDate = rateDate;
        }

        /// <summary>
        /// The predicted size
        /// </summary>
        public InstrumentSize Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// The <see cref="T:B4F.TotalGiro.Instruments.PredictedSizeReturnValue">status</see> of the prediction
        /// </summary>
        public PredictedSizeReturnValue Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// The rate used for the prediction
        /// </summary>
        public string Rate
        {
            get { return rate; }
            set { rate = value; }
        }

        /// <summary>
        /// The date of the used rate to predict the size
        /// </summary>
        public DateTime RateDate
        {
            get { return rateDate; }
            set
            {
                rateDate = value;
                if (IsOldDate)
                    Status = PredictedSizeReturnValue.OldRateDate;
                else
                    Status = PredictedSizeReturnValue.OK;
            }
        }

        /// <summary>
        /// Read-only. Was the date of the used rate to predict the size an old date?
        /// </summary>
        public bool IsOldDate
        {
            get
            {
                bool retVal = true;
                TimeSpan s = DateTime.Now.Date - RateDate.Date;
                double numberofdays = s.TotalDays;
                if (numberofdays < 3)
                    retVal = false;
                return retVal;
            }
        }

        #region Private Variables

        private InstrumentSize size;
        private PredictedSizeReturnValue status;
        private DateTime rateDate = DateTime.Now;
        private string rate;

        #endregion
    }
}
