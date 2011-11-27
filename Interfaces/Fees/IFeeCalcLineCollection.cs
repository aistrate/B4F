using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Fees.FeeCalculations
{
    public interface IFeeCalcLineCollection : IGenericCollection<IFeeCalcLine>
    {
        void ArrangeLines();
        IFeeCalcLine GetItemBySerialNo(int serialNo);
    }
}
