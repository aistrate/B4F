using System;
namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IExactExternalBooking
    {
        DateTime BookDate { get; set; }
        string BookingNumber { get; set; }
        B4F.TotalGiro.Instruments.Money ExactAmount { get; set; }
        string GeneralLedgerAccount { get; set; }
        string Journal { get; set; }
        int Key { get; set; }
        short LineNumber { get; set; }
    }
}
