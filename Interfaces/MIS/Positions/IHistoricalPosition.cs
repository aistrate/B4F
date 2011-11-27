using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
namespace B4F.TotalGiro.MIS.Positions
{
    public interface IHistoricalPosition
    {
        IAccountTypeInternal Account { get; set; }
        int Key { get; set; }
        DateTime PositionDate { get; set; }
        InstrumentSize ValueSize { get; set; }
    }
}
