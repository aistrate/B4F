 using System;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public interface IManagementFeeComponent : IGeneralOperationsComponent
    {
        int Period { get; set; }
        FeeType MgtFeeType { get; set; }
    }
}
