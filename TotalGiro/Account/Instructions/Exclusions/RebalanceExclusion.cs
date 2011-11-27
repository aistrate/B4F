using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Instructions.Exclusions
{
    public abstract class RebalanceExclusion: IRebalanceExclusion
    {
        public virtual int Key { get; set; }
        public virtual IRebalanceInstruction Parent { get; set; }
        public abstract int ComponentKey { get; }
        public abstract ModelComponentType ComponentType { get; }
        public abstract string ComponentName { get; }

        public DateTime CreationDate
        {
            get { return creationDate; }
        }
        private DateTime creationDate;
    }
}
