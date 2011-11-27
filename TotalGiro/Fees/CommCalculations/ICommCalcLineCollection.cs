using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Fees.CommCalculations
{
    public interface ICommCalcLineCollection: IList<CommCalcLine>
    {
        CommCalc Parent { get; set; }
        void AddCalculation(CommCalcLine item);
        bool RemoveCalculation(CommCalcLine item);
        void InsertCalculation(int index, CommCalcLine item);
        void RemoveCalculationAt(int index);
        void ArrangeLines();
    }
}
