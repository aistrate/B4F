using System;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public interface IGLBookingType
    {
        int Key { get; set; }
        string Name { get; }
        string Description { get; }
    }
}
