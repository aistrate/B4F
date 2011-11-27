using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.BackOffice.Orders
{
    public class PredefinedBeneficiary
    {
        public int Key { get; set; }
        public string SwiftAddress { get; set; }
        public string BenefBankAcctNr { get; set; }
        public string NarBenef1 { get; set; }
        public string NarBenef2 { get; set; }
        public string NarBenef3 { get; set; }
        public string NarBenef4 { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public IndicationOfCosts CostIndication { get; set; }
    }
}
