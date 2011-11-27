using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.DataMigration.MissingHistoricalData
{
    public class MissingHistoricalInstrumentData
    {
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public virtual IInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        public virtual DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public virtual bool IsBizzDay
        {
            get { return isBizzDay; }
            set { isBizzDay = value; }
        }

        #region Privates

        private int key;
        private IInstrument instrument;
        private DateTime date;
        private bool isBizzDay;

        #endregion
    }
}
