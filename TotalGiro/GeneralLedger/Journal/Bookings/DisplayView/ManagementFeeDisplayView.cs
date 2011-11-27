using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings.DisplayView
{
    public class ManagementFeeDisplayView : BookingDisplayView
    {
        public ManagementFeeDisplayView(ManagementFee ParentManagementFee)
        {

        }
        public ManagementFee ParentManagementFee { get; set; }

    }
}
