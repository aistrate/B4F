using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.MIS.Positions
{
    public class HistoricalPosition : IHistoricalPosition
    {
        public HistoricalPosition() { }
        public HistoricalPosition(int key, IInstrumentsWithPrices instrument, decimal valueSize,
            DateTime positionDate, IAccountTypeInternal account)
        {
            this.Key = key;
            this.ValueSize = new InstrumentSize(valueSize, instrument);
            this.PositionDate = positionDate;
            this.Account = account;
        }


        public int Key { get; set; }
        public IAccountTypeInternal Account { get; set; }
        public InstrumentSize ValueSize { get; set; }
        public DateTime PositionDate { get; set; }
        
    }
}
