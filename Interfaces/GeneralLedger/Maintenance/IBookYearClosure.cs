using System;
using B4F.TotalGiro.GeneralLedger.Static;
namespace B4F.TotalGiro.GeneralLedger.Journal.Maintenance
{
    public interface IBookYearClosure
    {
        IGLBookYear BookYear { get; set; }
        string CreatedBy { get; set; }
        DateTime CreationDate { get; }
        int Key { get; set; }
    }
}
