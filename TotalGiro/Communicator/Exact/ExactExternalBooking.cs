using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ExactExternalBooking : IExactExternalBooking 
    {

        public ExactExternalBooking()
        {

        }

        public int Key { get; set; }
        public DateTime BookDate { get; set; }
        public string BookingNumber { get; set; }
        public string Journal  { get; set; }
        public Int16 LineNumber { get; set; }
        public string GeneralLedgerAccount { get; set; }
        public Money ExactAmount { get; set; }

    }
}
