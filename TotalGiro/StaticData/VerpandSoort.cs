using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.StaticData
{
    public class VerpandSoort : IVerpandSoort
    {
        public VerpandSoort() { }

        public virtual int Key { get; set; }
        public virtual string Description { get; set; }
        public virtual IManagementCompany VerpandOwner { get; set; }

    }
}
