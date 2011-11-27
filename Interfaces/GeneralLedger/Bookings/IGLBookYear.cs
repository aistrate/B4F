using System;
namespace B4F.TotalGiro.GeneralLedger.Static
{
    public interface IGLBookYear
    {
        int BookYear { get; set; }
        int Key { get; set; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}
