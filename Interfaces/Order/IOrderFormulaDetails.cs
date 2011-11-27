using System;

namespace B4F.TotalGiro.Orders
{
    public interface IOrderFormulaDetails
    {
        int Key { get; }
        IOrder TopParentOrder { get; }
        string TopParentDisplayStatus { get; }
    }
}
