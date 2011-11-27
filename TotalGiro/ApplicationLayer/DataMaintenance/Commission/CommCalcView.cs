using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission
{
    public class CommCalcView
    {
        public string Name;
        public int CalcType;
        public decimal MinValue, FixedSetup;
        public decimal? MaxValue;

        public List<CommCalcLineView> LineViews = new List<CommCalcLineView>();
    }

    public class CommCalcLineView
    {
        public short SerialNo;
        public decimal LowerRange, StaticCharge, FeePercentage, Tariff;
        public bool IsAmountBased;
    }
}
