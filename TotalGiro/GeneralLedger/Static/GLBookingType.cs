using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class GLBookingType : IGLBookingType
    {
        protected GLBookingType() { }
        public virtual int Key { get; set; }
        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
