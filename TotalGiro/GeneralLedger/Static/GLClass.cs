using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class GLClass : IGLClass
    {
        public int Key { get; set; }
        public string GLClassNumber { get; set; }
        public string GLClassDescription { get; set; }


    }
}
